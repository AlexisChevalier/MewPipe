using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using MewPipe.DataFeeder.Entities;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Models;
using MewPipe.Logic.MongoDB;
using MewPipe.Logic.RabbitMQ;
using MewPipe.Logic.RabbitMQ.Messages;
using MewPipe.Logic.Repositories;
using MewPipe.Logic.Services;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using YoutubeExtractor;

namespace MewPipe.DataFeeder.Utils
{
	public static class VideoManager
	{
		private static UnitOfWork _unitOfWork = UnitOfWorkSingleton.GetInstance();
		private static IVideoGridFsClient _videoGridFsClient = new VideoGridFsClient();
		private static IVideoMimeTypeService _videoMimeTypeService = new VideoMimeTypeService();
		private static IVideoQualityTypeService _videoQualityTypeService = new VideoQualityTypeService();

		private static readonly string _cacheFolder = @"_cache";

		public static MewPipeVideo Download(string url)
		{
			IEnumerable<VideoInfo> vInfos = DownloadUrlResolver.GetDownloadUrls(url);
			var videoInfos = vInfos.Where(v => v.VideoType == VideoType.Mp4).ToList();
			var lowestResolution = GetLowestResolution(videoInfos);
			VideoInfo video = videoInfos.First(v => v.Resolution == lowestResolution);

			// Making sure the cache folder does exist
			CreateCacheFolder();

			var videoTitle = GetSafeVideoTitle(video.Title);
			var filePath = Path.Combine(_cacheFolder, videoTitle + ".mp4");

			var mewPipeVideo = ConvertToMewPipeVideo(video, url);
			mewPipeVideo.FilePath = filePath;

			// Checking if we have that video in the cache folder
			if (IsVideoAlreadyCached(videoTitle))
			{
				Console.WriteLine("Video " + videoTitle + " is already cached ! Skipping download ...");
				return mewPipeVideo;
			}

			// We don't have that video in the cache folder, Downloading it ...

			var videoDownloader = new VideoDownloader(video, filePath);

			// Register the ProgressChanged event and print the current progress
			videoDownloader.DownloadProgressChanged +=
				(sender, args) => Console.Write("\rDownloading: " + Math.Floor(args.ProgressPercentage) + "%");

			Console.WriteLine("Downloading " + videoTitle + " ...");
			videoDownloader.Execute();
			Console.WriteLine("\nDownloaded " + videoTitle + " !\n");

			return mewPipeVideo;
		}

		public static void FakeImpressions(MewPipeVideo video, List<ExcelUser> excelUsers, Dictionary<string, User> users)
		{
			var random = new Random();
			var impressions = new List<Impression>();
			var videoCategory = video.Category;
			foreach (var excelUser in excelUsers)
			{
				var likePercent = excelUser.VideoGameInterest;
				if (videoCategory == "Sport") likePercent = excelUser.SportInterest;
				if (videoCategory == "Music") likePercent = excelUser.MusicInterest;

				var impressionType = (random.Next(100) < likePercent)
					? Impression.ImpressionType.Good
					: Impression.ImpressionType.Bad;

				// Dislike rape prevention :D
				if (impressionType == Impression.ImpressionType.Bad)
				{
					if (excelUser.DislikeCount >= 5) continue; // He can't dislike anymore, he won't rate.
					excelUser.DislikeCount++;
				}

				impressions.Add(new Impression
				{
					User = users[excelUser.FullName],
					Type = impressionType,
					DateTimeUtc = DateTime.UtcNow
				});
			}

			video.Impressions = impressions;
		}

		public static bool IsVideoUploaded(string videoTitle)
		{
			return _unitOfWork.VideoRepository.GetOne(video => video.Name.Equals(videoTitle)) != null;
		}

