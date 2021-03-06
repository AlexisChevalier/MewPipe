﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MewPipe.Logic;
using MewPipe.Website.Extensions;
using MewPipe.Website.Security;

namespace MewPipe.Website
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		private void Application_Error(object sender, EventArgs e)
		{
			var exc = Server.GetLastError();

			if (exc is OauthExpiredTokenException)
			{
				HttpContext.Current.SaveIdentityFromContext(new Identity());
				FlashMessageExtensions.CreateCookieWithFlashMessage(
					FlashMessageExtensions.Notification.Warning,
					"You have been disconnected.");
				Server.ClearError();
				Response.Redirect("/", false);
				Context.ApplicationInstance.CompleteRequest();
			}
		}
	}
}