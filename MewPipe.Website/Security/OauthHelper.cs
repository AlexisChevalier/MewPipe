using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MewPipe.ApiClient;
using MewPipe.Logic;
using MewPipe.Logic.Contracts;

namespace MewPipe.Website.Security
{
    public class OauthHelper
    {
        public static async Task<T> TryAuthenticatedMethod<T>(MewPipeApiClient client, AccessTokenContract accessToken, Func<Task<T>> method)
        {
            try
            {
                client.SetBearerToken(accessToken.access_token);
                return await method();
            }
            catch (OauthExpiredTokenException) {}
            try
            {
                var refreshToken = await client.GetRefreshToken(ConfigurationManager.AppSettings["OAuth2TokenEndpoint"],
                    accessToken);
                var identity = HttpContext.Current.GetIdentityFromContext();
                identity.AccessToken = new SerializableAccessTokenContract(new AccessTokenContract
                {
                    access_token = refreshToken.access_token,
                    expires_in = refreshToken.expires_in,
                    refresh_token = refreshToken.refresh_token,
                    scope = refreshToken.scope,
                    token_type = refreshToken.token_type
                });
                HttpContext.Current.SaveIdentityFromContext(identity);

                client.SetBearerToken(refreshToken.access_token);
                return await method();
            }
            catch (OauthExpiredTokenException e)
            {
                throw;
            }
        }
    }
}