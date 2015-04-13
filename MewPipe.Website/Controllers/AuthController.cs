using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MewPipe.ApiClient;
using MewPipe.Website.Extensions;
using MewPipe.Website.Security;
using MewPipe.Website.ViewModels;

namespace MewPipe.Website.Controllers
{
    public class AuthController : Controller
    {
        readonly MewPipeApiClient _apiClient = new MewPipeApiClient(
                ConfigurationManager.AppSettings["APIEndpoint"],
                ConfigurationManager.AppSettings["OAuth2ClientID"],
                ConfigurationManager.AppSettings["OAuth2ClientSecret"]);

        // GET: Auth
        public ActionResult Login()
        {
            
            var uri = _apiClient.CreateAuthorizeUri(
                ConfigurationManager.AppSettings["OAuth2AuthorizeEndpoint"],
                ConfigurationManager.AppSettings["OAuth2RedirectEndpoint"]).ToString();

            return Redirect(uri);
        }

        public async Task<ActionResult> HandleOAuthRedirect(OAuthRedirectViewModel viewModel)
        {

            try
            {
                var accessToken = await _apiClient.GetAccessToken(
                    ConfigurationManager.AppSettings["OAuth2TokenEndpoint"],
                    viewModel.code,
                    ConfigurationManager.AppSettings["OAuth2RedirectEndpoint"]);

                _apiClient.SetBearerToken(accessToken.access_token);

                var user = await _apiClient.GetUserDetails();

                if (user == null || user.Id == null || user.Id == "")
                {
                    return RedirectToAction("Index", "Home").Error("Error while logging you in");
                }

                HttpContext.SaveIdentity(new Identity(user, accessToken));

                return RedirectToAction("Index", "Home").Success("Successfully logged in");

            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home").Error("Error while logging you in");
            }
        }

        #region Logout

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Request.SaveIdentity(new Identity());

            return RedirectToAction("Index", "Home").Success("Successfully logged you out");
        }

        #endregion
    }
}