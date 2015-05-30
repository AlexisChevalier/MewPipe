using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace MewPipe.VideosRepository
{
    public static class ImagePlaceholderStore
    {
        private static byte[] _imageBytes;

        public static void Initialize()
        {
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(HttpContext.Current.Server.MapPath("~/Images/videoPlaceholder.jpg"));
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                _imageBytes = bytes;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        public static Stream GetImageStream()
        {
            Stream stream = new MemoryStream(_imageBytes);
            return stream;
        }
    }
}