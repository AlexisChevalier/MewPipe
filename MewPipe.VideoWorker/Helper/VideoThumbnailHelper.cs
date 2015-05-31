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
            double miliSecondsCount = VideoInfosHelper.GetVideoDuration(inputPath);

            double seconds = miliSecondsCount / 1000;

            // on choisit une seconde à 25% du total de la video, puis conversion en float
            double d = Math.Floor((double)seconds / 4);
            float second = (float)d;

            Converter.GetVideoThumbnail(inputPath, outputThumb, second);
        }
    }
}
