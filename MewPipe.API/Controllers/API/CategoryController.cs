using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using MewPipe.API.Extensions;
using MewPipe.API.Filters;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Repositories;
using MewPipe.Logic.Services;

namespace MewPipe.API.Controllers.API
{
    [Oauth2AuthorizeFilter]
    public class CategoryController : ApiController
    {
        // GET api/account
        [Route("api/categories")]
        public CategoryContract[] Get()
        {
            var uow = new UnitOfWork();

            var categories = uow.CategoryRepository.Get().Select(c => new CategoryContract(c)).ToArray();

            return categories;
        }
    }
}
