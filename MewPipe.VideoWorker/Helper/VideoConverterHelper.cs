using System;
using System.IO;
using MewPipe.Logic.Models;
using MongoDB.Driver.GridFS;
using NReco.VideoConverter;

namespace MewPipe.VideoWorker.Helper
{
	public static class VideoConverterHelper
	{
		private static readonly FFMpegConverter Converter = new FFMpegConverter();
		private static bool _showingProgress;

		private const string AudioCodec = "aac";
		private const int AudioSampleRate = 44100;
		private const string VideoCodec = "h264";
		private const int VideoFramerate = 30;

		private const string OutPrefix = "out";

		#region Private helpers

		private static ConvertSettings GetConvertSettings(string frameSize, string customOutputArgs)
		{
			return new ConvertSettings
			{
				AudioCodec = AudioCodec,
				AudioSampleRate = AudioSampleRate,
				VideoCodec = VideoCodec,
				VideoFrameRate = VideoFramerate,
				VideoFrameSize = frameSize,
				CustomOutputArgs = customOutputArgs
			};
		}

		private static void EnsureShowingProgress()
		{
			if (_showingProgress) return;
			Converter.ConvertProgress += delegate(object sender, ConvertProgressEventArgs args)
			{
				var processed = args.Processed.TotalSeconds;
				var total = args.TotalDuration.TotalSeconds;
				var conversionPercent = Math.Floor(processed/total*100);
				Console.Write("\rConversion: " + conversionPercent + "%");
				if (conversionPercent >= 100) Console.WriteLine(" done !");
			};
			_showingProgress = true;
		}

		private static void CopyToStream(string filePath, Stream outputStream)
		{
			if (outputStream == null) return;

			using (var fileStream = File.OpenRead(filePath))
			{
				fileStream.Seek(0, SeekOrigin.Begin);
				fileStream.CopyTo(outputStream, 255*1024); // 255Ko buffer
			}
		}

		#endregion

		public static void DoConversion(string inputFilePath, MimeType mimeType, QualityType qualityType,
			MongoGridFSStream mongoStream)
		{
			if (mimeType.Name.Equals("MP4"))
			{
				if (qualityType.Name.Equals("1080")) To1080Mp4(inputFilePath, mongoStream);
				else if (qualityType.Name.Equals("720")) To720Mp4(inputFilePath, mongoStream);
				else if (qualityType.Name.Equals("480")) To480Mp4(inputFilePath, mongoStream);
				else if (qualityType.Name.Equals("360")) To360Mp4(inputFilePath, mongoStream);
			}
			else if (mimeType.Name.Equals("OGG"))
			{
				/* TODO: Replace with ToXXXOgg when done 
				if (qualityType.Name.Equals("1080")) To1080Mp4(inputFilePath, mongoStream);
				else if (qualityType.Name.Equals("720")) To720Mp4(inputFilePath, mongoStream);
				else if (qualityType.Name.Equals("480")) To480Mp4(inputFilePath, mongoStream);
				else if (qualityType.Name.Equals("360")) To360Mp4(inputFilePath, mongoStream);
				*/
			}
		}

		#region Conversions: 1080p to 360p MP4

		public static void To1080Mp4(string inputPath, Stream outputStream)
		{
			EnsureShowingProgress();

			var convertSettings = GetConvertSettings("1920x1080", "-strict -2 -crf 17");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "1080.mp4";

			// Show params in console
			//converter.PrintFFMpegParams(null, Format.mp4, convertSettings);

			Console.WriteLine("Converting to 1080p MP4 ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.mp4, convertSettings);

			CopyToStream(outputPath, outputStream);
		}

		public static void To720Mp4(string inputPath, Stream outputStream)
		{
			EnsureShowingProgress();

			var convertSettings = GetConvertSettings("1280x720", "-strict -2 -crf 17");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "720.mp4";

			// Show params in console
			//converter.PrintFFMpegParams(null, Format.mp4, convertSettings);

			Console.WriteLine("Converting to 720p MP4 ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.mp4, convertSettings);

			CopyToStream(outputPath, outputStream);
		}

		public static void To480Mp4(string inputPath, Stream outputStream)
		{
			EnsureShowingProgress();

			var convertSettings = GetConvertSettings("854x480", "-strict -2 -crf 17");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "480.mp4";

			// Show params in console
			//converter.PrintFFMpegParams(null, Format.mp4, convertSettings);

			Console.WriteLine("Converting to 480p MP4 ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.mp4, convertSettings);

			CopyToStream(outputPath, outputStream);
		}

		public static void To360Mp4(string inputPath, Stream outputStream)
		{
			EnsureShowingProgress();

			var convertSettings = GetConvertSettings("640x360", "-strict -2 -crf 17");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "360.mp4";

			// Show params in console
			//converter.PrintFFMpegParams(null, Format.mp4, convertSettings);

			Console.WriteLine("Converting to 360p MP4 ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.mp4, convertSettings);

			CopyToStream(outputPath, outputStream);
		}

		#endregion
	}
}