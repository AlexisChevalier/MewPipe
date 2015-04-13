using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using MewPipe.API.Extensions;
using MewPipe.Logic.Models.Oauth;
using MewPipe.Logic.Repositories;

namespace MewPipe.API.Filters
{
    public class Oauth2AuthorizeFilter : FilterAttribute, IAuthorizationFilter 
    {
        private static readonly UnitOfWork UnitOfWork = new UnitOfWork();

        private string[] _scopes;
        public string Scopes
        {
            get { return String.Join(" ", _scopes); }
            set
            {
                _scopes = value.Split(' ');
            }
        }

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            var authenticationHeader = actionContext.Request.Headers.Authorization;
            OauthAccessToken token;
            if (authenticationHeader == null)
            {
                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    RequestMessage = actionContext.ControllerContext.Request,
                    ReasonPhrase = "Missing authentication header"
                };
                return FromResult(actionContext.Response);
            }

            if (authenticationHeader.Scheme.Equals("Bearer"))
            {
                token = UnitOfWork.OauthAccessTokenRepository.GetOne(at => at.Token == authenticationHeader.Parameter);

                if (token == null || token.ExpirationTime < DateTime.UtcNow)
                {
                    actionContext.Response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        RequestMessage = actionContext.ControllerContext.Request,
                        ReasonPhrase = "Invalid access token"
                    };
                    return FromResult(actionContext.Response);
                }
            }
            else
            {
                actionContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    RequestMessage = actionContext.ControllerContext.Request,
                    ReasonPhrase = "Unsupported authentication scheme"
                };
                return FromResult(actionContext.Response);
            }

            //Scope
            if (_scopes != null && _scopes.Length > 0)
            {
                var tokenScopes = !String.IsNullOrWhiteSpace(token.Scope) ? token.Scope.Split(' ') : new string[0];

                if (!tokenScopes.Intersect(_scopes).Any())
                {
                    actionContext.Response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        RequestMessage = actionContext.ControllerContext.Request,
                        ReasonPhrase = "Insufficient scope"
                    };
                    return FromResult(actionContext.Response);
                }
            }

            actionContext.SetUser(token.User);

            return continuation();
        }

        private static Task<HttpResponseMessage> FromResult(HttpResponseMessage result)
        {
            var source = new TaskCompletionSource<HttpResponseMessage>();
            source.SetResult(result);
            return source.Task;
        }

    }
}