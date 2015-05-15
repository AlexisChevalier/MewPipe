using System;
using System.Globalization;
using System.IO;
using System.Text;
using NReco.VideoConverter;

namespace MewPipe.VideoWorker.Helper
{
	public static class NRecoMethodExtensions
	{
		public static void PrintFFMpegParams(this FFMpegConverter inst, string inputFormat, string outputFormat,
			ConvertSettings settings)
		{
			string str = Path.Combine(inst.FFMpegToolPath, inst.FFMpegExeName);
			if (!File.Exists(str))
				throw new FileNotFoundException("Cannot find ffmpeg tool: " + str);

			string ffMpegArgs = new CheatedFFMpegConverter().ComposeFFMpegCommandLineArgs("-", inputFormat, "-", outputFormat,
				settings);

			Console.WriteLine(ffMpegArgs);
		}
	}

	public class CheatedFFMpegConverter : FFMpegConverter
	{
		public void ComposeFFMpegOutputArgs(StringBuilder outputArgs, string outputFormat, OutputSettings settings)
		{
			if (settings == null)
				return;
			if (settings.MaxDuration.HasValue)
				outputArgs.AppendFormat(CultureInfo.InvariantCulture, " -t {0}", new object[]
				{
					settings.MaxDuration
				});
			if (outputFormat != null)
				outputArgs.AppendFormat(" -f {0} ", outputFormat);
			if (settings.AudioSampleRate.HasValue)
				outputArgs.AppendFormat(" -ar {0}", settings.AudioSampleRate);
			if (settings.AudioCodec != null)
				outputArgs.AppendFormat(" -acodec {0}", settings.AudioCodec);
			if (settings.VideoFrameCount.HasValue)
				outputArgs.AppendFormat(" -vframes {0}", settings.VideoFrameCount);
			if (settings.VideoFrameRate.HasValue)
				outputArgs.AppendFormat(" -r {0}", settings.VideoFrameRate);
			if (settings.VideoCodec != null)
				outputArgs.AppendFormat(" -vcodec {0}", settings.VideoCodec);
			if (settings.VideoFrameSize != null)
				outputArgs.AppendFormat(" -s {0}", settings.VideoFrameSize);
			if (settings.CustomOutputArgs == null)
				return;
			outputArgs.AppendFormat(" {0} ", settings.CustomOutputArgs);
		}

		public new string ComposeFFMpegCommandLineArgs(string inputFile, string inputFormat, string outputFile,
			string outputFormat, ConvertSettings settings)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (settings.AppendSilentAudioStream)
				stringBuilder.Append(" -f lavfi -i aevalsrc=0 ");
			if (settings.Seek.HasValue)
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " -ss {0}", new object[]
				{
					settings.Seek
				});
			if (inputFormat != null)
				stringBuilder.Append(" -f " + inputFormat);
			if (settings.CustomInputArgs != null)
				stringBuilder.AppendFormat(" {0} ", settings.CustomInputArgs);
			StringBuilder outputArgs = new StringBuilder();
			ComposeFFMpegOutputArgs(outputArgs, outputFormat, settings);
			if (settings.AppendSilentAudioStream)
				outputArgs.Append(" -shortest ");
			return string.Format("-y -loglevel info {0} -i {1} {2} {3}", (object) stringBuilder.ToString(),
				(object) CommandArgParameter(inputFile), (object) outputArgs.ToString(), (object) CommandArgParameter(outputFile));
		}
	}
}