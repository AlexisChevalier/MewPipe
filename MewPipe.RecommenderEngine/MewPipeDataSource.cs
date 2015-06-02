using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;

namespace MewPipe.RecommenderEngine
{
    public interface IDataSource
    {
        Dictionary<string, VideoUserRatingData[]> GetData();
    }

    public class MewPipeDataSource : IDataSource
    {
        public Dictionary<string, VideoUserRatingData[]> GetData()
        {
            var data = new Dictionary<string, VideoUserRatingData[]>();

            var uow = new UnitOfWork();

            var videoDetails = uow.VideoRepository.Get(null, null, "Impressions");

            foreach (var videoDetail in videoDetails)
            {
                var videoId = videoDetail.Id.ToString();
                data[videoId] = new VideoUserRatingData[videoDetail.Impressions.Count];
                var count = 0;
                foreach (var impression in videoDetail.Impressions)
                {
                    var ratingData = new VideoUserRatingData
                    {
                        UserId = impression.User.Id
                    };

                    if (impression.Type == Impression.ImpressionType.Bad)
                    {
                        ratingData.Rating = -1;
                    }
                    else
                    {
                        ratingData.Rating = +1;
                    }


                    data[videoId][count] = ratingData;
                    count++;
                }
            }

            return data;
        }
    }
}
