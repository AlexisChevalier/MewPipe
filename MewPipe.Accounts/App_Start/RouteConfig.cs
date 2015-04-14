using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace MewPipe.Accounts
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            /** Home **/

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

            /** Account **/

            routes.MapRoute(
                name: "Account",
                url: "Account/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );

            /** Manage **/

            routes.MapRoute(
                name: "Manage",
                url: "{action}/{id}",
                defaults: new { controller = "Manage", action = "Index", id = UrlParameter.Optional }
            );
            
        }
    }
}
