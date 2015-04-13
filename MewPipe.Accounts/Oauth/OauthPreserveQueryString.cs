using System.Linq;
using System.Web.Mvc;

namespace MewPipe.Accounts.Oauth
{
    public class OauthPreserveQueryString : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var redirectResult = filterContext.Result as RedirectToRouteResult;
            if (redirectResult != null)  {

                var query = filterContext.HttpContext.Request.QueryString;

                foreach (var key in query.Keys.Cast<string>().Where(key => !redirectResult.RouteValues.ContainsKey(key)))
                {
                    redirectResult.RouteValues.Add(key, query[key]);
                }
            }
        }
    }
}