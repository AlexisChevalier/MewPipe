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

            bundles.Add(new ScriptBundle("~/scripts/upload").Include(
                    "~/Scripts/uploader.js",
                    "~/Scripts/Site/upload.js"
                    ));

            bundles.Add(new ScriptBundle("~/scripts/player").Include(
                    "~/Scripts/videojs.js",
                    "~/Scripts/video-js-resolutions.js",
                    "~/Scripts/Site/player.js"
                    ));

            bundles.Add(new StyleBundle("~/styles").Include(
                      "~/Content/Libs/bootstrap.css",
                      "~/Content/Libs/bootstrap-social.css",
                      "~/Content/Libs/font-awesome.css",
                      "~/Content/Main.css"));

            bundles.Add(new StyleBundle("~/styles/upload").Include(
                      "~/Content/UploadVideo.css"));

            bundles.Add(new StyleBundle("~/styles/editVideo").Include(
                      "~/Content/EditVideo.css"));

            bundles.Add(new StyleBundle("~/styles/player").Include(
                      "~/Content/Libs/videojs.css",
                      "~/Content/Libs/video-js-resolutions.css",
                      "~/Content/Player.css"));

            bundles.Add(new StyleBundle("~/styles/search").Include(
                      "~/Content/Search.css"));

            bundles.Add(new StyleBundle("~/styles/myvideos").Include(
                      "~/Content/MyVideos.css"));
        }
    }
}
