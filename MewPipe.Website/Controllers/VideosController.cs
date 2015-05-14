using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using MewPipe.ApiClient;
using MewPipe.Logic.Contracts;
using MewPipe.Website.Security;
using MewPipe.Website.ViewModels;

namespace MewPipe.Website.Controllers
{
	public class VideosController : Controller
	{
		private readonly MewPipeApiClient _apiClient = new MewPipeApiClient(
			ConfigurationManager.AppSettings["APIEndpoint"],
			ConfigurationManager.AppSettings["OAuth2ClientID"],
			ConfigurationManager.AppSettings["OAuth2ClientSecret"]);

		public ActionResult Index(string videoId)
		{
			ViewBag.VideoId = videoId;
			return View();
		}

        [SiteAuthorize]
		public ActionResult UserVideos()
		{
			return View();
		}

        [SiteAuthorize]
		public async Task<ActionResult> UploadVideo()
		{
			_apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);
			var uploadToken =
				await
					_apiClient.RequestVideoUploadToken(
						new VideoUploadTokenRequestContract(
							Url.Action("EditVideo", "Videos", new {videoId = ""}, Request.Url.Scheme) + "/", "HOOK_URI"));

			ViewBag.Token = uploadToken;
			return View();
		}

        [SiteAuthorize]
		public async Task<ActionResult> EditVideo(string videoId)
		{
			_apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);
			ViewBag.VideoDetails = await _apiClient.GetVideoDetails(videoId);
			return View();
		}

		[HttpPost]
        [SiteAuthorize]
		public async Task<ActionResult> EditVideo(ValidateUploadedVideoViewModel viewModel)
		{
			return View();
		}
	}
}