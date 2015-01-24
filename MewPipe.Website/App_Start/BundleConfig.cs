using System.Web;
using System.Web.Optimization;

namespace MewPipe.Website
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts").Include(
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/jquery.validate*",
                    "~/Scripts/modernizr-*",
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/respond.js",
                    "~/Scripts/jquery.cookie.js",
                    "~/Scripts/jquery.flashMessage.js"
                    ));

            bundles.Add(new StyleBundle("~/styles").Include(
                      "~/Content/Libs/bootstrap.css",
                      "~/Content/Libs/bootstrap-social.css",
                      "~/Content/Libs/font-awesome.css",
                      "~/Content/Main.css"));
        }
    }
}
