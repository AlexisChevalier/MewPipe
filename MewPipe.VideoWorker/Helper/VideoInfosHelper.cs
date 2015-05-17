using System.Text.RegularExpressions;
using NReco.VideoConverter;

namespace MewPipe.VideoWorker.Helper
{
	public static class VideoInfosHelper
	{
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
	}
}