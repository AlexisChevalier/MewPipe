using System.Web.Mvc;

namespace MewPipe.Accounts.Controllers
{
    public class HomeController : Controller
    {
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