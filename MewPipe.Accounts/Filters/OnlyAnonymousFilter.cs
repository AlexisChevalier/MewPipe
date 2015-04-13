using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MewPipe.Logic.Models.Oauth;
using MewPipe.Logic.Repositories;
using System.Web.Mvc;
using System.Web.Routing;

namespace MewPipe.Accounts.Filters
{
    public class OnlyAnonymousFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary 
                { 
                    { "controller", "Manage" }, 
                    { "action", "Index" } 
                });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}