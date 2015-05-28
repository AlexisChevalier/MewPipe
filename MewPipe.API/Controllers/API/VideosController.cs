using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AttributeRouting.Web.Http;
using MewPipe.API.Extensions;
using MewPipe.API.Filters;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;
using MewPipe.Logic.Services;

namespace MewPipe.API.Controllers.API
{
    public class VideosController : ApiController
    {
        [HttpGet]
        [Route("api/videos/{publicVideoId}")]
        [Oauth2AuthorizeFilter(AllowAnonymousUsers = true)]
        public VideoContract Get(string publicVideoId)
        {
            Debug.Assert(publicVideoId != null);

            var videoApiService = new VideoApiService();

            var user = ActionContext.GetUser();

            return new VideoContract(videoApiService.GetVideo(publicVideoId, user), user == null ? null : user.Id);   
        }


        [HttpPost]
        [Route("api/videos")]
        [Oauth2AuthorizeFilter]
        public VideoUploadTokenContract RequestUploadToken([FromBody]VideoUploadTokenRequestContract contract)
        {
            Debug.Assert(contract != null);

            var service = new VideoApiService();

            var token = service.GenerateVideoUploadToken(contract, ActionContext.GetUser());

            return new VideoUploadTokenContract(token);
        }

        [HttpPut]
        [Route("api/videos/{publicVideoId}")]
        [Oauth2AuthorizeFilter]
        public VideoContract PutUpdateBasicVideoDetails(string publicVideoId, [FromBody]VideoUpdateContract contract)
        {
            Debug.Assert(publicVideoId != null);
            Debug.Assert(contract != null);

            var videoApiService = new VideoApiService();

            var updatedVideo = videoApiService.UpdateVideo(publicVideoId, ActionContext.GetUser(), contract);

            return new VideoContract(updatedVideo);
        }

        [HttpDelete]
        [Route("api/videos/{publicVideoId}")]
        [Oauth2AuthorizeFilter]
        public VideoContract DeleteVideo(string publicVideoId)
        {
            Debug.Assert(publicVideoId != null);

            var videoApiService = new VideoApiService();

            var deletedVideo = videoApiService.DeleteVideo(publicVideoId, ActionContext.GetUser());

            return new VideoContract(deletedVideo);
        }

        [HttpGet]
        [Route("api/videos/{publicVideoId}/whiteList")]
        [Oauth2AuthorizeFilter]
        public UserContract[] GetVideoWhiteList(string publicVideoId)
        {
            Debug.Assert(publicVideoId != null);

            var videoApiService = new VideoApiService();

            var whiteList = videoApiService.GetVideoWhiteList(publicVideoId, ActionContext.GetUser());

            return whiteList;
        }

        [HttpPost]
        [Route("api/videos/{publicVideoId}/whiteList")]
        [Oauth2AuthorizeFilter]
        public UserContract[] PostUserToWhiteList(string publicVideoId, string userEmail)
        {
            Debug.Assert(userEmail != null);
            Debug.Assert(publicVideoId != null);

            var videoApiService = new VideoApiService();

            var whiteList = videoApiService.AddUserToWhiteList(publicVideoId, userEmail, ActionContext.GetUser());

            return whiteList;
        }

        [HttpDelete]
        [Route("api/videos/{publicVideoId}/whiteList")]
        [Oauth2AuthorizeFilter]
        public UserContract[] DeleteUserFromWhiteList(string publicVideoId, string userId)
        {
            Debug.Assert(userId != null);
            Debug.Assert(publicVideoId != null);

            var videoApiService = new VideoApiService();

            var whiteList = videoApiService.RemoveUserFromWhiteList(publicVideoId, userId, ActionContext.GetUser());

            return whiteList;
        }
    }
}
