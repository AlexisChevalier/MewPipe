﻿using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MewPipe.Accounts.Extensions;
using MewPipe.Accounts.ViewModels;
using MewPipe.Logic.Models;
using MewPipe.Logic.MongoDB;
using MewPipe.Logic.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace MewPipe.Accounts.Controllers
{
	[Authorize]
	public class ManageController : Controller
	{
		#region Initialization

		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public ManageController()
		{
		}

		public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		public ApplicationSignInManager SignInManager
		{
			get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
			private set { _signInManager = value; }
		}

		public ApplicationUserManager UserManager
		{
			get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
			private set { _userManager = value; }
		}

		#endregion

		#region Manage home

		//
		// GET: /Manage/Index
		public async Task<ActionResult> Index(ManageMessageId? message)
		{
			var userId = User.Identity.GetUserId();

			var user = await UserManager.FindByIdAsync(userId);

			var model = new IndexViewModel
			{
				HasPassword = HasPassword(),
				Logins = await UserManager.GetLoginsAsync(userId),
				User = user
			};
			return View(model);
		}

		#endregion

		#region Password

		//
		// GET: /Manage/ChangePassword
		public ActionResult ChangePassword()
		{
			return View();
		}

		//
		// POST: /Manage/ChangePassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user == null)
			{
				return RedirectToAction("Index").Error("An error occured please try again later.");
			}

			var isSamePass = await UserManager.CheckPasswordAsync(user, model.NewPassword);
			if (isSamePass)
			{
				ModelState.AddModelError("", "The new password is the same as the current one.");
				return View(model);
			}

			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				/* var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{*/
				await SignInManager.SignInAsync(user, false, false);
				/*}*/
				return RedirectToAction("Index").Success("Password successfully updated");
			}

			AddErrors(result);
			return View(model);
		}

		//
		// GET: /Manage/SetPassword
		public ActionResult SetPassword()
		{
			return View();
		}

		//
		// POST: /Manage/SetPassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
				if (result.Succeeded)
				{
					var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
					if (user != null)
					{
						await SignInManager.SignInAsync(user, false, false);
					}
					return RedirectToAction("Index").Success("Password successfully defined");
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		#endregion

        //THIS FEATURE HAS BEEN DISABLED FOR RELEASE - WE ENCOUNTERED AN UNEXPECTED PROBLEM WITH A VERSION OF ASP.NET OWIN IDENTITY
		#region External Logins

		//
		// POST: /Manage/RemoveLogin
		/*[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
		{
			var result =
				await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInManager.SignInAsync(user, false, false);
				}
				return RedirectToAction("ManageLogins").Success("External login successfully removed");
			}
			return RedirectToAction("ManageLogins").Error("Can't remove this external login at the moment");
		}

		//
		// GET: /Manage/ManageLogins
		public async Task<ActionResult> ManageLogins(ManageMessageId? message)
		{
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user == null)
			{
				return View("Error");
			}
			var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
			var otherLogins =
				AuthenticationManager.GetExternalAuthenticationTypes()
					.Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider))
					.ToList();
			ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
			return View(new ManageLoginsViewModel
			{
				CurrentLogins = userLogins,
				OtherLogins = otherLogins
			});
		}

		//
		// POST: /Manage/LinkLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LinkLogin(string provider)
		{
			// Request a redirect to the external login provider to link a login for the current user
			return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"),
				User.Identity.GetUserId());
		}

		//
		// GET: /Manage/LinkLoginCallback
		public async Task<ActionResult> LinkLoginCallback()
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
			if (loginInfo == null)
			{
				return RedirectToAction("ManageLogins", new {Message = ManageMessageId.Error});
			}
			var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);

			if (result.Succeeded)
			{
				return RedirectToAction("ManageLogins").Success("External login successfully added");
			}
			return
				RedirectToAction("ManageLogins")
					.Error("Can't add this external login, please allow MewPipe to interract with your account");
		}*/

		#endregion

		#region Delete Account

		//
		// GET: /Manage/DeleteAccount
		public ActionResult DeleteAccount()
		{
			return View();
		}

		//
		// POST: /Manage/DeleteAccount
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ProcessDeleteAccount()
		{
			//Récuperer l'utilisateur
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			var uow = new UnitOfWork();
			var videoGridFsClient = new VideoGridFsClient();
			var thumbnailGridFsClient = new ThumbnailGridFsClient();

			var extendedUser = uow.UserRepository.GetOne(u => u.Id == user.Id,
				"Videos, Impressions, Videos.VideoFiles, Videos.VideoFiles.MimeType, Videos.VideoFiles.QualityType");

			//Supprimer les vidéos
			foreach (var video in extendedUser.Videos.ToList())
			{
				uow.GetContext()
					.Database.ExecuteSqlCommand(string.Format("DELETE FROM Recommendations WHERE Video_Id ='{0}'", video.Id));

				video.Status = Video.StatusTypes.Processing;

				uow.VideoRepository.Update(video);
				uow.Save();


				//Supprimer les fichiers (Mongo + SQL Server)
				foreach (var videoFile in video.VideoFiles)
				{
					videoGridFsClient.RemoveFile(video, videoFile.MimeType, videoFile.QualityType);
				}


				//Suppression de la miniature
				thumbnailGridFsClient.RemoveFile(video);


				//Supprimer l'entité vidéo
				uow.VideoRepository.Delete(video);
				uow.Save();
			}

			//Supprimer les données utilisateur
			//Supprimer les likes ?


			uow.GetContext()
				.Database.ExecuteSqlCommand(string.Format("DELETE FROM Impressions WHERE User_Id ='{0}'", extendedUser.Id));
			uow.GetContext()
				.Database.ExecuteSqlCommand(string.Format("DELETE FROM OauthAccessTokens WHERE User_Id ='{0}'", extendedUser.Id));
			uow.GetContext()
				.Database.ExecuteSqlCommand(string.Format("DELETE FROM OauthAuthorizationCodes WHERE User_Id ='{0}'",
					extendedUser.Id));
			uow.GetContext()
				.Database.ExecuteSqlCommand(string.Format("DELETE FROM OauthRefreshTokens WHERE User_Id ='{0}'", extendedUser.Id));
			uow.GetContext()
				.Database.ExecuteSqlCommand(string.Format("DELETE FROM OauthUserTrusts WHERE User_Id ='{0}'", extendedUser.Id));

			uow.Save();

			await UserManager.DeleteAsync(user);

			//Déconnecter l'utilisateur
			AuthenticationManager.SignOut();

			//Rediriger utilisateur vers 
			return RedirectToAction("Register", "Account").Success("Your account has been successfully deleted !");
		}

		#endregion

		#region Helpers

		protected override void Dispose(bool disposing)
		{
			if (disposing && _userManager != null)
			{
				_userManager.Dispose();
				_userManager = null;
			}

			base.Dispose(disposing);
		}

		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get { return HttpContext.GetOwinContext().Authentication; }
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private bool HasPassword()
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user != null)
			{
				return user.PasswordHash != null;
			}
			return false;
		}


		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			Error
		}

		#endregion
	}
}