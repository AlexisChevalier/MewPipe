using System;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using MewPipe.Logic.MongoDB;
using MewPipe.Website.Security;

namespace MewPipe.VideosRepository.Security
{
    public static class RequestExtensions
    {
        private const string IDENTITY_SESSION_KEY = "Identity";

        public static Identity GetIdentity(this HttpContext context)
        {
            var sessionStateSection = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");

            var cookie = context.Request.Cookies.Get(sessionStateSection.CookieName);

            if (cookie == null)
            {
                return null;
            }

            return GetIdentityFromStore(cookie.Value, context);
        }

        private static Identity GetIdentityFromStore(string cookie, HttpContext context)
        {
            var sessionStore = new MongoSessionStateStore(
                ConfigurationManager.ConnectionStrings["MewPipeMongoConnection"].ConnectionString,
                "/");

            bool locked;
            TimeSpan lockAge;
            object lockId;
            SessionStateActions actions;

            var data = sessionStore.GetItem(context, cookie, out locked, out lockAge, out lockId, out actions);

            return data == null ? null : data.Items[IDENTITY_SESSION_KEY] as Identity;
        }
    }
}