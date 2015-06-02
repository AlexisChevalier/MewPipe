using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MewPipe.Logic.Models;
using MewPipe.Logic.RabbitMQ;
using MewPipe.Logic.RabbitMQ.Messages;
using MewPipe.Logic.Services;
using MewPipe.VideoWorker.Helper;
using MongoDB.Driver.GridFS;

namespace MewPipe.VideoWorker
{
	internal class Program
	{
		private static readonly VideoWorkerService VideoWorkerService = new VideoWorkerService();
		private static readonly VideoMimeTypeService VideoMimeTypeService = new VideoMimeTypeService();
		private static readonly VideoQualityTypeService VideoQualityTypeService = new VideoQualityTypeService();

		private static string _workFolder;


		private static void Main(string[] args)
		{
			_workFolder = ConfigurationManager.ConnectionStrings["MewPipeVideoWorkerConversionsFolder"].ConnectionString;
			Run();
		}

		private static void Run()
		{
			using (var workerQueueManager = new WorkerQueueManager())
			{
				using (IChannelQueueConsumer channelQueueConsumer =
					workerQueueManager.GetChannelQueueConsumer(WorkerQueueManager.QueueChannelIdentifier.NewVideos))
				{
					while (true)
					{
						QueueMessage<NewVideoMessage> newVideoMessage = channelQueueConsumer.DequeueMessage<NewVideoMessage>();

						try
						{
							HandleMessage(newVideoMessage.GetMessageData());

							Trace.WriteLine(String.Format("[INFO] Video {0} successfully encoded - DeliveryTag {1}",
								newVideoMessage.GetMessageData().VideoId, newVideoMessage.GetBasicDeliverEventArgs().DeliveryTag));
						}
						catch (InvalidFileException e)
						{
							//TODO: Should we clean the database ?
							//TODO: Find a way to notify the error.
							Trace.WriteLine(String.Format("[ERROR] Video {0}'s encoding failed - Invalid file - DeliveryTag {1}",
								newVideoMessage.GetMessageData().VideoId, newVideoMessage.GetBasicDeliverEventArgs().DeliveryTag));
						}
						catch (Exception e)
						{
							//TODO: Should we clean the database ?
							//TODO: Should we handle another exception (CorruptedFileException, BadFileException, etc... ?)
							Trace.WriteLine(e);
						}

						channelQueueConsumer.AcknowledgeMessage(newVideoMessage);
					}
				}
			}
		}

		private static void StreamToFile(Stream stream, string filePath)
		{
			using (FileStream fileStream = File.OpenWrite(filePath))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(fileStream, 255*1024);
			}
		}

		private static void HandleMessage(NewVideoMessage message)
		{
			Video video = VideoWorkerService.GetVideoDetails(message.VideoId);
			DoTotalConversion(video);

			Thread.Sleep(1000);
		}

		private static void DoTotalConversion(Video video)
		{
			// Create the tmp work folder for that video
			string tmpWorkFolder = _workFolder + @"\" + video.Id;
			Directory.CreateDirectory(tmpWorkFolder);
			Trace.WriteLine(
				String.Format("[INFO] The temporary work folder \"{0}\" has been created to do conversions in it.",
					video.Id));

			MongoGridFSStream oVideoStream = VideoWorkerService.GetVideoUploadedFile(video);

			// Get and store the original video on the disk:
			string inputFilePath = tmpWorkFolder + @"\input.tmp";
			StreamToFile(oVideoStream, inputFilePath);

			// Get and store the thumbnail in mongo
			Trace.WriteLine(
				String.Format("[INFO] Getting the thumbnail of the video id {0} ...",
					video.Id));
			string thumbnailPath = tmpWorkFolder + @"\thumbnail.jpg";
			VideoThumbnailHelper.GetVideoThumbnail(inputFilePath, thumbnailPath);

			using (FileStream fileStream = File.OpenRead(thumbnailPath))
			{
				VideoWorkerService.AddThumbnail(video, fileStream);
			}
			Trace.WriteLine(
				String.Format("[SUCCESS] Successfully got the thumbnail of the video id {0}.",
					video.Id));

			// Get the needed datas for the total conversion
			MimeType[] encodingMimeTypes = VideoMimeTypeService.GetEncodingMimeTypes();
			QualityType[] encodingQualityTypes = VideoQualityTypeService.GetEncodingQualityTypes();
			QualityType vidQuality = VideoInfosHelper.GuessVideoQualityType(inputFilePath);
			int vidQualityResY = int.Parse(vidQuality.Name);

			Trace.WriteLine(
				String.Format("[INFO] Processing video id {0} for total conversion ...",
					video.Id));

			Stopwatch timeWatcher = Stopwatch.StartNew();

			var tasks = new List<Task>();
			foreach (MimeType mimeType in encodingMimeTypes)
			{
				foreach (QualityType qualityType in encodingQualityTypes)
				{
					try
					{
						int qualityResY = int.Parse(qualityType.Name);
						if (qualityResY > vidQualityResY) continue; // We won't convert the vid to a higher resolution

						// Preventing undesired behaviours: (http://stackoverflow.com/a/8127421/2193438)
						MimeType mType = mimeType;
						QualityType qType = qualityType;
						// Creating the task
						Task t = Task.Factory.StartNew(() => VideoConverterHelper.DoConversion(inputFilePath, mType, qType, video));
						tasks.Add(t);
					}
					catch (Exception)
					{
						// Skip this qualityType of that mimeType if something went wrong
					}
				}
			}

			// Wait for the tasks to complete
			Task.WaitAll(tasks.ToArray());

			VideoWorkerService.RemoveVideoUploadedFile(video);

			var duration = VideoInfosHelper.GetVideoDuration(inputFilePath)/1000;
			VideoWorkerService.MarkVideoAsPublished(video, (long) duration); // Loosing milliseconds precision

			timeWatcher.Stop();
			long elapsedS = timeWatcher.ElapsedMilliseconds/1000;
			Trace.WriteLine(
				String.Format("[SUCCESS] Successfully processed video id {0} for total conversion ({1} secs)",
					video.Id, elapsedS));
#if !DEBUG
			Directory.Delete(tmpWorkFolder, true); // Delete the tmp work folder
			Trace.WriteLine(
				String.Format("[INFO] The temporary work folder \"{0}\" has been deleted.",
					video.Id));
#else
			Trace.WriteLine(
				String.Format("[WARNING] The temporary work folder \"{0}\" is not deleted because we are running in Debug mod.",
					video.Id));
#endif
		}
	}
}