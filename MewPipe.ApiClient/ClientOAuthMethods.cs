using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MewPipe.Logic.Contracts;
using MewPipe.Logic.Extensions;

namespace MewPipe.ApiClient
{
    public partial class MewPipeApiClient
    {
        public Uri CreateAuthorizeUri(string authorizeEndpoint, string redirectEndpoint)
        {
            return new Uri(authorizeEndpoint + "?response_type=code&client_id=" + _clientId + "&redirect_uri=" + redirectEndpoint);
        }

        public async Task<AccessTokenContract> GetAccessToken(string tokenEndpoint, string code, string redirectEndpoint)
        {
            var queryString = new NameValueCollection {
                {"grant_type", "authorization_code"},
                {"code", code},
                {"redirect_uri", redirectEndpoint},
                {"client_id", _clientId},
                {"client_secret", _clientSecret}
            }.ToQueryString();

            return await _httpClient.SendPost<AccessTokenContract>(queryString, null, tokenEndpoint);
        }

        public async Task<RefreshTokenContract> GetRefreshToken(string tokenEndpoint, AccessTokenContract accessToken)
        {
            var queryString = new NameValueCollection {
                {"grant_type", "refresh_token"},
                {"refresh_token", accessToken.refresh_token},
                {"client_id", _clientId},
                {"client_secret", _clientSecret}
            }.ToQueryString();

            var refreshToken = await _httpClient.SendPost<RefreshTokenContract>(queryString, null, tokenEndpoint);

            return refreshToken;
        }
    }
}
