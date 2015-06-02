using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.RecommenderEngine
{
    public interface IRecommenderEngine
    {
        IEnumerable<KeyValuePair<string, double>> GetTopMatches(Dictionary<string, VideoUserRatingData[]> videos,
            string videoId, int count = 5);
    }

    //http://en.wikipedia.org/wiki/Pearson_product-moment_correlation_coefficient
    public class PearsonScoreEngine : IRecommenderEngine
    {
        private static double GetPearsonScore(IReadOnlyDictionary<string, VideoUserRatingData[]> videos, string firstVideoId, string secondVideoId)
        {
            var mutuallyRatedItems = new Dictionary<string, int>();

            /**
             * Fetching all the users that have a rating for the same video
             */
            foreach (var userRating in videos[firstVideoId].Where(userRating => videos[secondVideoId].Any(ur => ur.UserId == userRating.UserId)))
            {
                mutuallyRatedItems[userRating.UserId] = 1;
            }

            var numberOfCommonRatedItems = mutuallyRatedItems.Count;

            if (numberOfCommonRatedItems == 0)
            {
                return 0;
            }

            /*
             * Adding the ratings for each video
             * and Sum the squares for each video
             */
            double sumVideo1 = 0;
            double sumVideo1Squared = 0;
            double sumVideo2 = 0;
            double sumVideo2Squared = 0;
            double productsSum = 0;

            foreach (var item in mutuallyRatedItems)
            {
                var rating1 = videos[firstVideoId].SingleOrDefault(ur => ur.UserId == item.Key);
                var rating2 = videos[secondVideoId].SingleOrDefault(ur => ur.UserId == item.Key);
                if (rating1 != null && rating2 != null)
                {
                    sumVideo1 += rating1.Rating;
                    sumVideo1Squared += Math.Pow(rating1.Rating, 2);
                    sumVideo2 += rating2.Rating;
                    sumVideo2Squared += Math.Pow(rating2.Rating, 2);
                    productsSum += (rating1.Rating*rating2.Rating);
                }
            }

            /*
             * Calculates pearson score
             */
            var numerator = productsSum - (sumVideo1*sumVideo2/numberOfCommonRatedItems);
            var denominator =
                Math.Sqrt((sumVideo1Squared - Math.Pow(sumVideo1, 2) / numberOfCommonRatedItems) *
                          (sumVideo2Squared - Math.Pow(sumVideo2, 2) / numberOfCommonRatedItems));

            if (denominator.Equals(0))
            {
                return 0;
            }

            return numerator/denominator;
        }

        public IEnumerable<KeyValuePair<string, double>> GetTopMatches(Dictionary<string, VideoUserRatingData[]> videos, string videoId, int count = 5)
        {
            var scores = new Dictionary<string, double>();
            foreach (var videoUserRatingData in videos)
            {
                if (videoUserRatingData.Key != videoId)
                {
                    var score = GetPearsonScore(videos, videoId, videoUserRatingData.Key);
                    scores.Add(videoUserRatingData.Key, score);
                }
            }

            var scoreList = scores.ToList();
            scoreList.Sort((v1, v2) => v2.Value.CompareTo(v1.Value));
            return scoreList.Take(count);
        }
    }
}
