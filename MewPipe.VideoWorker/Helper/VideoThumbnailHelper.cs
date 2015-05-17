using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MewPipe.VideoWorker.Helper
{
    public static class VideoThumbnailHelper
    {
        private static readonly FFMpegConverter Converter = new FFMpegConverter();

        public static void GetVideoThumbnail(string inputPath, string outputThumb)
        {
            float? frame = 30;
            Converter.GetVideoThumbnail(inputPath, outputThumb, frame);
        }
    }
}
