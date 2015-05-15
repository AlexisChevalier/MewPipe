using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MewPipe.Logic.Models;
using MewPipe.Logic.RabbitMQ;
using MewPipe.Logic.RabbitMQ.Messages;
using MewPipe.Logic.Services;
using MewPipe.VideoWorker.Helper;
using MewPipe.VideoWorker.Properties;
using MongoDB.Driver.GridFS;

namespace MewPipe.VideoWorker
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//TODO: If you need to handle the args[] array you should do it here.

			#region Testing conversions

			/*
			var inputVid = @"C:\Dropbox\Documents\projets\4PJT\Workspace\Conversions\input1080.wmv";

			VideoConverterHelper.To1080Mp4(inputVid, null);
			VideoConverterHelper.To720Mp4(inputVid, null);
			VideoConverterHelper.To480Mp4(inputVid, null);
			VideoConverterHelper.To360Mp4(inputVid, null);
			*/

			#endregion

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
			var service = new VideoWorkerService(); //TODO put it as a static field

			Video video = service.GetVideoDetails(message.VideoId);
			//DoTotalConversion(video);

			service.MarkVideoAsPublished(video);

			Thread.Sleep(1000);
		}

		private static void DoTotalConversion(Video video)
		{
			var service = new VideoWorkerService(); //TODO put it as a static field
		    var mimeTypeService = new VideoMimeTypeService();
		    var qualityTypeService = new VideoQualityTypeService();

			MongoGridFSStream oVideoStream = service.GetVideoUploadedFile(video);

			string ext = ".mp4"; //TODO Need that info in Video or in the message

			string workFolder = Settings.Default.VideoWorkerConversionsFolder;
			string inputFilePath = workFolder + @"\input" + ext;
			StreamToFile(oVideoStream, inputFilePath);

			string vidId = video.Id.ToString();

		    var encodingMimeTypes = mimeTypeService.GetEncodingMimeTypes();
		    var encodingQualityTypes = qualityTypeService.GetEncodingQualityTypes();

            /**
             * @JB : Supprime ce commentaire une fois utilisé stp
             * Tu peux itérer sur les mime types d'encoding et sur les qualités prédéfinies afin d'obtenir directement des objets
             * stockés en DB que je pourrais réutiliser aprés dans les services
             * Ca va te poser soucis sur ton code par contre, je pense qu'une factory pour récupérer le bon converter peut régler le soucis
             */
            foreach (var mimeType in encodingMimeTypes)
		    {
                foreach (var qualityType in encodingQualityTypes)
                {
                    //TODO: Do something if the video is good enough
                }
		    }


			VideoConverterHelper.To1080Mp4(inputFilePath, service.GetStreamToAddConvertedVideo(vidId, "mp4", "1080"));
			VideoConverterHelper.To720Mp4(inputFilePath, service.GetStreamToAddConvertedVideo(vidId, "mp4", "720"));
			VideoConverterHelper.To480Mp4(inputFilePath, service.GetStreamToAddConvertedVideo(vidId, "mp4", "480"));
			VideoConverterHelper.To360Mp4(inputFilePath, service.GetStreamToAddConvertedVideo(vidId, "mp4", "360"));

			Console.WriteLine("All conversions done !");
		}
	}
}