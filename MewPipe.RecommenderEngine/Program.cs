using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.RabbitMQ;
using MewPipe.Logic.RabbitMQ.Messages;

namespace MewPipe.RecommenderEngine
{
	public class Program
	{
		private static Thread _allVideosProcessThread;
		private static bool _closing = false;

		private static void Main(string[] args)
		{
			if (args[0] == "master")
			{
				_allVideosProcessThread = new Thread(ProcessAllRecommendations);
				_allVideosProcessThread.Start();
			}

			Run();
		}

		private static void Run()
		{
			using (var workerQueueManager = new WorkerQueueManager())
			{
				using (var channelQueueConsumer =
					workerQueueManager.GetChannelQueueConsumer(WorkerQueueManager.QueueChannelIdentifier.RecommendationsUpdates))
				{
					while (!_closing)
					{
						var recommendationsUpdateMessage =
							channelQueueConsumer.DequeueMessage<RecommendationsUpdateMessage>();

						try
						{
							var message = recommendationsUpdateMessage.GetMessageData();
							var id = ShortGuid.Decode(message.VideoId);
							ProcessOneVideoRecommendations(id);
						}
						catch (Exception e)
						{
							Trace.WriteLine(e);
						}

						channelQueueConsumer.AcknowledgeMessage(recommendationsUpdateMessage);
					}
				}
			}
		}

		private static void ProcessOneVideoRecommendations(Guid id)
		{
			var watch = Stopwatch.StartNew();
			var pearsonEngine = new PearsonScoreEngine();
			var contentEngine = new ContentScoreEngine();
			var mewPipeDataSource = new MewPipeDataSource();

			//1 - RECUPERATION
			var realData = mewPipeDataSource.GetData();

			//2 - RESULTS
			var finalArray = new KeyValuePair<string, double>[0];

			var socialResults = pearsonEngine.GetTopMatches(realData.VideoUserRatingDatas,
				realData.VideoRatingDatas.First(r => r.Value.VideoId == id.ToString()).Value);
			var contentResults = contentEngine.GetTopMatches(realData.VideoRatingDatas,
				realData.VideoRatingDatas.First(r => r.Value.VideoId == id.ToString()).Value);

			if (socialResults.Count > 0)
			{
				//finalArray = contentResults.ToArray();
				finalArray = socialResults.ToArray();
			}

			//3 - COMBINATION
			//TODO

			mewPipeDataSource.SaveData(id.ToString(), finalArray);

			watch.Stop();

			Console.Out.WriteLine("[INFO][" + DateTime.Now.ToLongDateString() + "] Recommendations for video " + id +
								" in database updated in " + watch.ElapsedMilliseconds + " ms");
		}

		private static void ProcessAllRecommendations()
		{
			while (!_closing)
			{
				var watch = Stopwatch.StartNew();
				var pearsonEngine = new PearsonScoreEngine();
				var contentEngine = new ContentScoreEngine();
				var mewPipeDataSource = new MewPipeDataSource();

				//1 - RECUPERATION
				var realData = mewPipeDataSource.GetData();

				//2 - RESULTS
				foreach (var video in realData.VideoRatingDatas)
				{
					var finalArray = new KeyValuePair<string, double>[0];

					var smallWatch = Stopwatch.StartNew();
					var socialResults = pearsonEngine.GetTopMatches(realData.VideoUserRatingDatas,
						realData.VideoRatingDatas.First(r => r.Value.VideoId == video.Key).Value);
					Console.Out.WriteLine(smallWatch.ElapsedMilliseconds + " ms elapsed for social results");
					smallWatch.Restart();
					var contentResults = contentEngine.GetTopMatches(realData.VideoRatingDatas,
						realData.VideoRatingDatas.First(r => r.Value.VideoId == video.Key).Value);
					Console.Out.WriteLine(smallWatch.ElapsedMilliseconds + " ms elapsed for content results");
					smallWatch.Restart();


					if (socialResults.Count > 0)
					{
						//finalArray = contentResults.ToArray();
						finalArray = socialResults.ToArray();
					}

					//3 - COMBINATION
					//TODO

					Console.Out.WriteLine(smallWatch.ElapsedMilliseconds + " ms elapsed for array building");
					smallWatch.Restart();
					mewPipeDataSource.SaveData(video.Key, finalArray);
					Console.Out.WriteLine(smallWatch.ElapsedMilliseconds + " ms elapsed for saving data");
					smallWatch.Restart();
				}

				watch.Stop();
				Console.Out.WriteLine("[INFO][" + DateTime.Now.ToLongDateString() + "] All Recommendations in database updated in " +
									watch.ElapsedMilliseconds + " ms");
			}
		}
	}
}