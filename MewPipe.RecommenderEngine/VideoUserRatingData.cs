using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.RecommenderEngine
{
    public class VideoUserRatingData
    {
        public bool NotIndexed { get; set; }
        public string UserId { get; set; }
        public double SocialRating { get; set; }

    }
}
