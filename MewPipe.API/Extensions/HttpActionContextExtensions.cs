using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using MewPipe.Logic.Models;

namespace MewPipe.API.Extensions
{
    public static class HttpActionContextExtensions
    {
        public static void SetUser(this HttpActionContext httpActionContext, User user)
        {
            httpActionContext.Request.GetRouteData().Values.Add("USER", user);
        }

        public static User GetUser(this HttpActionContext httpActionContext)
        {
            return httpActionContext.Request.GetRouteData().Values["USER"] as User;
        }
    }
}