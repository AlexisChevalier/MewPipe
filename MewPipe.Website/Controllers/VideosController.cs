using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MewPipe.ApiClient;
using MewPipe.Logic;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;
using MewPipe.Logic.Services;
using MewPipe.Website.Extensions;
using MewPipe.Website.Security;
using MewPipe.Website.ViewModels;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

namespace MewPipe.Website.Controllers
{
	public class VideosController : Controller
	{
		private readonly MewPipeApiClient _apiClient = new MewPipeApiClient(
			ConfigurationManager.AppSettings["APIEndpoint"],
			ConfigurationManager.AppSettings["OAuth2ClientID"],
			ConfigurationManager.AppSettings["OAuth2ClientSecret"]);

		public async Task<ActionResult> Index(string videoId)
		{
		    VideoContract video = null;
		    try
		    {
                if (HttpContext.GetIdentity().IsAuthenticated())
		        {
                    video = await OauthHelper.TryAuthenticatedMethod(_apiClient, 
                        Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                        () => _apiClient.GetVideoDetails(videoId));
		        }
		        else
                {
                    video = await _apiClient.GetVideoDetails(videoId);
		        }
		    }
		    catch (HttpException e)
		    {
		        if (e.GetHttpCode() == (int) HttpStatusCode.Unauthorized)
		        {
		            ViewBag.Unauthorized = true;
		        }
		        else
		        {
                    ViewBag.NotFound = true;
                }
                return View();
		    }
		    catch (Exception e)
		    {
                ViewBag.NotFound = true;
                return View();
		    }

		    if (video == null)
		    {
                ViewBag.NotFound = true;
                return View();
		    }

            var videoService = new VideoApiService();

            var updatedVideo = videoService.AddView(video.PublicId);
            video.Views = updatedVideo.Views;

            ViewBag.VideoDetails = video;
            ViewBag.JsonVideoDetails = JsonConvert.SerializeObject(video);

			return View();
		}

        public async Task<ActionResult> Search(SearchViewModel viewModel)
        {
            ViewBag.HideSearchBar = true;

            if (viewModel.Term == null)
            {
                ViewBag.Results = new SearchContract();
                ViewBag.NoSearch = true;
            }
            else
            {
                var videos = await _apiClient.SearchVideos(viewModel.Term, viewModel.OrderCriteria, viewModel.OrderDesc, viewModel.Page, 20);
                ViewBag.Results = videos;
                ViewBag.NoSearch = false;
            }

            return View(viewModel);
        }

        [SiteAuthorize]
        public async Task<ActionResult> UserVideos()
        {
            var videos = await OauthHelper.TryAuthenticatedMethod(_apiClient, 
                Request.GetIdentity().AccessToken.ToAccessTokenContract(), 
                () => _apiClient.GetUserVideos());

            ViewBag.videos = videos;

            return View();
		}

        [SiteAuthorize]
		public async Task<ActionResult> UploadVideo()
		{
            var uploadToken = await OauthHelper.TryAuthenticatedMethod(_apiClient,
                Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                () => _apiClient.RequestVideoUploadToken(
						new VideoUploadTokenRequestContract(
							Url.Action("EditVideo", "Videos", new {videoId = ""}, Request.Url.Scheme) + "/", "HOOK_URI")));

			ViewBag.Token = uploadToken;
			return View();
		}

        [SiteAuthorize]
		public async Task<ActionResult> EditVideo(string videoId)
		{
			_apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);

