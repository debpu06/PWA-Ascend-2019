using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Extensions
{
    public static class ContentExtensions
    {
        private static Injected<UrlResolver> _urlResolver = default(Injected<UrlResolver>);
        private static Injected<IRelationRepository> _relationRepository = default(Injected<IRelationRepository>);
        private static Injected<IContentLoader> _contentLoader = default(Injected<IContentLoader>);
        private static Injected<ReferenceConverter> _referenceConverter = default(Injected<ReferenceConverter>);
        private static readonly CookieService CookieService = new CookieService();

        private const int MaxHistory = 10;
        private const string Delimiter = "^!!^";

        public static IEnumerable<PageData> GetSiblings(this PageData pageData)
        {
            return GetSiblings(pageData, _contentLoader.Service);
        }

        public static IEnumerable<PageData> GetSiblings(this PageData pageData, IContentLoader contentLoader)
        {
            var filter = new FilterContentForVisitor();
            return contentLoader.GetChildren<PageData>(pageData.ParentLink).Where(page => !filter.ShouldFilter(page));
        }

        public static string GetUrl(this EntryContentBase entry)
        {
            return GetUrl(entry, _relationRepository.Service, _urlResolver.Service);
        }

        public static string GetUrl(this EntryContentBase entry, IRelationRepository linksRepository, UrlResolver urlResolver)
        {
            var productLink = entry is VariationContent ?
                entry.GetParentProducts(linksRepository).FirstOrDefault() : 
                entry.ContentLink;

            if (productLink == null)
            {
                return string.Empty;
            }

            var urlBuilder = new UrlBuilder(urlResolver.GetUrl(productLink));

            if (entry.Code != null && entry is VariationContent)
            {
                urlBuilder.QueryCollection.Add("variationCode", entry.Code);
            }
            
            return urlBuilder.ToString();
        }

        public static void AddBrowseHistory(this EntryContentBase entry)
        {
            
            var history = CookieService.Get("BrowseHistory");
            var values = string.IsNullOrEmpty(history) ? new List<string>() : 
                history.Split(new [] { Delimiter }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (values.Contains(entry.Code))
            {
                return;
            }
                                                                                                                                                                                                                                 
            if (values.Any())
            {
                if (values.Count == MaxHistory)
                {
                    values.RemoveAt(0);
                }
            }

            values.Add(entry.Code);

            CookieService.Set("BrowseHistory", string.Join(Delimiter, values));
        }

        public static IList<EntryContentBase> GetBrowseHistory()
        {
            var entryCodes = CookieService.Get("BrowseHistory");
            if (string.IsNullOrEmpty(entryCodes))
            {
                return new List<EntryContentBase>();
            }

            var contentLinks = _referenceConverter.Service.GetContentLinks(entryCodes.Split(new[]
            {
                Delimiter
            }, StringSplitOptions.RemoveEmptyEntries));


            return _contentLoader.Service.GetItems(contentLinks.Select(x => x.Value), new LoaderOptions())
                .OfType<EntryContentBase>()
                .ToList();
        }

        public static void AddPageBrowseHistory(this PageData page)
        {

            var history = CookieService.Get("PageBrowseHistory");
            var values = string.IsNullOrEmpty(history) ? new List<int>() :
                history.Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToList();

            if (values.Contains(page.ContentLink.ID))
            {
                return;
            }

            if (values.Any())
            {
                if (values.Count == 2)
                {
                    values.RemoveAt(0);
                }
            }

            values.Add(page.ContentLink.ID);

            CookieService.Set("PageBrowseHistory", string.Join(Delimiter, values));
        }

        public static IList<PageData> GetPageBrowseHistory()
        {
            var pageIds = CookieService.Get("PageBrowseHistory");
            if (string.IsNullOrEmpty(pageIds))
            {
                return new List<PageData>();
            }

            var contentLinks = pageIds.Split(new[]
            {
                Delimiter
            }, StringSplitOptions.RemoveEmptyEntries).Select(x => new ContentReference(x));
            return _contentLoader.Service.GetItems(contentLinks, new LoaderOptions())
                .OfType<PageData>()
                .ToList();
        }
    }
}