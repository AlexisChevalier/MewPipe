using System;
using System.IO;
using MewPipe.Logic.Models;
using MewPipe.Logic.Services;
using MongoDB.Driver.GridFS;
using NReco.VideoConverter;

namespace MewPipe.VideoWorker.Helper
{
	public static class VideoConverterHelper
	{
		private static readonly FFMpegConverter Converter = new FFMpegConverter();
		private static bool _showingProgress;

		private const string OutPrefix = "out";

		#region Private helpers

		private static ConvertSettings GetMp4ConvertSettings(string frameSize, string customOutputArgs)
		{
			const string audioCodec = "aac";
			const int audioSampleRate = 44100;
			const string videoCodec = "h264";
			const int videoFramerate = 30;
			return new ConvertSettings
			{
				AudioCodec = audioCodec,
				AudioSampleRate = audioSampleRate,
				VideoCodec = videoCodec,
				VideoFrameRate = videoFramerate,
				VideoFrameSize = frameSize,
				CustomOutputArgs = customOutputArgs
			};
		}

		private static ConvertSettings GetOggConvertSettings(string frameSize, string customOutputArgs)
		{
			const int videoFramerate = 30;
			return new ConvertSettings
			{
				VideoFrameRate = videoFramerate,
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

	    private static void SaveVideoToStorage(string filePath, Video video, MimeType mimeType, QualityType qualityType)
	    {
            if (video == null) return;

			using (var fileStream = File.OpenRead(filePath))
			{
	            var service = new VideoWorkerService();

                service.AddConvertedVideo(video, mimeType, qualityType, fileStream);
	        }
	    }

		#endregion

		public static void DoConversion(string inputFilePath, MimeType mimeType, QualityType qualityType, Video video)
		{
			if (mimeType.Name.Equals("MP4"))
			{
				if (qualityType.Name.Equals("1080")) To1080Mp4(inputFilePath, video, mimeType, qualityType);
                else if (qualityType.Name.Equals("720")) To720Mp4(inputFilePath, video, mimeType, qualityType);
                else if (qualityType.Name.Equals("480")) To480Mp4(inputFilePath, video, mimeType, qualityType);
                else if (qualityType.Name.Equals("360")) To360Mp4(inputFilePath, video, mimeType, qualityType);
			}
			else if (mimeType.Name.Equals("OGG"))
			{
                if (qualityType.Name.Equals("1080")) To1080Ogg(inputFilePath, video, mimeType, qualityType);
                else if (qualityType.Name.Equals("720")) To720Ogg(inputFilePath, video, mimeType, qualityType);
                else if (qualityType.Name.Equals("480")) To480Ogg(inputFilePath, video, mimeType, qualityType);
                else if (qualityType.Name.Equals("360")) To360Ogg(inputFilePath, video, mimeType, qualityType);
			}
		}

		#region Conversions: 1080p to 360p MP4

        public static void To1080Mp4(string inputPath, Video video, MimeType mimeType, QualityType qualityType)
		{
			EnsureShowingProgress();

			var convertSettings = GetMp4ConvertSettings("1920x1080", "-strict -2 -crf 17");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "1080.mp4";

			// Show params in console
			//converter.PrintFFMpegParams(null, Format.mp4, convertSettings);

			Console.WriteLine("Converting to 1080p MP4 ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.mp4, convertSettings);

            SaveVideoToStorage(outputPath, video, mimeType, qualityType);
            //CopyToStream(outputPath, outputStream);
		}

        public static void To720Mp4(string inputPath, Video video, MimeType mimeType, QualityType qualityType)
		{
			EnsureShowingProgress();

			var convertSettings = GetMp4ConvertSettings("1280x720", "-strict -2 -crf 17");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "720.mp4";

			// Show params in console
			//converter.PrintFFMpegParams(null, Format.mp4, convertSettings);

			Console.WriteLine("Converting to 720p MP4 ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.mp4, convertSettings);

            SaveVideoToStorage(outputPath, video, mimeType, qualityType);
		}

        public static void To480Mp4(string inputPath, Video video, MimeType mimeType, QualityType qualityType)
		{
			EnsureShowingProgress();

			var convertSettings = GetMp4ConvertSettings("854x480", "-strict -2 -crf 17");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "480.mp4";

			// Show params in console
			//converter.PrintFFMpegParams(null, Format.mp4, convertSettings);

			Console.WriteLine("Converting to 480p MP4 ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.mp4, convertSettings);

            SaveVideoToStorage(outputPath, video, mimeType, qualityType);
		}

		public static void To360Mp4(string inputPath, Video video, MimeType mimeType, QualityType qualityType)
		{
			EnsureShowingProgress();

			var convertSettings = GetMp4ConvertSettings("640x360", "-strict -2 -crf 17");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "360.mp4";

			// Show params in console
			//converter.PrintFFMpegParams(null, Format.mp4, convertSettings);

			Console.WriteLine("Converting to 360p MP4 ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.mp4, convertSettings);

		    SaveVideoToStorage(outputPath, video, mimeType, qualityType);
		}

		#endregion

		#region Conversions: 1080p to 360p OGG

        public static void To1080Ogg(string inputPath, Video video, MimeType mimeType, QualityType qualityType)
		{
			EnsureShowingProgress();

			var cSettings = GetOggConvertSettings("1920x1080", "-codec:v libtheora -qscale:v 7 -codec:a libvorbis -qscale:a 5");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "1080.ogv";

			Console.WriteLine("Converting to 1080p OGG ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.ogg, cSettings);

            SaveVideoToStorage(outputPath, video, mimeType, qualityType);
		}

        public static void To720Ogg(string inputPath, Video video, MimeType mimeType, QualityType qualityType)
		{
			EnsureShowingProgress();

			var convertSettings = GetOggConvertSettings("1280x720",
				"-codec:v libtheora -qscale:v 7 -codec:a libvorbis -qscale:a 5");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "720.ogg";

			Console.WriteLine("Converting to 720p OGG ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.ogg, convertSettings);

            SaveVideoToStorage(outputPath, video, mimeType, qualityType);
		}

        public static void To480Ogg(string inputPath, Video video, MimeType mimeType, QualityType qualityType)
		{
			EnsureShowingProgress();

			var convertSettings = GetOggConvertSettings("854x480",
				"-codec:v libtheora -qscale:v 7 -codec:a libvorbis -qscale:a 5");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "480.ogg";

			Console.WriteLine("Converting to 480p OGG ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.ogg, convertSettings);

            SaveVideoToStorage(outputPath, video, mimeType, qualityType);
		}

        public static void To360Ogg(string inputPath, Video video, MimeType mimeType, QualityType qualityType)
		{
			EnsureShowingProgress();

			var convertSettings = GetOggConvertSettings("640x360",
				"-codec:v libtheora -qscale:v 7 -codec:a libvorbis -qscale:a 5");
			var outputPath = Path.GetDirectoryName(inputPath) + @"\" + OutPrefix + "360.ogg";

			Console.WriteLine("Converting to 360p OGG ...");
			Converter.ConvertMedia(inputPath, null, outputPath, Format.ogg, convertSettings);

            SaveVideoToStorage(outputPath, video, mimeType, qualityType);
		}

		#endregion
	}
}