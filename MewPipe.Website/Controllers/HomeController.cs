using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MewPipe.Website.Extensions;

namespace MewPipe.Website.Controllers
{
    public class HomeController : Controller
    {
        [Route("", Name = "Home")]
        public ActionResult Index()
        {
            return View();
        }
    }
}