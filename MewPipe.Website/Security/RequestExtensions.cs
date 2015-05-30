using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

        public static Identity GetIdentityValueFromState(this HttpSessionState session)
        {
            return session[IDENTITY_SESSION_KEY] as Identity ?? session.SaveIdentityValueFromState(new Identity());
        }

        public static Identity SaveIdentityValueFromState(this HttpSessionState session, Identity identityValue)
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

        public static Identity GetIdentityFromContext(this HttpContext context)
        {
            return context.Session.GetIdentityValueFromState();
        }

        public static Identity SaveIdentityFromContext(this HttpContext context, Identity identityValue)
        {
            return context.Session.SaveIdentityValueFromState(identityValue);
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

    [Serializable]
    public class Identity
    {
        public SerializableUserContract User { get; set; }
        public SerializableAccessTokenContract AccessToken { get; set; }

        public Identity()
        {
            User = null;
            AccessToken = null;
        }

        public Identity(UserContract user, AccessTokenContract accessToken)
        {
            User = new SerializableUserContract(user);
            AccessToken = new SerializableAccessTokenContract(accessToken);
        }

        public bool IsAuthenticated()
        {
            return User != null;
        }
    }

    [Serializable]
    public class SerializableUserContract
    {
        public SerializableUserContract()
        {
        }
        public SerializableUserContract(UserContract user)
        {
            Id = user.Id;
            Email = user.Email;
        }

        public UserContract ToUserContract()
        {
            return new UserContract
            {
                Email = Email,
                Id = Id
            };
        }

        public string Id { get; set; }
        public string Email { get; set; }
    }

    [Serializable]
    public class SerializableAccessTokenContract
    {
        public SerializableAccessTokenContract(AccessTokenContract accessTokenContract)
        {
            access_token = accessTokenContract.access_token;
            token_type = accessTokenContract.token_type;
            expires_in = accessTokenContract.expires_in;
            refresh_token = accessTokenContract.refresh_token;
            scope = accessTokenContract.scope;
        }
        public AccessTokenContract ToAccessTokenContract()
        {
            return new AccessTokenContract
            {
                access_token = access_token,
                token_type = token_type,
                expires_in = expires_in,
                refresh_token = refresh_token,
                scope = scope
            };
        }

        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
    }
}