            try
            {
                var video = await OauthHelper.TryAuthenticatedMethod(_apiClient,
                    Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                    () => _apiClient.GetVideoDetails(videoId));

                var categories = await OauthHelper.TryAuthenticatedMethod(_apiClient,
                    Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                    () => _apiClient.GetVideoCategories());

                if (video == null)
                {
                    return RedirectToAction("UserVideos").Error("Can't find a video with this ID");
                }

                if (video.PrivacyStatus == Video.PrivacyStatusTypes.Private)
                {
                    var whiteList = await _apiClient.GetVideoWhiteList(videoId);
                    ViewBag.WhiteList = whiteList;
                }

                ViewBag.VideoDetails = video;

                var viewModel = new EditVideoViewModel
                {
                    Description = video.Description,
                    Name = video.Name,
                    PrivacyStatus = video.PrivacyStatus,
                    PublicId = video.PublicId,
                    CategoryId = video.Category.Id,
                    CategoryList = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id,
                        Text = c.Name
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                if (e is OauthExpiredTokenException) throw;
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
                await OauthHelper.TryAuthenticatedMethod(_apiClient,
                    Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                    () => _apiClient.UpdateVideo(viewModel.PublicId, new VideoUpdateContract
                        {
                            Description = viewModel.Description,
                            Name = viewModel.Name,
                            PrivacyStatus = viewModel.PrivacyStatus,
                            CategoryId = viewModel.CategoryId
                        }));

                return RedirectToAction("EditVideo", new {videoId = viewModel.PublicId}).Success("Video successfully updated !");
            }
            catch (Exception e)
            {
                if (e is OauthExpiredTokenException) throw;
                return RedirectToAction("EditVideo", new {videoId = viewModel.PublicId}).Error("There was an error updating your video.");
            }
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize]
        public async Task<ActionResult> DeleteVideo(DeleteVideoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UserVideos", "Videos").Error("Invalid request");
            }

            _apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);

            try
            {
                await OauthHelper.TryAuthenticatedMethod(_apiClient,
                    Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                    () => _apiClient.DeleteVideo(viewModel.PublicId));

                return RedirectToAction("UserVideos").Success("Video successfully deleted !");
            }
            catch (Exception e)
            {
                if (e is OauthExpiredTokenException) throw;
                return RedirectToAction("UserVideos").Error("There was an error deleting your video.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SiteAuthorize]
        public async Task<ActionResult> AddUserToVideoWhitelist(AddUserToVideoWhiteListViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UserVideos", "Videos").Error("Invalid request");
            }

            _apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);

            try
            {
                await OauthHelper.TryAuthenticatedMethod(_apiClient,
                    Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                    () =>_apiClient.AddUserToWhiteList(viewModel.PublicId, viewModel.UserEmail));

                return RedirectToAction("EditVideo", new { videoId = viewModel.PublicId }).Success("User successfully added to the white list !");
            }
            catch (Exception e)
            {
                if (e is OauthExpiredTokenException) throw;
                return RedirectToAction("EditVideo", new { videoId = viewModel.PublicId }).Error("Can't add this user to the whitelist. Maybe he doesn't exists.");
            }
        }

        [HttpGet]
        [SiteAuthorize]
        public async Task<ActionResult> RemoveUserFromVideoWhitelist(string videoId, string userId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UserVideos", "Videos").Error("Invalid request");
            }

            _apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);

            try
            {
                await OauthHelper.TryAuthenticatedMethod(_apiClient,
                    Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                    () => _apiClient.RemoveUserFromWhiteList(videoId, userId));

                return RedirectToAction("EditVideo", new { videoId = videoId }).Success("Video successfully deleted !");
            }
            catch (Exception e)
            {
                if (e is OauthExpiredTokenException) throw;
                return RedirectToAction("EditVideo", new { videoId = videoId }).Error("There was an error deleting your video.");
            }
        }

        [HttpPost]
        [SiteAuthorize(ReturnJsonError = true)]
        public async Task<ActionResult> SetVideoImpression(string videoId, bool good)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UserVideos", "Videos").Error("Invalid request");
            }

            _apiClient.SetBearerToken(Request.GetIdentity().AccessToken.access_token);

            try
            {
                var video = await OauthHelper.TryAuthenticatedMethod(_apiClient,
                    Request.GetIdentity().AccessToken.ToAccessTokenContract(),
                    () => _apiClient.SetVideoImpression(new ImpressionContract
                    {
                        PublicVideoId = videoId,
                        Type = good ? Impression.ImpressionType.Good : Impression.ImpressionType.Bad,
                        UserId = Request.GetIdentity().User.Id
                    }));

                return new JsonResult
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    Data = video
                };
            }
            catch (Exception e)
            {
                if (e is OauthExpiredTokenException)
                {
                    return new JsonResult
                    {
                        ContentEncoding = Encoding.UTF8,
                        ContentType = "application/json",
                        Data = new
                        {
                            Error = "NEED_LOGIN"
                        }
                    };
                }
                return new JsonResult
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    Data = new
                    {
                        Error = "UNKNOWN_ERROR"
                    }
                };
            }
        }
	}
}