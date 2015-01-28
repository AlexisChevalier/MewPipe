using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MewPipe.Website.Oauth
{
    public class OauthPreserveQueryString : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var redirectResult = filterContext.Result as RedirectToRouteResult;
            if (redirectResult == null)
            {
                return;
            }

            var query = filterContext.HttpContext.Request.QueryString;
            
            foreach (var key in query.Keys.Cast<string>().Where(key => !redirectResult.RouteValues.ContainsKey(key)))
            {
                redirectResult.RouteValues.Add(key, query[key]);
            }
        }
    }
}