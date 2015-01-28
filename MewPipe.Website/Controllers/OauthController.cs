using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;
using MewPipe.DAL;
using MewPipe.DAL.Models.Oauth;
using MewPipe.DAL.Repositories;
using MewPipe.Website.Oauth;
using MewPipe.Website.ViewModels;
using Microsoft.AspNet.Identity;

namespace MewPipe.Website.Controllers
{
    public class OauthController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        [Route("oauth/authorize", Name = "OauthAuthorize")]
        [Authorize]
        [OauthPreserveQueryString]
        public ActionResult Authorize(AuthorizeRequestViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                //TODO: Handle Error
                return HandleOauthError(OauthErrors.InvalidParameter, string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }

            var client = _unitOfWork.OauthClientRepository.GetOne(oc => oc.ClientId == viewModel.client_id);
            var userId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepository.GetOne(u => u.Id == userId);

            if (user == null)
            {
                throw new AuthenticationException();
            }

            if (client == null)
            {
                return HandleOauthError(OauthErrors.InvalidParameter, "invalid client_id");
            }

            var trust = _unitOfWork.OauthUserTrustRepository.GetOne(t => t.User.Id == user.Id && t.OauthClient.Id == client.Id);

            if (trust == null)
            {
                return RedirectToAction("Dialog");
            }

            /**
             * Implicit Grant
             */
            if (viewModel.response_type.Equals("token"))
            {
                //TODO
            }

            /**
             * Authorization Code Grant
             */
            if (viewModel.response_type.Equals("code"))
            {
                var authorizationCode = new OauthAuthorizationCode(user, client, viewModel.state, viewModel.scope,
                    TokenGenerator.GenerateRandomString(8), viewModel.redirect_uri);

                _unitOfWork.OauthAuthorizationCodeRepository.Insert(authorizationCode);
                _unitOfWork.Save();

                return Redirect(viewModel.redirect_uri + ToQueryString(new NameValueCollection
                {
                    {"code", authorizationCode.Code},
                    {"state", viewModel.state}
                }));
            }

            return HandleOauthError(OauthErrors.InvalidResponseType);
        }

        [Route("oauth/dialog", Name = "OauthDialog")]
        [Authorize]
        [OauthPreserveQueryString]
        public ActionResult Dialog(DialogRequest viewModel)
        {
            if (!ModelState.IsValid)
            {
                //TODO: Handle Error
                return HandleOauthError(OauthErrors.InvalidParameter, string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }

            var client = _unitOfWork.OauthClientRepository.GetOne(oc => oc.ClientId == viewModel.client_id);
            var userId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepository.GetOne(u => u.Id == userId);

            if (user == null)
            {
                throw new AuthenticationException();
            }

            if (client == null)
            {
                return HandleOauthError(OauthErrors.InvalidParameter, "invalid client_id");
            }

            ViewBag.User = user;
            ViewBag.Client = client;
            ViewBag.Scope = viewModel.scope;

            return View(new DialogViewModel
            {
                Scope = viewModel.scope,
                ClientId = client.ClientId
            });
        }

        [Route("oauth/dialog", Name = "OauthDialogPost")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [OauthPreserveQueryString]
        public ActionResult DialogPost(DialogViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                //TODO: Handle Error
                return HandleOauthError(OauthErrors.InvalidParameter, string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }

            var client = _unitOfWork.OauthClientRepository.GetOne(oc => oc.ClientId == viewModel.ClientId);
            var userId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepository.GetOne(u => u.Id == userId);

            if (user == null)
            {
                throw new AuthenticationException();
            }

            if (client == null)
            {
                return HandleOauthError(OauthErrors.InvalidParameter, "invalid client_id");
            }

            if (viewModel.Decision == "Allow")
            {
                var trust = new OauthUserTrust
                {
                    OauthClient = client,
                    User = user,
                    Scope = viewModel.Scope
                };

                _unitOfWork.OauthUserTrustRepository.Insert(trust);
                _unitOfWork.Save();

                return RedirectToAction("Authorize");
            }

            return Redirect(client.RedirectUri + ToQueryString(new NameValueCollection
            {
                {"error", "access_denied"},
                {"error_description", "The user denied access to your application"}
            }));
        }

        [Route("oauth/token", Name = "OauthAccessToken")]
        [TokenMethodSelectorAtrribute(GrantType = "authorization_code")]
        [HttpPost]
        public ActionResult AccessToken(AccessTokenRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                //TODO: Handle Error
                return HandleOauthError(OauthErrors.InvalidParameter, string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }

            if (!viewModel.grant_type.Equals("authorization_code"))
            {
                //TODO: Handle Error
                return HandleOauthError(OauthErrors.InvalidParameter, "grant_type invalid");
            }

            var client = _unitOfWork.OauthClientRepository.GetOne(oc => oc.ClientId == viewModel.client_id && oc.ClientSecret == viewModel.client_secret);
            var userId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepository.GetOne(u => u.Id == userId);

            if (user == null)
            {
                throw new AuthenticationException();
            }

            if (client == null)
            {
                return HandleOauthError(OauthErrors.InvalidParameter, "invalid client_id or secret_id");
            }

            var authorizationCode =
                _unitOfWork.OauthAuthorizationCodeRepository.GetOne(
                    ac =>
                        ac.OauthClient.Id == client.Id && ac.User.Id == user.Id && ac.Code == viewModel.code);

            if (authorizationCode == null)
            {
                return HandleOauthError(OauthErrors.InvalidParameter, "code invalid");
            }

            if (authorizationCode.Validity < DateTime.UtcNow)
            {
                _unitOfWork.OauthAuthorizationCodeRepository.Delete(authorizationCode);
                _unitOfWork.Save();
                return HandleOauthError(OauthErrors.InvalidParameter, "code expired");
            }

            if (authorizationCode.RedirectUri != viewModel.redirect_uri)
            {
                return HandleOauthError(OauthErrors.InvalidParameter, "redirect_uri invalid");
            }

            if (authorizationCode.Scope != viewModel.scope)
            {
                return HandleOauthError(OauthErrors.InvalidParameter, "scope invalid");
            }

            if (authorizationCode.State != viewModel.state)
            {
                return HandleOauthError(OauthErrors.InvalidParameter, "state invalid");
            }



            _unitOfWork.OauthAuthorizationCodeRepository.Delete(authorizationCode);
            _unitOfWork.Save();

            return null;
        }


        [Route("oauth/token", Name = "OauthRefrestToken")]
        [TokenMethodSelectorAtrribute(GrantType = "refresh_token")]
        [HttpPost]
        public ActionResult RefreshToken(RefreshTokenRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                //TODO: Handle Error
                return HandleOauthError(OauthErrors.InvalidParameter, string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            //Verify RefreshToken

            //Return Access Token
            return null;
        }

        #region Helpers

        private ActionResult HandleOauthError(OauthError error, string details = null)
        {
            error.Details = details;
            ViewBag.Error = error;
            Response.StatusCode = error.HttpCode;
            return View("OauthError");
        }

        private string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }
        #endregion
    }
}