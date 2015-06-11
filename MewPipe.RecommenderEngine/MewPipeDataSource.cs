using System;
using System.Collections.Generic;
using System.Linq;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;

namespace MewPipe.RecommenderEngine
{
	public class MewPipeDataSource : IDataSource
	{
		public DataSourceResult GetData()
		{
			var data = new DataSourceResult
			{
				VideoUserRatingDatas = new Dictionary<string, VideoUserRatingData[]>(),
				VideoRatingDatas = new Dictionary<string, VideoRatingData>()
			};

			var uow = new UnitOfWork();

			var videoDetails = uow.VideoRepository.Get(null, null, "Impressions, Impressions.User, Category, Tags, User");

			foreach (var videoDetail in videoDetails)
			{
				var videoId = videoDetail.Id.ToString();
				if (videoDetail.Impressions.Count > 0)
				{
					data.VideoUserRatingDatas[videoId] = new VideoUserRatingData[videoDetail.Impressions.Count];
					var count = 0;
					foreach (var impression in videoDetail.Impressions)
					{
						var ratingData = new VideoUserRatingData
						{
							UserId = impression.User.Id.ToLower()
						};

						if (impression.Type == Impression.ImpressionType.Bad)
						{
							ratingData.SocialRating = -1;
						}
						else
						{
							ratingData.SocialRating = +1;
						}

						data.VideoUserRatingDatas[videoId][count] = ratingData;
						count++;
					}
				}

				data.VideoRatingDatas[videoId] = new VideoRatingData
				{
					Category = videoDetail.Category.Name.ToLower(),
					Description = videoDetail.Description == null ? "" : videoDetail.Description.ToLower(),
					Title = videoDetail.Name.ToLower(),
					Tags = videoDetail.Tags.Select(t => t.Name.ToLower()).ToArray(),
					VideoId = videoId,
					UploaderId = videoDetail.User.Id
				};
			}

			return data;
		}

		public void SaveData(string videoId, KeyValuePair<string, double>[] results)
		{
		    try
		    {
                var uow = new UnitOfWork();

                uow.GetContext().Database.ExecuteSqlCommand(string.Format("DELETE FROM Recommendations WHERE Video_Id1 ='{0}'", videoId));
                uow.Save();

                var videoGuid = Guid.Parse(videoId);

                var video = uow.VideoRepository.GetOne(v => v.Id == videoGuid, "Recommendations");

                var temporaryData = results.ToDictionary(result => Guid.Parse(result.Key), result => result.Value);

                var recommendedVideos = uow.VideoRepository.Get(v => temporaryData.Keys.Contains(v.Id))
                    .ToDictionary(v => v, v => temporaryData[v.Id]);

                video.Recommendations = new List<Recommendation>();
                foreach (var recommendedVideo in recommendedVideos)
                {
                    video.Recommendations.Add(new Recommendation
                    {
                        Score = recommendedVideo.Value,
                        Video = recommendedVideo.Key
                    });
                }
                uow.VideoRepository.Update(video);
                uow.Save();
            }
		    catch (Exception)
		    {
		        // ignored
		    }
		}
	}

	public class DataSourceResult
	{
		public Dictionary<string, VideoUserRatingData[]> VideoUserRatingDatas { get; set; }
		public Dictionary<string, VideoRatingData> VideoRatingDatas { get; set; }
	}
}