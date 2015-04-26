using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MewPipe.API.Extensions;
using MewPipe.API.Filters;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;
using MewPipe.Logic.Services;

namespace MewPipe.API.Controllers.API
{
    public interface IVideosController
    {
        VideoContract GetVideosDetails(string publicVideoId);
        VideoUploadTokenContract RequestUploadToken(VideoUploadTokenRequestContract contract);
    }

    public class VideosController : ApiController, IVideosController
    {
        [Oauth2AuthorizeFilter(AllowAnonymousUsers = true)]
        public VideoContract GetVideosDetails(string publicVideoId)
        {
            Debug.Assert(publicVideoId != null);

            var videoApiService = new VideoApiService();

            return new VideoContract(videoApiService.GetVideo(publicVideoId, ActionContext.GetUser()));

        }

        [Oauth2AuthorizeFilter]
        public VideoUploadTokenContract RequestUploadToken([FromBody]VideoUploadTokenRequestContract contract)
        {
            Debug.Assert(contract != null);

            var service = new VideoApiService();

            var token = service.GenerateVideoUploadToken(contract, ActionContext.GetUser());

            return new VideoUploadTokenContract(token);
        }
    }
}
