using System;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;
using MewPipe.Accounts.Oauth;
using MewPipe.Accounts.ViewModels;
using MewPipe.Logic.Extensions;
using MewPipe.Logic.Models.Oauth;
using MewPipe.Logic.Repositories;
using Microsoft.AspNet.Identity;

namespace MewPipe.Accounts.Controllers
{
    public class OauthController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        /**
         * Authorize Endpoint
         */
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
                //TODO: Handle Error
                throw new AuthenticationException();
            }

            if (client == null)
            {
                //TODO: Handle Error
                return HandleOauthError(OauthErrors.InvalidParameter, "invalid client_id");
            }

            if (client.RedirectUri != viewModel.redirect_uri)
            {
                //TODO: Handle Error
                return HandleOauthError(OauthErrors.InvalidParameter, "invalid redirect_rui");
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
                var authorizationCode = new OauthAuthorizationCode
                {
                    User = user,
                    OauthClient = client,
                    State = viewModel.state,
                    Scope = viewModel.scope,
                    RedirectUri = viewModel.redirect_uri,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(10),
                    Code = TokenGenerator.GenerateRandomString(8)
                };

                _unitOfWork.OauthAuthorizationCodeRepository.Insert(authorizationCode);
                _unitOfWork.Save();

                return Redirect(viewModel.redirect_uri + new NameValueCollection
                {
                    {"code", authorizationCode.Code},
                    {"state", viewModel.state}
                }.ToQueryString());
            }

            return HandleOauthError(OauthErrors.InvalidResponseType);
        }

        /**
         * Dialog view
         */
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

            ViewBag.QueryString = viewModel;

            return View(new DialogViewModel
            {
                Scope = viewModel.scope,
                ClientId = client.ClientId
            });
        }

        /**
         * Dialog POST
         */
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

            return Redirect(client.RedirectUri + new NameValueCollection
            {
                {"error", "access_denied"},
                {"error_description", "The user denied access to your application"}
            }.ToQueryString());
        }

        /**
         * Access Token issuance Endpoint
         */
        [Route("oauth/token", Name = "OauthAccessToken")]
        [TokenMethodSelectorAtrribute(GrantType = "authorization_code")]
        [HttpPost]
        public JsonResult AccessToken(AccessTokenRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                //TODO: Handle Error
                return HandleJsonOauthError(OauthErrors.InvalidParameter, string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }

            var client = _unitOfWork.OauthClientRepository.GetOne(oc => oc.ClientId == viewModel.client_id && oc.ClientSecret == viewModel.client_secret);
            var authorizationCode =
                _unitOfWork.OauthAuthorizationCodeRepository.GetOne(
                    ac =>
                        ac.OauthClient.Id == client.Id && ac.Code == viewModel.code);

            if (authorizationCode == null)
            {

                //TODO: Handle Error
                return HandleJsonOauthError(OauthErrors.InvalidParameter, "code invalid");
            }

            var user = authorizationCode.User;

            if (user == null)
            {
                //TODO: Handle Error
                throw new AuthenticationException();
            }

            if (client == null)
            {
                //TODO: Handle Error
                return HandleJsonOauthError(OauthErrors.InvalidParameter, "invalid client_id or secret_id");
            }

            if (authorizationCode.ExpirationTime < DateTime.UtcNow)
            {

                //TODO: Handle Error
                _unitOfWork.OauthAuthorizationCodeRepository.Delete(authorizationCode);
                _unitOfWork.Save();
                return HandleJsonOauthError(OauthErrors.InvalidParameter, "code expired");
            }

            if (authorizationCode.RedirectUri != viewModel.redirect_uri)
            {

                //TODO: Handle Error
                return HandleJsonOauthError(OauthErrors.InvalidParameter, "redirect_uri invalid");
            }

            if (authorizationCode.Scope != viewModel.scope)
            {

                //TODO: Handle Error
                return HandleJsonOauthError(OauthErrors.InvalidParameter, "scope invalid");
            }

            if (authorizationCode.State != viewModel.state)
            {

                //TODO: Handle Error
                return HandleJsonOauthError(OauthErrors.InvalidParameter, "state invalid");
            }

            _unitOfWork.OauthAuthorizationCodeRepository.Delete(authorizationCode);

            var accessToken = new OauthAccessToken
            {
                ExpirationTime = DateTime.UtcNow.AddHours(1),
                Token = TokenGenerator.GenerateRandomString(32),
                Type = "bearer",
                OauthClient = client,
                User = user,
                Scope = viewModel.scope
            };

            _unitOfWork.OauthAccessTokenRepository.Insert(accessToken);

            var refreshToken = new OauthRefreshToken
            {
                OauthClient = client,
                User = user,
                Scope = viewModel.scope,
                ExpirationTime = DateTime.UtcNow.AddDays(14),
                Token = TokenGenerator.GenerateRandomString(64)
            };

            _unitOfWork.OauthRefreshTokenRepository.Insert(refreshToken);

            _unitOfWork.Save();

            var result = new AccessTokenResponseViewModel
            {
                access_token = accessToken.Token,
                expires_in = 3600,
                scope = viewModel.scope,
                refresh_token = refreshToken.Token,
                token_type = accessToken.Type

            };

            return Json(result);
        }

        /**
         * Access Token refreh Endpoint
         */
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

            var oldRefreshToken = _unitOfWork.OauthRefreshTokenRepository.GetOne(rt => rt.Token == viewModel.refresh_token);

            if (oldRefreshToken == null)
            {
                //TODO: Handle Error
                return HandleJsonOauthError(OauthErrors.InvalidParameter, "invalid refresh_token");
            }
            var user = oldRefreshToken.User;

            if (user == null)
            {
                //TODO: Handle Error
                throw new AuthenticationException();
            }

            

            var newAccessToken = new OauthAccessToken
            {
                ExpirationTime = DateTime.UtcNow.AddHours(1),
                Token = TokenGenerator.GenerateRandomString(32),
                Type = "bearer",
                OauthClient = oldRefreshToken.OauthClient,
                User = oldRefreshToken.User,
                Scope = oldRefreshToken.Scope
            };

            _unitOfWork.OauthAccessTokenRepository.Insert(newAccessToken);

            var newRefreshToken = new OauthRefreshToken
            {
                OauthClient = oldRefreshToken.OauthClient,
                User = oldRefreshToken.User,
                Scope = oldRefreshToken.Scope,
                ExpirationTime = DateTime.UtcNow.AddDays(14),
                Token = TokenGenerator.GenerateRandomString(64)
            };

            _unitOfWork.OauthRefreshTokenRepository.Insert(newRefreshToken);
            _unitOfWork.OauthRefreshTokenRepository.Delete(oldRefreshToken);

            _unitOfWork.Save();

            var result = new AccessTokenResponseViewModel
            {
                access_token = newAccessToken.Token,
                expires_in = 3600,
                scope = viewModel.scope,
                refresh_token = newRefreshToken.Token,
                token_type = newAccessToken.Type
            };

            return Json(result);
        }

        #region Helpers

        private ActionResult HandleOauthError(OauthError error, string details = null)
        {
            error.Details = details;
            ViewBag.Error = error;

            return View("OauthError");
        }

        private JsonResult HandleJsonOauthError(OauthError error, string details = null)
        {
            error.Details = details;
            Response.StatusCode = error.HttpCode;
            return Json(error);
        }

        private void CleanupOauthDb()
        {
            //Cleanup Authorization codes

            _unitOfWork.OauthAuthorizationCodeRepository.DeleteMany(ac => ac.ExpirationTime < DateTime.UtcNow);

            //Cleanup Access tokens

            _unitOfWork.OauthAccessTokenRepository.DeleteMany(ac => ac.ExpirationTime < DateTime.UtcNow);

            //Cleanup Refresh tokens

            _unitOfWork.OauthRefreshTokenRepository.DeleteMany(ac => ac.ExpirationTime < DateTime.UtcNow);

        }
        #endregion
    }
}