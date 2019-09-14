using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class UrlHelpers
    {
        public static RouteValueDictionary ContentRoute(this UrlHelper urlHelper,
            ContentReference contentLink,
            object routeValues = null)
        {
            var first = new RouteValueDictionary(routeValues);

            var values = first.Union(urlHelper.RequestContext.RouteData.Values);

            values[RoutingConstants.ActionKey] = "index";
            values[RoutingConstants.NodeKey] = contentLink;
            return values;
        }

        /// <summary>
        ///     Returns the target URL for a PageReference. Respects the page's shortcut setting
        ///     so if the page is set as a shortcut to another page or an external URL that URL
        ///     will be returned.
        /// </summary>
        public static IHtmlString PageLinkUrl(this UrlHelper urlHelper,
            ContentReference pageLink)
        {
            if (ContentReference.IsNullOrEmpty(pageLink))
                return MvcHtmlString.Empty;

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var page = contentLoader.Get<PageData>(pageLink);

            return PageLinkUrl(urlHelper, page);
        }

        /// <summary>
        ///     Returns the target URL for a page. Respects the page's shortcut setting
        ///     so if the page is set as a shortcut to another page or an external URL that URL
        ///     will be returned.
        /// </summary>
        public static IHtmlString PageLinkUrl(this UrlHelper urlHelper,
            PageData page)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            switch (page.LinkType)
            {
                case PageShortcutType.Normal:
                case PageShortcutType.FetchData:
                    return new MvcHtmlString(urlResolver.GetUrl(page.PageLink));

                case PageShortcutType.Shortcut:
                    var shortcutProperty = page.Property["PageShortcutLink"] as PropertyPageReference;
                    if (shortcutProperty != null && !ContentReference.IsNullOrEmpty(shortcutProperty.PageLink))
                        return urlHelper.PageLinkUrl(shortcutProperty.PageLink);
                    break;

                case PageShortcutType.External:
                    return new MvcHtmlString(page.LinkURL);
            }
            return MvcHtmlString.Empty;
        }

        public static IHtmlString GetSegmentedUrl(this UrlHelper urlHelper,
            PageData currentPage,
            params string[] Segments)
        {
            var url = urlHelper.PageLinkUrl(currentPage).
                ToString();

            if (!url.EndsWith("/"))
                url = url + '/';
            url += string.Join("/", Segments);
            //TODO: Url-encode segments

            return new HtmlString(url);
        }

        public static IHtmlString ImageExternalUrl(this UrlHelper urlHelper,
            ImageData image)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            return new MvcHtmlString(urlResolver.GetUrl(image.ContentLink));
        }

        public static IHtmlString ImageExternalUrl(this UrlHelper urlHelper,
            ImageData image,
            string variant)
        {
            return urlHelper.ImageExternalUrl(image.ContentLink, variant);
        }

        public static IHtmlString ImageExternalUrl(this UrlHelper urlHelper,
            Uri imageUri,
            string variant)
        {
            return new MvcHtmlString(
                string.IsNullOrWhiteSpace(variant) ? imageUri.ToString() : imageUri + "/" + variant);
        }

        public static IHtmlString ImageExternalUrl(this UrlHelper urlHelper,
            ContentReference imageref,
            string variant)
        {
            if (ContentReference.IsNullOrEmpty(imageref))
                return MvcHtmlString.Empty;

            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            var url = urlResolver.GetUrl(imageref);
            //Inject variant
            if (!string.IsNullOrEmpty(variant))
                if (url.Contains("?"))
                    url = url.Insert(url.IndexOf('?'), "/" + variant);
                else
                    url = url + "/" + variant;
            return new MvcHtmlString(url);
        }

        public static IHtmlString CampaignUrl(this UrlHelper urlHelper,
            IHtmlString url,
            string campaign)
        {
            var s = url.ToString();
            if (s.Contains("?"))
                return new MvcHtmlString(s + "&utm_campaign=" + HttpContext.Current.Server.UrlEncode(campaign));
            return new MvcHtmlString(s + "?utm_campaign=" + HttpContext.Current.Server.UrlEncode(campaign));
        }

        private static RouteValueDictionary Union(this RouteValueDictionary first,
            RouteValueDictionary second)
        {
            var dictionary = new RouteValueDictionary(second);
            foreach (var pair in first)
                if (pair.Value != null)
                    dictionary[pair.Key] = pair.Value;

            return dictionary;
        }

        public static IHtmlString EmployeeLocationUrl(this UrlHelper urlHelper, string location)
        {
            return WriteShortenedUrl(EmployeeLocationRootUrl(urlHelper).ToString(), location);
        }

        /// <summary>
        /// Returns the Root Page Link Url for the Employee Location Page
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <returns></returns>
        public static IHtmlString EmployeeLocationRootUrl(this UrlHelper urlHelper)
        {
            if (string.IsNullOrEmpty(_locationRootUrl))
            {
                lock (_syncObject)
                {
                    var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
                    var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

                    ContentReference locationRoot = (contentLoader.Get<BaseStartPage>(ContentReference.StartPage) as AlloyStartPage)?.EmployeeLocationPageLink ?? ContentReference.EmptyReference;
                    if (!ContentReference.IsNullOrEmpty(locationRoot))
                    {
                        _locationRootUrl = urlResolver.GetUrl(locationRoot);
                    }
                }
            }

            return new MvcHtmlString(_locationRootUrl);
        }

        /// <summary>
        /// Returns the target URL for an Employee expertise. 
        /// </summary>
        public static IHtmlString EmployeeExpertiseUrl(this UrlHelper urlHelper, string expertise)
        {
            return WriteShortenedUrl(EmployeeExpertiseRootUrl(urlHelper).ToString(), expertise);
        }

        /// <summary>
        /// Returns the Root Page Link Url for the Employee Expertise Page
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <returns></returns>
        public static IHtmlString EmployeeExpertiseRootUrl(this UrlHelper urlHelper)
        {
            if (string.IsNullOrEmpty(_expertiseRootUrl))
            {
                lock (_syncObject)
                {
                    var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
                    var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

                    ContentReference expertiseRoot = (contentLoader.Get<BaseStartPage>(ContentReference.StartPage) as AlloyStartPage)?.EmployeeExpertiseLink ?? ContentReference.EmptyReference;
                    if (!ContentReference.IsNullOrEmpty(expertiseRoot))
                    {
                        _expertiseRootUrl = urlResolver.GetUrl(expertiseRoot);
                    }
                }
            }

            return new MvcHtmlString(_expertiseRootUrl);
        }

        private static volatile object _syncObject = new object();
        private static string _expertiseRootUrl = string.Empty;
        private static string _locationRootUrl = string.Empty;

        private static IHtmlString WriteShortenedUrl(string root, string segment)
        {
            string fullUrlPath = string.Format("{0}{1}/", root, segment.ToLower().Replace(" ", "-"));

            return new MvcHtmlString(fullUrlPath);
        }
    }
}