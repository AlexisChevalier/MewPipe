using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using MewPipe.Logic.Helpers;
using MewPipe.Logic.Models;
using MewPipe.Logic.Repositories;
using MewPipe.Logic.Services;
using MewPipe.VideosRepository.Security;
using MewPipe.Website.Security;
using Newtonsoft.Json.Linq;

namespace MewPipe.VideosRepository.Controllers.API
{
    [EnableCors(origins: "http://mewpipe.local:44402", headers: "*", methods: "*")]
    public class ThumbnailsDataController : ApiController
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        [Route("api/thumbnailsData/{publicVideoId}")]
        public HttpResponseMessage Get(string publicVideoId)
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
                return videoApiService.GetThumbnailHttpResponseMessage(Request, publicVideoId, user);
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
