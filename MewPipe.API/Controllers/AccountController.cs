using System.Collections.Generic;
using System.Web.Http;
using MewPipe.API.Extensions;
using MewPipe.API.Filters;
using MewPipe.Logic.Contracts;

namespace MewPipe.API.Controllers
{
    [Oauth2AuthorizeFilter]
    public class AccountController : ApiController
    {
        // GET api/account
        public UserContract Get()
        {
            var user = new UserContract(ActionContext.GetUser());

            return user;
        }
    }
}