		public static void UploadToMewPipe(MewPipeVideo mewpipeVideo)
		{
			var user = _unitOfWork.UserRepository.GetOne(u => u.UserName.Equals(mewpipeVideo.Author));
			var category = _unitOfWork.CategoryRepository.GetOne(c => c.Name.Equals(mewpipeVideo.Category));

			var video = new Video
			{
				User = user,
				Name = mewpipeVideo.Title,
				Description = mewpipeVideo.Description,
				Category = category,
				Views = mewpipeVideo.Views,
				Impressions = new Collection<Impression>(),
				//
				DateTimeUtc = DateTime.UtcNow,
				PrivacyStatus = Video.PrivacyStatusTypes.Public,
				Status = Video.StatusTypes.Processing,
				AllowedUsers = new List<User>(),
				VideoFiles = new List<VideoFile>(),
				NotificationHookUri = "",
				UploadRedirectUri = "",
				Tags = new List<Tag>()
			};
			_unitOfWork.VideoRepository.Insert(video);
			_unitOfWork.Save();

			// Need to save the impressions first
			foreach (var impression in mewpipeVideo.Impressions)
			{
				impression.Video = video;
				_unitOfWork.ImpressionRepository.Insert(impression);
			}
			_unitOfWork.Save();

			// Upload

			var name = String.Format("{0}-VideoFile-{1}-{2}", new ShortGuid(video.Id), "video/mp4", "Uploaded");

			name = name.Replace("/", "_");

			var gridFsOptions = new MongoGridFSCreateOptions
			{
				Id = ObjectId.GenerateNewId(),
				UploadDate = DateTime.UtcNow,
				ContentType = "video/mp4"
			};


			var mongoStream = TryGetStream(name, gridFsOptions);
			using (FileStream fileStream = File.OpenRead(mewpipeVideo.FilePath))
			{
				fileStream.CopyTo(mongoStream, 255*1024); // 255 Ko buffer size
			}
			mongoStream.Close();
			mongoStream.Dispose();

			var mimeTypeService = new VideoMimeTypeService(_unitOfWork);
			var qualityTypeService = new VideoQualityTypeService(_unitOfWork);

			video.VideoFiles.Add(new VideoFile
			{
				Video = video,
				IsOriginalFile = true,
				MimeType = mimeTypeService.GetAllowedMimeTypeForDecoding("video/mp4"),
				QualityType = qualityTypeService.GetUploadingQualityType()
			});

			_unitOfWork.VideoRepository.Update(video);
			_unitOfWork.Save();

			using (var workerQueueManager = new WorkerQueueManager())
			{
				using (var channelQueue =
					workerQueueManager.GetChannelQueue(WorkerQueueManager.QueueChannelIdentifier.NewVideos))
				{
					channelQueue.SendPersistentMessage(new NewVideoMessage(video.PublicId));
				}
				using (var channelQueue =
					workerQueueManager.GetChannelQueue(WorkerQueueManager.QueueChannelIdentifier.RecommendationsUpdates))
				{
					channelQueue.SendPersistentMessage(new RecommendationsUpdateMessage(video.PublicId));
				}
			}
		}

		#region Private Helpers

		private static int GetLowestResolution(IEnumerable<VideoInfo> vInfos)
		{
			var lowest = 1080;
			foreach (var videoInfo in vInfos)
			{
				if (videoInfo.Resolution < lowest && videoInfo.Resolution > 0)
					lowest = videoInfo.Resolution;
			}
			return lowest;
		}

		private static void CreateCacheFolder()
		{
			if (!Directory.Exists(_cacheFolder))
				Directory.CreateDirectory(_cacheFolder);
		}

		private static bool IsVideoAlreadyCached(string videoTitle)
		{
			return File.Exists(Path.Combine(_cacheFolder, videoTitle + ".mp4"));
		}

		private static string GetSafeVideoTitle(string videoTitle)
		{
			return videoTitle.Replace("/", "").Replace("|", "").Replace(":", "");
		}

		private static MewPipeVideo ConvertToMewPipeVideo(VideoInfo vInfos, string url)
		{
			string html = GetWebPage(url);
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			var viewsStr = doc.GetElementbyId("watch7-views-info").Element("div").InnerText;
			viewsStr = Regex.Replace(viewsStr, "[^.0-9]", ""); // Removes all non-numeric characters
			viewsStr = Regex.Replace(viewsStr, @"\u00A0", ""); // Removes all non-breakable spaces
			var views = long.Parse(viewsStr);

			var description = doc.GetElementbyId("eow-description").InnerText;

			var category = doc
				.GetElementbyId("watch-description-extras")
				.Element("ul")
				.Element("li")
				.Element("ul")
				.Element("li")
				.InnerText;

			return new MewPipeVideo
			{
				Title = vInfos.Title,
				Description = description,
				Views = views
			};
		}

		private static string GetWebPage(string url)
		{
			string result = null;
			WebResponse response = null;
			StreamReader reader = null;

			try
			{
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
				request.Method = "GET";
				response = request.GetResponse();
				reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
				result = reader.ReadToEnd();
			}
			catch (Exception ex)
			{
			}
			finally
			{
				if (reader != null)
					reader.Close();
				if (response != null)
					response.Close();
			}
			return result;
		}

		private static MongoGridFSStream TryGetStream(string filename, MongoGridFSCreateOptions options)
		{
			var tentative = 0;

			while (true)
			{
				tentative++;
				try
				{
					return _videoGridFsClient.GetDatabase().GridFS.Create(filename, options);
				}
				catch (Exception)
				{
					if (tentative >= 5)
					{
						throw;
					}
					Thread.Sleep(300);
				}
			}
		}

		#endregion
	}
}