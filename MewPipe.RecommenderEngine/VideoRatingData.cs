using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.RecommenderEngine
{
    public class VideoRatingData
    {
        public string VideoId { get; set; }
        public VideoUserRatingData[] VideoUserRatingDatas { get; set; }
    }
}
