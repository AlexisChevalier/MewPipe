using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MewPipe.ApiClient;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Repositories;
using MewPipe.Website.Security;

namespace MewPipe.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly MewPipeApiClient _apiClient = new MewPipeApiClient(
            ConfigurationManager.AppSettings["APIEndpoint"],
            ConfigurationManager.AppSettings["OAuth2ClientID"],
            ConfigurationManager.AppSettings["OAuth2ClientSecret"]);

        public async Task<ActionResult> Index()
        {
            VideoContract[] trends;
            if (HttpContext.GetIdentity().IsAuthenticated())
            {
                trends = await OauthHelper.TryAuthenticatedMethod(_apiClient,
                    Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                    () => _apiClient.GetTrends());
            }
            else
            {
                trends = await _apiClient.GetTrends();
            }

            //TODO: TEMP
            ViewBag.Videos = trends;

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