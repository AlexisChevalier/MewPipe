using System.Configuration;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.InfoCard;
using MewPipe.ApiClient;
using MewPipe.Logic.Contracts;
using MewPipe.Website.Security;
using MewPipe.Website.ViewModels;

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

        public ActionResult UserVideos()
        {
            return View();
        }

        public async Task<ActionResult> UploadVideo()
        {
            _apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);
                var uploadToken =
                    await
                        _apiClient.RequestVideoUploadToken(
                            new VideoUploadTokenRequestContract(
                                Url.Action("EditVideo", "Videos", null, Request.Url.Scheme) + "/", "HOOK_URI"));

            ViewBag.Token = uploadToken;
            return View();
        }

        

        public async Task<ActionResult> EditVideo(string videoId)
        {
            _apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);
            ViewBag.VideoDetails = await _apiClient.GetVideoDetails(videoId);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EditVideo(ValidateUploadedVideoViewModel viewModel)
        {
            return View();
        }
    }
}