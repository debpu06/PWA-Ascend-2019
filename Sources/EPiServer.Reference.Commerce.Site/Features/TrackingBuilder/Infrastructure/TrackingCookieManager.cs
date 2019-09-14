using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Infrastructure
{
    public static class TrackingCookieManager
    {
        public static string TrackingCookieName = "_madid";
        public static string GetTrackingCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[TrackingCookieName];
            return cookie == null ? string.Empty : cookie.Value;
        }

        public static void SetTrackingCookie(string value)
        {
            if (HttpContext.Current.Request.Cookies[TrackingCookieName] == null)
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(TrackingCookieName));
            }
            HttpContext.Current.Response.Cookies[TrackingCookieName].Value = value;
        }
    }
}