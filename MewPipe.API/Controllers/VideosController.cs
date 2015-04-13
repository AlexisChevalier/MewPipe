using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MewPipe.API.Filters;
using MewPipe.Logic.Services;

namespace MewPipe.API.Controllers
{
    public interface IVideosController
    {
        HttpResponseMessage Get(string videoId);
    }

    [Oauth2AuthorizeFilter]
    public class VideosController : ApiController, IVideosController
    {
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

        public HttpResponseMessage Post(string videoId)
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
    }
}
