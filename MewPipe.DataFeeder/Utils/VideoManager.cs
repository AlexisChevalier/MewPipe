using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MewPipe.DataFeeder.Entities;
using YoutubeExtractor;

namespace MewPipe.DataFeeder.Utils
{
	public static class VideoManager
	{
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

		#endregion
	}
}