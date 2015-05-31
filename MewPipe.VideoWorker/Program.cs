﻿using System;
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

namespace MewPipe.VideoWorker
{
	internal class Program
	{
		private static readonly VideoWorkerService VideoWorkerService = new VideoWorkerService();
		private static readonly VideoMimeTypeService VideoMimeTypeService = new VideoMimeTypeService();
		private static readonly VideoQualityTypeService VideoQualityTypeService = new VideoQualityTypeService();

		private static void Main(string[] args)
		{
            Run();
		}

		private static void Run()
		{
			using (var workerQueueManager = new WorkerQueueManager())
			{
				using (var channelQueueConsumer =
					workerQueueManager.GetChannelQueueConsumer(WorkerQueueManager.QueueChannelIdentifier.NewVideos))
				{
					while (true)
					{
						var newVideoMessage = channelQueueConsumer.DequeueMessage<NewVideoMessage>();

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
			using (var fileStream = File.OpenWrite(filePath))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(fileStream, 255*1024);
			}
		}

		private static void HandleMessage(NewVideoMessage message)
		{
			var video = VideoWorkerService.GetVideoDetails(message.VideoId);
			DoTotalConversion(video);
			VideoWorkerService.MarkVideoAsPublished(video, 0);

			Thread.Sleep(1000);
		}

		private static void DoTotalConversion(Video video)
		{
			var oVideoStream = VideoWorkerService.GetVideoUploadedFile(video);

			// Get and store the original video on the disk:
			string workFolder = ConfigurationManager.ConnectionStrings["MewPipeVideoWorkerConversionsFolder"].ConnectionString;
			string inputFilePath = workFolder + @"\input.tmp";
			StreamToFile(oVideoStream, inputFilePath);

            string thumbnailPath = workFolder + @"\thumbnail.jpg";
            VideoThumbnailHelper.GetVideoThumbnail(inputFilePath, thumbnailPath);

            using (var fileStream = File.OpenRead(inputFilePath))
            {
                VideoWorkerService.AddThumbnail(video, fileStream); 
            }

			// Get the needed datas for the total conversion
			var encodingMimeTypes = VideoMimeTypeService.GetEncodingMimeTypes();
			var encodingQualityTypes = VideoQualityTypeService.GetEncodingQualityTypes();
			var vidQuality = VideoInfosHelper.GuessVideoQualityType(inputFilePath);
			var vidQualityResY = int.Parse(vidQuality.Name);

			var timeWatcher = Stopwatch.StartNew();

			var tasks = new List<Task>();
			foreach (var mimeType in encodingMimeTypes)
			{
				foreach (var qualityType in encodingQualityTypes)
				{
					try
					{
						var qualityResY = int.Parse(qualityType.Name);
						if (qualityResY > vidQualityResY) continue; // We won't convert the vid to a higher resolution

						// Preventing undesired behaviours: (http://stackoverflow.com/a/8127421/2193438)
						var mType = mimeType;
						var qType = qualityType;
						// Creating the task
						var t = Task.Factory.StartNew(() => VideoConverterHelper.DoConversion(inputFilePath, mType, qType, video));
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

			timeWatcher.Stop();
			var elapsedS = timeWatcher.ElapsedMilliseconds/1000;
			Console.WriteLine("All conversions done in " + elapsedS + " secs !");
		}
	}
}