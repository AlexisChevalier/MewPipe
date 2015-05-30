using System;
using System.Text.RegularExpressions;
using MewPipe.Logic.Models;
using MewPipe.Logic.Services;
using NReco.VideoConverter;

namespace MewPipe.VideoWorker.Helper
{
	public static class VideoInfosHelper
	{
		private static readonly VideoQualityTypeService VideoQualityTypeService = new VideoQualityTypeService();

		/// <summary>
		/// Get and returns the number of frames a video on the disk contains.
		/// </summary>
		/// <param name="videoPath">The path of the video to get the frames count of.</param>
		/// <returns>The number of frames the video contains.</returns>
		public static int GetFramesCount(string videoPath)
		{
			var framesRegex = new Regex(@"frame= ([0-9]*) fps");
			string framesStr = null;

			var ffMpeg = new FFMpegConverter();
			ffMpeg.LogReceived += delegate(object sender, FFMpegLogEventArgs args)
			{
				var ffmpegOutput = args.Data;
				if (!ffmpegOutput.Contains("frame= ")) return;
				framesStr = framesRegex.Match(ffmpegOutput).Groups[1].Value;
			};
			ffMpeg.Invoke("-i " + videoPath + " -vcodec copy -acodec copy -f NULL NULL");

			if (framesStr == null) return -1;
			return int.Parse(framesStr) + 1;
		}

		/// <summary>
		/// Get and returns the closest QualityType from the given Y resolution.
		/// </summary>
		/// <param name="resolutionY">The Y resolution to get the closest QualityType from.</param>
		/// <returns>The closest QualityType from the given Y resolution</returns>
		/// <example>GetClosestQualityType(544) will return the 480 QualityType instance.</example>
		public static QualityType GetClosestQualityType(int resolutionY) // Supporting only 16:9 videos
		{
			var qualities = VideoQualityTypeService.GetEncodingQualityTypes();

			QualityType closestQuality = null;
			var minDiff = 99999;
			foreach (var quality in qualities)
			{
				try
				{
					var qualityResY = int.Parse(quality.Name);
					var diff = Math.Abs(resolutionY - qualityResY);
					if (diff > minDiff) continue;
					closestQuality = quality;
					minDiff = diff;
				}
				catch (Exception)
				{
					// continue;
				}
			}
			return closestQuality;
		}

		/// <summary>
		/// Guess and returns the closest video's QualityType.
		/// </summary>
		/// <param name="videoPath">The path to the video to get the QualityType of.</param>
		/// <returns>The closest video's QualityType</returns>
		/// <example>A video in 968x544 will return the 480 QualityType instance.</example>
		public static QualityType GuessVideoQualityType(string videoPath)
		{
			var resolutionRgx = new Regex(@", ([0-9]*)x([0-9]*)");
			string resolutionXStr = null;
			string resolutionYStr = null;

			var ffMpeg = new FFMpegConverter();
			ffMpeg.LogReceived += delegate(object sender, FFMpegLogEventArgs args)
			{
				var ffmpegOutput = args.Data;
				if (!ffmpegOutput.Contains("Video: ")) return;
				var groups = resolutionRgx.Match(ffmpegOutput).Groups;
				resolutionXStr = groups[1].Value;
				resolutionYStr = groups[2].Value;
			};
			ffMpeg.Invoke("-i " + videoPath + " -vcodec copy -acodec copy -f NULL NULL");

			if (resolutionXStr == null || resolutionYStr == null) return null;
			return GetClosestQualityType(int.Parse(resolutionYStr));
		}

		public static void ShowFFmpegHeader()
		{
			var ffMpeg = new FFMpegConverter();
			ffMpeg.LogReceived += delegate(object sender, FFMpegLogEventArgs args)
			{
				var ffmpegOutput = args.Data;
				if (ffmpegOutput.Contains("usage: ffmpeg") || ffmpegOutput.Contains("Use -h")) return;
				Console.WriteLine(ffmpegOutput);
			};
			try
			{
				ffMpeg.Invoke("");
			}
			catch (Exception)
			{
			}
		}
	}
}