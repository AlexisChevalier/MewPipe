using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YoutubeExtractor;

namespace MewPipe.DataFeeder.Utils
{
	public static class VideoManager
	{
		private static readonly string _cacheFolder = @"_cache";

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

		public static string Download(string url)
		{
			IEnumerable<VideoInfo> vInfos = DownloadUrlResolver.GetDownloadUrls(url);
			var videoInfos = vInfos.Where(v => v.VideoType == VideoType.Mp4).ToList();
			var lowestResolution = GetLowestResolution(videoInfos);
			VideoInfo video = videoInfos.First(v => v.Resolution == lowestResolution);

			// Making sure the cache folder does exist
			CreateCacheFolder();

			var videoTitle = GetSafeVideoTitle(video.Title);
			var filePath = Path.Combine(_cacheFolder, videoTitle + ".mp4");

			if (IsVideoAlreadyCached(videoTitle))
			{
				Console.WriteLine("Video " + videoTitle + " is already cached ! Skipping download ...");
				return filePath;
			}

			var videoDownloader = new VideoDownloader(video, filePath);

			// Register the ProgressChanged event and print the current progress
			videoDownloader.DownloadProgressChanged +=
				(sender, args) => Console.Write("\rDownloading: " + Math.Floor(args.ProgressPercentage) + "%");

			Console.WriteLine("Downloading " + videoTitle + " ...");
			videoDownloader.Execute();
			Console.WriteLine("\nDownloaded " + videoTitle + " !\n");

			return filePath;
		}
	}
}