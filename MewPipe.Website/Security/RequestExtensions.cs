using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using MewPipe.Logic.Contracts;

namespace MewPipe.Website.Security
{
    public static class RequestExtensions
    {
        private const string IDENTITY_SESSION_KEY = "Identity";

        public static Identity GetIdentityValue(this HttpSessionStateBase session)
        {
            return session[IDENTITY_SESSION_KEY] as Identity ?? session.SaveIdentityValue(new Identity());
        }

        public static Identity SaveIdentityValue(this HttpSessionStateBase session, Identity identityValue)
        {
            session[IDENTITY_SESSION_KEY] = identityValue;

            return identityValue;
        }

        public static Identity GetIdentity(this HttpContextBase context)
        {
            return context.Session.GetIdentityValue();      
        }

        public static Identity SaveIdentity(this HttpContextBase context, Identity identityValue)
        {
            return context.Session.SaveIdentityValue(identityValue);
        }

        public static Identity GetIdentity(this HttpRequestBase context)
        {
            return context.RequestContext.HttpContext.Session.GetIdentityValue();
        }

        public static Identity SaveIdentity(this HttpRequestBase context, Identity identityValue)
        {
            return context.RequestContext.HttpContext.Session.SaveIdentityValue(identityValue);
        }
    }

    public class Identity
    {
        public UserContract User { get; set; }
        public AccessTokenContract AccessToken { get; set; }

        public Identity()
        {
            User = null;
            AccessToken = null;
        }

        public Identity(UserContract user, AccessTokenContract accessToken)
        {
            User = user;
            AccessToken = accessToken;
        }

        public bool IsAuthenticated()
        {
            return User != null;
        }
    }
}