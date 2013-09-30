using System;
using System.Web;

namespace MongoBlog.Web.Core
{
    public static class LanguageProvider
    {
        public static string GetUICurrentLanguageCode(HttpContextBase context)
        {
            var cookie = context.Request.Cookies["lang"];
            if (null == cookie)
            {
                SetUILanguageCode(context, "eng");
                return "eng";
            }
            return cookie.Value;
        }

        public static void SetUILanguageCode(HttpContextBase context, string lang)
        {
            var cookie = context.Request.Cookies["lang"] ?? new HttpCookie("lang");
            cookie.Value = lang;
            cookie.Expires = DateTime.Now.AddMonths(12);
            context.Response.Cookies.Add(cookie);
        }
    }
}