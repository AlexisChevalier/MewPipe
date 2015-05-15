using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using MewPipe.ApiClient;
using MewPipe.Logic.Contracts;
using MewPipe.Website.Extensions;
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
        public async Task<ActionResult> UserVideos()
        {
            _apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);
            var videos = await _apiClient.GetUserVideos();

            ViewBag.videos = videos;

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

            try
            {
                var video = await _apiClient.GetVideoDetails(videoId);

                if (video == null)
                {
                    return RedirectToAction("UserVideos").Error("Can't find a video with this ID");
                }

                ViewBag.VideoDetails = video;

                var viewModel = new EditVideoViewModel
                {
                    Description = video.Description,
                    Name = video.Name,
                    PrivacyStatus = video.PrivacyStatus,
                    PublicId = video.PublicId
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                return RedirectToAction("UserVideos").Error("Can't find a video with this ID");
            }
		}

		[HttpPost]
        [SiteAuthorize]
        public async Task<ActionResult> EditVideo(EditVideoViewModel viewModel)
		{
		    if (!ModelState.IsValid)
		    {
		        return View(viewModel);
		    }

			_apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);

            try
            {
                var video = await _apiClient.GetVideoDetails(viewModel.PublicId);

                if (video == null)
                {
                    return RedirectToAction("UserVideos").Error("Can't find a video with this ID");
                }

                return RedirectToAction("UserVideos").Success("Video successfully updated !");
            }
            catch (Exception e)
            {
                return RedirectToAction("UserVideos").Error("Can't find a video with this ID");
            }
		}
	}
}