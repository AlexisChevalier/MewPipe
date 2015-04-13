using System.Web;
using System.Web.Mvc;

namespace MewPipe.Website.Controllers
{
    public class VideosController : Controller
    {
        public ActionResult Index(string videoId)
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserVideos()
        {
            return View();
        }

        public ActionResult UploadVideo()
        {
            return View();
        }

        public ActionResult EditVideo(string videoId)
        {
            return View();
        }
    }
}