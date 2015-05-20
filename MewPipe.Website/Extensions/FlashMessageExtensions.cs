﻿using System.Web;
using System.Web.Mvc;

namespace MewPipe.Website.Extensions
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

        public static ActionResult Information(this ActionResult result, string message)
        {
            CreateCookieWithFlashMessage(Notification.Info, message);
            return result;
        }

        public static void CreateCookieWithFlashMessage(Notification notification, string message)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(string.Format("Flash.{0}", notification), message)
            {
                Path = "/"
            });
        }

        public enum Notification
        {
            Danger,
            Warning,
            Success,
            Info
        }
    }
}