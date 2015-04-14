using System.Web;
using System.Web.Mvc;

namespace MewPipe.Accounts.Extensions
{
    /**
     * See https://gist.github.com/slashdotdash/d333bcf22ca167aa7f01
     */
    internal static class FlashMessageExtensions
    {
        public static ActionResult Error(this ActionResult result, string message)
        {
            CreateCookieWithFlashMessage(Notification.Danger, message);
            return result;
        }

        public static ActionResult Warning(this ActionResult result, string message)
        {
            CreateCookieWithFlashMessage(Notification.Warning, message);
            return result;
        }

        public static ActionResult Success(this ActionResult result, string message)
        {
            CreateCookieWithFlashMessage(Notification.Success, message);
            return result;
        }

        public static HttpContext Information(this HttpContext result, string message)
        {
            CreateCookieWithFlashMessage(Notification.Info, message);
            return result;
        }

        public static void SetErrorMessage(this HttpContext result, string message)
        {
            CreateCookieWithFlashMessage(Notification.Danger, message);
        }

        public static void SetWarningMessage(this HttpContext result, string message)
        {
            CreateCookieWithFlashMessage(Notification.Warning, message);
        }

        public static void SetSuccessMessage(this HttpContext result, string message)
        {
            CreateCookieWithFlashMessage(Notification.Success, message);
        }

        public static void SetInformationMessage(this ActionResult result, string message)
        {
            CreateCookieWithFlashMessage(Notification.Info, message);
        }

        private static void CreateCookieWithFlashMessage(Notification notification, string message)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(string.Format("Flash.{0}", notification), message)
            {
                Path = "/"
            });
        }

        private enum Notification
        {
            Danger,
            Warning,
            Success,
            Info
        }
    }
}