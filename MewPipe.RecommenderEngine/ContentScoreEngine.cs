using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.RecommenderEngine
{
    public class ContentScoreEngine : IRecommenderEngine<VideoRatingData>

{
        public List<KeyValuePair<string, double>> GetTopMatches(Dictionary<string, VideoRatingData> videos, VideoRatingData currentVideo, int count = 20)
        {
            var contentScores = videos.Where(videoRatingData => videoRatingData.Value.VideoId != currentVideo.VideoId && !videoRatingData.Value.NotIndexed).ToDictionary(videoRatingData => videoRatingData.Value.VideoId, videoRatingData => videoRatingData.Value.GetContentScore(currentVideo));

            var results = contentScores.ToList();
            results.Sort((v1, v2) => v2.Value.CompareTo(v1.Value));
            return results.Take(count).ToList();
        }
    }
}
