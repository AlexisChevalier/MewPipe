using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MewPipe.API.Filters;
using MewPipe.Logic.RabbitMQ;
using MewPipe.Logic.RabbitMQ.Messages;
using MewPipe.Logic.Services;

namespace MewPipe.API.Controllers
{
    public interface IVideosController
    {
        HttpResponseMessage Get(string videoId);
    }

    public class VideosController : ApiController, IVideosController
    {
        [Oauth2AuthorizeFilter(AllowAnonymousUsers = true)]
        public HttpResponseMessage Get(string videoId)
        {
            Debug.Assert(videoId != null);

            var videoApiService = new VideoApiService();

            try
            {
                return videoApiService.GetVideoHttpResponseMessage(Request, videoId, null);
            }
            catch (InvalidByteRangeException byteRangeException)
            {
                return Request.CreateErrorResponse(byteRangeException);
            }
            catch (HttpException httpException)
            {
                return Request.CreateErrorResponse((HttpStatusCode)httpException.GetHttpCode(), httpException);
            }
        }

        [Oauth2AuthorizeFilter]
        public HttpResponseMessage Post(string videoId)
        {
            using (var workerQueueManager = new WorkerQueueManager())
            {
                using (var channelQueue = workerQueueManager.GetChannelQueue(WorkerQueueManager.QueueChannelIdentifier.NewVideos))
                {
                    channelQueue.SendPersistentMessage(new NewVideoMessage(videoId));
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
