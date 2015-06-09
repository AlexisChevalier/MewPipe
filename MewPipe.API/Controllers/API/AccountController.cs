using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using MewPipe.API.Extensions;
using MewPipe.API.Filters;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Factories;
using MewPipe.Logic.Services;

namespace MewPipe.API.Controllers.API
{
    [Oauth2AuthorizeFilter]
    public class AccountController : ApiController
    {
        // GET api/account
        [Route("api/account")]
        public UserContract Get()
        {
            var user = new UserContract(ActionContext.GetUser());

            return user;
        }

        [Route("api/account/videos")]
        public VideoContract[] GetVideos()
        {
            var user = ActionContext.GetUser();

            var videoApiService = new VideoServiceFactory().GetVideoApiService();

            var videos = videoApiService.GetUserVideos(user);

            return videos.Select(v => new VideoContract(v)).ToArray();
        }
    }
}
