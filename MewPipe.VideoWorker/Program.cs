﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MewPipe.Logic.Models;
using MewPipe.Logic.RabbitMQ;
using MewPipe.Logic.RabbitMQ.Messages;
using MewPipe.Logic.Services;
using MewPipe.VideoWorker.Helper;
using MewPipe.VideoWorker.Properties;

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
			VideoWorkerService.MarkVideoAsPublished(video);

			Thread.Sleep(1000);
		}

		private static void DoTotalConversion(Video video)
		{
			var oVideoStream = VideoWorkerService.GetVideoUploadedFile(video);

			// Get and store the original video on the disk:
			string workFolder = Settings.Default.VideoWorkerConversionsFolder;
			string inputFilePath = workFolder + @"\input.tmp";
			StreamToFile(oVideoStream, inputFilePath);

			// Get the needed datas for the total conversion
			var encodingMimeTypes = VideoMimeTypeService.GetEncodingMimeTypes();
			var encodingQualityTypes = VideoQualityTypeService.GetEncodingQualityTypes();
			var vidQuality = VideoInfosHelper.GuessVideoQualityType(inputFilePath);
			var vidQualityResY = int.Parse(vidQuality.Name);

			foreach (var mimeType in encodingMimeTypes)
			{
				if (!mimeType.Name.Equals("MP4")) continue; // Only MP4 conversion is supported TODO: remove when ogg is supported
				foreach (var qualityType in encodingQualityTypes)
				{
					try
					{
						var qualityResY = int.Parse(qualityType.Name);
						if (qualityResY > vidQualityResY) continue; // We won't convert the vid to a higher resolution
						var mongoStream = VideoWorkerService.GetStreamToAddConvertedVideo(video, mimeType, qualityType);
						VideoConverterHelper.DoConversion(inputFilePath, mimeType, qualityType, video);
					}
					catch (Exception)
					{
						// Skip this qualityType of that mimeType if something went wrong
					}
				}
			}

            VideoWorkerService.RemoveVideoUploadedFile(video);

			Console.WriteLine("All conversions done !");
		}
	}
}