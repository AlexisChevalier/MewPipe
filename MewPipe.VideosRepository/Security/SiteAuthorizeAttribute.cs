using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MewPipe.Website.Security;

namespace MewPipe.VideosRepository.Security
{
   
    public class SiteAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (!filterContext.HttpContext.GetIdentity().IsAuthenticated())
            {
                throw new HttpException(403, "Unauthorized");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}