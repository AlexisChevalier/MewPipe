﻿using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using MewPipe.ApiClient;
using MewPipe.Logic;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Models;
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
            var video = await _apiClient.GetVideoDetails(videoId);

            if (video == null)
            {
                //TODO: 404
            }

		    var videoService = new VideoApiService();

		    var updatedVideo = videoService.AddView(video.PublicId);
		    video.Views = updatedVideo.Views;

            ViewBag.VideoDetails = video;
		    ViewBag.JsonVideoDetails = JsonConvert.SerializeObject(video);
			return View();
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
                    PublicId = video.PublicId
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
                            PrivacyStatus = viewModel.PrivacyStatus
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
	}
}