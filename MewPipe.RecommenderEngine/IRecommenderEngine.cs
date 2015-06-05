using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.RecommenderEngine
{
    public interface IRecommenderEngine<T>
    {
        List<KeyValuePair<string, double>> GetTopMatches(Dictionary<string, T> videos,
            VideoRatingData currentVideo, int count = 20);
    }
}
