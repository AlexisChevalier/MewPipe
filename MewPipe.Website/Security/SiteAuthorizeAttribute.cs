using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using MewPipe.Website.Extensions;

namespace MewPipe.Website.Security
{
   
    public class SiteAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (!filterContext.HttpContext.GetIdentity().IsAuthenticated())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    {
                        "controller", 
                        "Home"
                    },            
                    { 
                        "action", 
                        "Index" 
                    }
                }).Error("You must be logged in to access this area !");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}