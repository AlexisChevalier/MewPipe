using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AttributeRouting.Web.Http;
using MewPipe.API.Extensions;
using MewPipe.API.Filters;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Factories;
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

            var videoApiService = new VideoServiceFactory().GetVideoApiService();

            var user = ActionContext.GetUser();

            return new VideoContract(videoApiService.GetVideo(publicVideoId, user), user == null ? null : user.Id);   
        }


        [HttpPost]
        [Route("api/videos")]
        [Oauth2AuthorizeFilter]
        public VideoUploadTokenContract RequestUploadToken([FromBody]VideoUploadTokenRequestContract contract)
        {
            Debug.Assert(contract != null);

            var service = new VideoServiceFactory().GetVideoApiService();

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

            var videoApiService = new VideoServiceFactory().GetVideoApiService();

            var updatedVideo = videoApiService.UpdateVideo(publicVideoId, ActionContext.GetUser(), contract);

            return new VideoContract(updatedVideo);
        }

        [HttpDelete]
        [Route("api/videos/{publicVideoId}")]
        [Oauth2AuthorizeFilter]
        public VideoContract DeleteVideo(string publicVideoId)
        {
            Debug.Assert(publicVideoId != null);

            var videoApiService = new VideoServiceFactory().GetVideoApiService();

            var deletedVideo = videoApiService.DeleteVideo(publicVideoId, ActionContext.GetUser());

            return new VideoContract(deletedVideo);
        }

        [HttpGet]
        [Route("api/videos/{publicVideoId}/whiteList")]
        [Oauth2AuthorizeFilter]
        public UserContract[] GetVideoWhiteList(string publicVideoId)
        {
            Debug.Assert(publicVideoId != null);

            var videoApiService = new VideoServiceFactory().GetVideoApiService();

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

            var videoApiService = new VideoServiceFactory().GetVideoApiService();

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

            var videoApiService = new VideoServiceFactory().GetVideoApiService();

            var whiteList = videoApiService.RemoveUserFromWhiteList(publicVideoId, userId, ActionContext.GetUser());

            return whiteList;
        }

        [HttpGet]
        [Route("api/videos/trends")]
        [Oauth2AuthorizeFilter(AllowAnonymousUsers = true)]
        public VideoContract[] GetWorldTrends()
        {
            var unitOfWork = new UnitOfWork();
            var today = DateTime.UtcNow;

            var trends = unitOfWork.GetContext()
                .Impressions
                .Include("Video")
                .Include("Video.User")
                .Where(i => i.Type == Impression.ImpressionType.Good && i.Video.PrivacyStatus == Video.PrivacyStatusTypes.Public && i.Video
                .Status == Video.StatusTypes.Published)
                .AsEnumerable()
                .GroupBy(i => new
                {
                    video = i.Video,
                    //date = i.DateTimeUtc.ToString("yyyy-M-d")
                    date = (today - i.DateTimeUtc).Seconds % 86400
                })
                .AsEnumerable()
                .Select(g => new
                {
                    video = g.Key.video,
                    count = g.Count(),
                    date = g.Key.date
                })
                .OrderByDescending(g => g.date)
                .ThenByDescending(g => g.count)
                .GroupBy(g => g.video.Id, (key, c) => c.FirstOrDefault())
                .Take(40)
                .Select(i => i.video).ToList();

            var trendsContract = trends.Select(trend => new VideoContract(trend)).ToList();

            //TODO: THIS BUGS.
            if (trends.Count < 40)
            {
                var bannedIds = trends.Select(t => t.Id).ToArray();

                var added = unitOfWork
                    .VideoRepository
                    .Get(v => !bannedIds.Contains(v.Id) && v.PrivacyStatus == Video.PrivacyStatusTypes.Public && v.Status == Video.StatusTypes.Published, q => q.OrderByDescending(v => v.DateTimeUtc), "User").Take(40 - trends.Count);

                trendsContract.AddRange(added.Select(addedVideo => new VideoContract(addedVideo)));
            }

            return trendsContract.ToArray();
        }
    }
}
