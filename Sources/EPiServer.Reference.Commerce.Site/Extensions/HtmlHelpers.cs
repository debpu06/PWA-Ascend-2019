using EPiServer.Core;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using EPiServer.Web.Mvc.Html;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class HtmlHelpers
    {
        private static readonly Lazy<IContentLoader> ContentLoader = new Lazy<IContentLoader>(() => ServiceLocator.Current.GetInstance<IContentLoader>());
        private const string CssFormat = "<link href=\"{0}\" rel=\"stylesheet\" />";
        private const string ScriptFormat = "<script src=\"{0}\"></script>";
        private const string MetaFormat = "<meta property=\"{0}\" content=\"{1}\" />";

        public static MvcHtmlString RenderExtendedCss(this HtmlHelper helper, IContent content)
        {
            if (content == null)
            {
                return new MvcHtmlString("");
            }
            var sitePageData = content as SitePageData;
            if (sitePageData == null)
            {
                return new MvcHtmlString("");
            }

            var outputCss = new StringBuilder(string.Empty);
            var start = ServiceLocator.Current.GetInstance<IContentLoader>().Get<BaseStartPage>(ContentReference.StartPage);

            if ((sitePageData.CssFiles == null || sitePageData.CssFiles.Count == 0) && start.CssFiles != null)
            {
                AppendFiles(start.CssFiles, outputCss, CssFormat);
            }
            AppendFiles(sitePageData.CssFiles, outputCss, CssFormat);

            // Inlined CSS & Google Font
            outputCss.AppendLine("<style>");
            outputCss.AppendLine(!string.IsNullOrWhiteSpace(sitePageData.Css) ? sitePageData.Css :
                !string.IsNullOrWhiteSpace(start.Css) ? start.Css : "");
            outputCss.AppendLine("</style>");

            return new MvcHtmlString(outputCss.ToString());
        }

        public static MvcHtmlString RenderExtendedScripts(this HtmlHelper helper, IContent content)
        {
            if (content == null)
            {
                return new MvcHtmlString("");
            }
            var output = new StringBuilder(string.Empty);
            var sitePageData = content as SitePageData;
            if (sitePageData == null)
            {
                return new MvcHtmlString("");
            }

            AppendFiles(sitePageData.ScriptFiles, output, ScriptFormat);

            if (string.IsNullOrWhiteSpace(sitePageData.Scripts))
            {
                return new MvcHtmlString(output.ToString());
            }
            output.AppendLine("<script type=\"text/javascript\">");
            output.AppendLine(sitePageData.Scripts);
            output.AppendLine("</script>");


            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString RenderMetaData(this HtmlHelper helper, IContent content)
        {
            if (content == null)
            {
                return new MvcHtmlString("");
            }
            var output = new StringBuilder(string.Empty);
            var sitePageData = content as SitePageData;
            if (sitePageData == null)
            {
                return new MvcHtmlString("");
            }

            if (!string.IsNullOrWhiteSpace(sitePageData.MetaTitle))
            {
                output.AppendLine(string.Format(MetaFormat, "title", sitePageData.MetaTitle));
            }

            if (!string.IsNullOrEmpty(sitePageData.Keyword))
            {
                output.AppendLine(string.Format(MetaFormat, "keywords", sitePageData.Keyword));
            }

            if (!string.IsNullOrWhiteSpace(sitePageData.MetaDescription))
            {
                output.AppendLine(string.Format(MetaFormat, "description", sitePageData.MetaDescription));
            }

            if (sitePageData.DisableIndexing)
            {
                output.AppendLine("<meta name=\"robots\" content=\"NOINDEX, NOFOLLOW\">");
            }

            return new MvcHtmlString(output.ToString());
        }

        public static ContentReference GetSearchPage(this HtmlHelper helper)
        {
            return ContentLoader.Value.Get<BaseStartPage>(ContentReference.StartPage).SearchPage;
        }

        private static void AppendFiles(LinkItemCollection files, StringBuilder outputString, string formatString)
        {
            if (files == null || files.Count <= 0) return;

            foreach (var item in files.Where(item => !string.IsNullOrEmpty(item.Href)))
            {
                IPermanentLinkMapper mapper = ServiceLocator.Current.GetInstance<IPermanentLinkMapper>();
                var map = mapper.Find(new UrlBuilder(item.Href));
                outputString.AppendLine(map == null
                    ? string.Format(formatString, item.GetMappedHref())
                    : string.Format(formatString, UrlResolver.Current.GetUrl(map.ContentReference)));
            }
        }

        public static ConditionalLink BeginConditionalLink(this HtmlHelper helper, bool shouldWriteLink, IHtmlString url, string title = null, string cssClass = null)
        {
            if (shouldWriteLink)
            {
                var linkTag = new TagBuilder("a");
                linkTag.Attributes.Add("href", url.ToHtmlString());

                if (!string.IsNullOrWhiteSpace(title))
                {
                    linkTag.Attributes.Add("title", helper.Encode(title));
                }

                if (!string.IsNullOrWhiteSpace(cssClass))
                {
                    linkTag.Attributes.Add("class", cssClass);
                }

                helper.ViewContext.Writer.Write(linkTag.ToString(TagRenderMode.StartTag));
            }
            return new ConditionalLink(helper.ViewContext, shouldWriteLink);
        }

        /// <summary>
        /// Writes an opening <![CDATA[ <a> ]]> tag to the response if the shouldWriteLink argument is true.
        /// Returns a ConditionalLink object which when disposed will write a closing <![CDATA[ </a> ]]> tag
        /// to the response if the shouldWriteLink argument is true.
        /// </summary>
        /// <remarks>
        /// Overload which only executes the delegate for retrieving the URL if the link should be written.
        /// This may be used to prevent null reference exceptions by adding null checkes to the shouldWriteLink condition.
        /// </remarks>
        public static ConditionalLink BeginConditionalLink(this HtmlHelper helper, bool shouldWriteLink, Func<IHtmlString> urlGetter, string title = null, string cssClass = null)
        {
            IHtmlString url = MvcHtmlString.Empty;

            if (shouldWriteLink)
            {
                url = urlGetter();
            }

            return helper.BeginConditionalLink(shouldWriteLink, url, title, cssClass);
        }

        public class ConditionalLink : IDisposable
        {
            private readonly ViewContext _viewContext;
            private readonly bool _linked;
            private bool _disposed;

            public ConditionalLink(ViewContext viewContext, bool isLinked)
            {
                _viewContext = viewContext;
                _linked = isLinked;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);

            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                if (_linked)
                {
                    _viewContext.Writer.Write("</a>");
                }
            }
        }

        public static IHtmlString MenuList(
            this HtmlHelper helper,
            ContentReference rootLink,
            Func<MenuItem, HelperResult> itemTemplate = null,
            bool includeRoot = false,
            bool requireVisibleInMenu = true,
            bool requirePageTemplate = true)
        {
            itemTemplate = itemTemplate ?? GetDefaultItemTemplate(helper);
            var currentContentLink = helper.ViewContext.RequestContext.GetContentLink();
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

            Func<IEnumerable<PageData>, IEnumerable<PageData>> filter =
                pages => pages.FilterForDisplay(requirePageTemplate, requireVisibleInMenu);

            var pagePath = contentLoader.GetAncestors(currentContentLink)
                .Reverse()
                .Select(x => x.ContentLink)
                .SkipWhile(x => !x.CompareToIgnoreWorkID(rootLink))
                .ToList();

            var menuItems = contentLoader.GetChildren<PageData>(rootLink)
                .FilterForDisplay(requirePageTemplate, requireVisibleInMenu)
                .Select(x => CreateMenuItem(x, currentContentLink, pagePath, contentLoader, filter))
                .ToList();

            if (includeRoot)
            {
                menuItems.Insert(0, CreateMenuItem(contentLoader.Get<PageData>(rootLink), currentContentLink, pagePath, contentLoader, filter));
            }

            var buffer = new StringBuilder();
            var writer = new StringWriter(buffer);
            foreach (var menuItem in menuItems)
            {
                itemTemplate(menuItem).WriteTo(writer);
            }

            return new MvcHtmlString(buffer.ToString());
        }

        private static MenuItem CreateMenuItem(PageData page, ContentReference currentContentLink, List<ContentReference> pagePath, IContentLoader contentLoader, Func<IEnumerable<PageData>, IEnumerable<PageData>> filter)
        {
            var menuItem = new MenuItem(page)
            {
                Selected = page.ContentLink.CompareToIgnoreWorkID(currentContentLink) ||
                               pagePath.Contains(page.ContentLink),
                HasChildren =
                        new Lazy<bool>(() => filter(contentLoader.GetChildren<PageData>(page.ContentLink)).Any())
            };
            return menuItem;
        }

        private static Func<MenuItem, HelperResult> GetDefaultItemTemplate(HtmlHelper helper)
        {
            return x => new HelperResult(writer => writer.Write(helper.PageLink(x.Page)));
        }

        public class MenuItem
        {
            public MenuItem(PageData page)
            {
                Page = page;
            }
            public PageData Page { get; set; }
            public bool Selected { get; set; }
            public Lazy<bool> HasChildren { get; set; }
        }
    }
}
