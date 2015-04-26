using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MewPipe.ApiClient;
using MewPipe.Logic.Contracts;
using MewPipe.Website.Security;

namespace MewPipe.Website.Controllers
{
    public class VideosController : Controller
    {
        readonly MewPipeApiClient _apiClient = new MewPipeApiClient(
                ConfigurationManager.AppSettings["APIEndpoint"],
                ConfigurationManager.AppSettings["OAuth2ClientID"],
                ConfigurationManager.AppSettings["OAuth2ClientSecret"]);

        public ActionResult Index(string videoId)
        {
            ViewBag.VideoId = videoId;
            return View();
        }

        [HttpGet]
        public ActionResult UserVideos()
        {
            return View();
        }

        public async Task<ActionResult> UploadVideo()
        {
            _apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);
            var uploadToken = await _apiClient.RequestVideoUploadToken(new VideoUploadTokenRequestContract("REDIRECT_URI", "HOOK_URI"));
            ViewBag.Token = uploadToken;
            return View();
        }

        public ActionResult EditVideo(string videoId)
        {
            return View();
        }
    }
}