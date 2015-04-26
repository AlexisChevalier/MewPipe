using System.Linq;
using System.Web.Mvc;
using MewPipe.Logic.Repositories;

namespace MewPipe.Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            var unitOfWork = new UnitOfWork();

            ViewBag.Videos = unitOfWork.VideoRepository.Get().ToList();

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }
}