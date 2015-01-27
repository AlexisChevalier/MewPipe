using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MewPipe.Website.Controllers
{
    public class VideosController : Controller
    {
        /**
         * Video player page
         */
        [Route("v/{videoId}", Name = "ViewVideo")]
        public ActionResult Index(string videoId)
        {
            return View();
        }
    }
}