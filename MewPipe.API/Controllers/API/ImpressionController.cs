using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Authentication;
using System.Web.Http;
using MewPipe.API.Extensions;
using MewPipe.API.Filters;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Factories;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;
using MewPipe.Logic.Services;

namespace MewPipe.API.Controllers.API
{
    [Oauth2AuthorizeFilter]
    public class ImpressionController : ApiController
    {
        [HttpGet]
        [Route("api/impressions/{userId}/{publicVideoId}")]
        [Oauth2AuthorizeFilter]
        public ImpressionContract GetVideoImpression(string publicVideoId)
        {
            var service = new VideoServiceFactory().GetVideoApiService();

            if (!service.IsUserAllowedToSeeVideo(publicVideoId, ActionContext.GetUser()))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            var uow = new UnitOfWork();

            var id = ShortGuid.Decode(publicVideoId);

            var userId = ActionContext.GetUser().Id;

            var impression = uow.ImpressionRepository.GetOne(i => i.User.Id == userId && i.Video.Id == id, "User, Video");

            if (impression == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return new ImpressionContract(impression);
        }


        [HttpPost]
        [Route("api/impressions")]
        [Oauth2AuthorizeFilter]
        public VideoContract SetVideoImpression(ImpressionContract impression)
        {
            var service = new VideoServiceFactory().GetVideoApiService();

            if (!service.IsUserAllowedToSeeVideo(impression.PublicVideoId, ActionContext.GetUser()))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            var videoApiService = new VideoServiceFactory().GetVideoApiService();

            var video = videoApiService.SetImpression(impression);
            var user = ActionContext.GetUser();

            return new VideoContract(video, user.Id);
        }
    }
}
