using System;
using System.Diagnostics;
using System.Threading;
using MewPipe.Logic.RabbitMQ;
using MewPipe.Logic.RabbitMQ.Messages;
using MewPipe.Logic.Services;

namespace MewPipe.VideoWorker
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//TODO: If you need to handle the args[] array you should do it here.

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

		private static void HandleMessage(NewVideoMessage message)
		{
			var service = new VideoWorkerService();

			var video = service.GetVideoDetails(message.VideoId);

			//TODO: Call your main function here

			service.MarkVideoAsPublished(video);

			Thread.Sleep(1000);
		}
	}
}