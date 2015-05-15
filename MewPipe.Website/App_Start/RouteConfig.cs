using System.Web.Mvc;
using System.Web.Routing;

namespace MewPipe.Website
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			/** Home **/
			routes.MapRoute("HomePage", "", new {controller = "Home", action = "Index"}
				);

			routes.MapRoute("AboutPage", "about", new {controller = "Home", action = "About"}
				);

			routes.MapRoute("TermsOfUsePage", "termsOfUse", new {controller = "Home", action = "TermsOfUse"}
				);

			routes.MapRoute("PrivacyPage", "privacy", new {controller = "Home", action = "Privacy"}
				);

			/** Videos **/

			routes.MapRoute("VideoPage", "v/{videoId}", new {controller = "Videos", action = "Index"}
				);

			routes.MapRoute("UserVideoUploadPage", "myVideos/upload", new {controller = "Videos", action = "UploadVideo"}
				);

			routes.MapRoute("UserVideoEditPage", "myVideos/edit/{videoId}",
				new {controller = "Videos", action = "EditVideo", videoId = UrlParameter.Optional}
				);

            routes.MapRoute("UserVideoDelete", "myVideos/delete/{videoId}",
                new { controller = "Videos", action = "DeleteVideo", videoId = UrlParameter.Optional }
                );

            routes.MapRoute("UserVideoWhiteListAdd", "myVideos/whiteList/add/{videoId}",
                new { controller = "Videos", action = "AddUserToVideoWhitelist", videoId = UrlParameter.Optional }
                );

            routes.MapRoute("UserVideoWhiteListDelete", "myVideos/whiteList/delete/{videoId}",
                new { controller = "Videos", action = "RemoveUserFromVideoWhitelist", videoId = UrlParameter.Optional }
                );

			routes.MapRoute("UserVideosPage", "myVideos", new {controller = "Videos", action = "UserVideos"}
				);

			/** Auth **/

			routes.MapRoute("LoginRedirect", "auth/login", new {controller = "Auth", action = "Login"}
				);

			routes.MapRoute("OAuthEndpoint", "auth/HandleOAuthRedirect",
				new {controller = "Auth", action = "HandleOAuthRedirect"}
				);

			routes.MapRoute("LogOff", "auth/LogOff", new {controller = "Auth", action = "LogOff"}
				);
		}
	}
}