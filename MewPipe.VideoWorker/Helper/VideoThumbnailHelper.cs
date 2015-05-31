using NReco.VideoConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MewPipe.VideoWorker.Helper;

namespace MewPipe.VideoWorker.Helper
{
    public static class VideoThumbnailHelper
    {
        private static readonly FFMpegConverter Converter = new FFMpegConverter();

        public static void GetVideoThumbnail(string inputPath, string outputThumb)
        {
            // on récupère le nombre de frames de la vidéo
            int framesCount = VideoInfosHelper.GetFramesCount(inputPath);

            // on choisit une frame à 25% du total de frames, puis conversion en float
            double d = Math.Floor((double)framesCount / 4);
            //float frame = (float)d;
            int frame = 1;

            Converter.GetVideoThumbnail(inputPath, outputThumb, frame);
        }
    }
}
