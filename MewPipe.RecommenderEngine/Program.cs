using System;
using System.Collections.Generic;
using System.ComponentModel;
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
			if (args.Any() && args[0] == "master")
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
            Console.Out.WriteLine("[INFO][" + DateTime.Now.ToLongTimeString() + "] Processing recommendations for video " + id + "...");
			var watch = Stopwatch.StartNew();
			var contentEngine = new ContentScoreEngine();
			var mewPipeDataSource = new MewPipeDataSource();

			//1 - RECUPERATION
			var realData = mewPipeDataSource.GetData();

			//2 - RESULTS
			var finalArray = new KeyValuePair<string, double>[0];

			var contentResults = contentEngine.GetTopMatches(realData.VideoRatingDatas,
				realData.VideoRatingDatas.First(r => r.Value.VideoId == id.ToString()).Value);

            if (contentResults.Count > 0)
			{
                finalArray = contentResults.ToArray();
			}

			mewPipeDataSource.SaveData(id.ToString(), finalArray);

			watch.Stop();

            Console.Out.WriteLine("[INFO][" + DateTime.Now.ToLongTimeString() + "] Recommendations for video " + id +
								" in database updated in " + watch.ElapsedMilliseconds + " ms");
		}

		private static void ProcessAllRecommendations()
        {
			while (!_closing)
            {
                Console.Out.WriteLine("[INFO][" + DateTime.Now.ToLongTimeString() + "] Processing all the recommendations...");
				var watch = Stopwatch.StartNew();
				var pearsonEngine = new PearsonScoreEngine();
				var contentEngine = new ContentScoreEngine();
				var mewPipeDataSource = new MewPipeDataSource();

				//1 - RECUPERATION
				var realData = mewPipeDataSource.GetData();

				//2 - RESULTS
				foreach (var video in realData.VideoRatingDatas)
				{
					var finalList = new List<KeyValuePair<string, double>>();

					var socialResults = pearsonEngine.GetTopMatches(realData.VideoUserRatingDatas,
						realData.VideoRatingDatas.First(r => r.Value.VideoId == video.Key).Value);
					var contentResults = contentEngine.GetTopMatches(realData.VideoRatingDatas,
						realData.VideoRatingDatas.First(r => r.Value.VideoId == video.Key).Value);

				    var empty = false;
				    var counter = 20;

                    //3 - COMBINATION
				    do
				    {
				        var nextSocial = socialResults.OrderByDescending(s => s.Value).FirstOrDefault();
				        var nextContent = contentResults.OrderByDescending(s => s.Value).FirstOrDefault();

				        if (string.IsNullOrWhiteSpace(nextSocial.Key))
				        {
                            if (finalList.All(fe => fe.Key != nextContent.Key))
                            {
                                finalList.Add(nextContent);
                                counter--;
                            }
                            contentResults.Remove(nextContent);
                            counter--;
                        }
                        else if (string.IsNullOrWhiteSpace(nextContent.Key))
                        {
                            if (finalList.All(fe => fe.Key != nextSocial.Key))
                            {
                                finalList.Add(nextSocial);
                                counter--;
                            }

                            socialResults.Remove(nextSocial);
                        }
                        else
                        {
                            if (nextSocial.Value >= nextContent.Value)
                            {
                                if (finalList.All(fe => fe.Key != nextSocial.Key))
                                {
                                    finalList.Add(nextSocial);
                                    counter--;
                                }

                                socialResults.Remove(nextSocial);
                            }
                            else
                            {
                                if (finalList.All(fe => fe.Key != nextContent.Key))
                                {
                                    finalList.Add(nextContent);
                                    counter--;
                                }
                                contentResults.Remove(nextContent);
                            }   
                        }

                        if (socialResults.Count == 0 && contentResults.Count == 0)
                        {
                            empty = true;
                        }

				    } while (counter > 0 || empty);

					mewPipeDataSource.SaveData(video.Key, finalList.OrderByDescending(v => v.Value).ToArray());
				}

				watch.Stop();
                Console.Out.WriteLine("[INFO][" + DateTime.Now.ToLongTimeString() + "] All Recommendations in database updated in " +
									watch.ElapsedMilliseconds + " ms");
			}
		}
	}
}