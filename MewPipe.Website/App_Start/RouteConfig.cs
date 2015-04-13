using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            routes.MapRoute(
                name: "HomePage",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "AboutPage",
                url: "about",
                defaults: new { controller = "Home", action = "About" }
            );

            routes.MapRoute(
                name: "TermsOfUsePage",
                url: "termsOfUse",
                defaults: new { controller = "Home", action = "TermsOfUse" }
            );

            routes.MapRoute(
                name: "PrivacyPage",
                url: "privacy",
                defaults: new { controller = "Home", action = "Privacy" }
            );

            /** Videos **/

            routes.MapRoute(
                name: "VideoPage",
                url: "v/{videoId}",
                defaults: new { controller = "Videos", action = "Index" }
            );

            routes.MapRoute(
                name: "UserVideoUploadPage",
                url: "myVideos/upload",
                defaults: new { controller = "Videos", action = "UploadVideo" }
            );

            routes.MapRoute(
                name: "UserVideoEditPage",
                url: "myVideos/edit/{videoId}",
                defaults: new { controller = "Videos", action = "EditVideo" }
            );

            routes.MapRoute(
                name: "UserVideosPage",
                url: "myVideos",
                defaults: new { controller = "Videos", action = "UserVideos" }
            );

            /** Auth **/

            routes.MapRoute(
                name: "LoginRedirect",
                url: "auth/login",
                defaults: new { controller = "Auth", action = "Login" }
            );

            routes.MapRoute(
                name: "OAuthEndpoint",
                url: "auth/HandleOAuthRedirect",
                defaults: new { controller = "Auth", action = "HandleOAuthRedirect" }
            );

            routes.MapRoute(
                name: "LogOff",
                url: "auth/LogOff",
                defaults: new { controller = "Auth", action = "LogOff" }
            );
        }
    }
}
