using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;
using MewPipe.Logic.Services;
using MewPipe.VideosRepository.Security;
using MewPipe.Website.Security;
using Newtonsoft.Json.Linq;

namespace MewPipe.VideosRepository.Controllers.API
{
    public class VideosDataController : ApiController
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        [Route("api/videosData/{publicVideoId}")]
        public HttpResponseMessage Get(string publicVideoId, string encoding = "", string quality = "")
        {
            Debug.Assert(publicVideoId != null);

            var identity = HttpContext.Current.GetIdentity();

            User user = null;
            if (identity.User != null)
            {
                user = _unitOfWork.UserRepository.GetById(identity.User.Id);
            }

            var videoApiService = new VideoApiService();

            try
            {
                return videoApiService.GetVideoHttpResponseMessage(Request, publicVideoId, user);
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

        [Route("api/videosData")]
        public async Task<HttpResponseMessage> Post(string uploadRequestId, string returnType = "redirect")
        {
            Debug.Assert(uploadRequestId != null);

            var videoApiService = new VideoApiService();

            var video = await videoApiService.UploadVideoFromMultipartRequest(Request, uploadRequestId);

            if (returnType == "redirect")
            {
                var response = Request.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = new Uri(video.UploadRedirectUri + video.PublicId);
                return response;
            }

            return new HttpResponseMessage
            {
                Content = new ObjectContent<dynamic>(new
                {
                    VideoId = video.PublicId
                }, new JsonMediaTypeFormatter())
            };
        }
    }
}
