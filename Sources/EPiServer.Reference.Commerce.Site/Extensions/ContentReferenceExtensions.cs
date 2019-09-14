using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class ContentReferenceExtensions
    {
        private static readonly Lazy<IContentLoader> ContentLoader = new Lazy<IContentLoader>(() => ServiceLocator.Current.GetInstance<IContentLoader>());
        private static readonly Lazy<IContentProviderManager> ProviderManager = new Lazy<IContentProviderManager>(() => ServiceLocator.Current.GetInstance<IContentProviderManager>());
        private static readonly Lazy<IPageCriteriaQueryService> PageCriteriaQueryService = new Lazy<IPageCriteriaQueryService>(() => ServiceLocator.Current.GetInstance<IPageCriteriaQueryService>());

        public static IContent Get<TContent>(this ContentReference contentLink) where TContent : IContent
        {
            return ContentLoader.Value.Get<TContent>(contentLink);
        }

        public static IEnumerable<T> GetAll<T>(this ContentReference rootLink) where T : PageData
        {
            var children = ContentLoader.Value.GetChildren<PageData>(rootLink);
            foreach (var child in children)
            {
                var childOfRequestedTyped = child as T;
                if (childOfRequestedTyped != null)
                {
                    yield return childOfRequestedTyped;
                }
                foreach (var descendant in GetAll<T>(child.ContentLink))
                {
                    yield return descendant;
                }
            }
        }

        public static IEnumerable<PageData> FindPagesByPageType(this ContentReference pageLink, bool recursive, int pageTypeId)
        {
            if (ContentReference.IsNullOrEmpty(pageLink))
            {
                throw new ArgumentNullException("pageLink", "No page link specified, unable to find pages");
            }

            var pages = recursive
                ? FindPagesByPageTypeRecursively(pageLink, pageTypeId)
                : ContentLoader.Value.GetChildren<PageData>(pageLink);

            return pages;
        }

        private static IEnumerable<PageData> FindPagesByPageTypeRecursively(ContentReference pageLink, int pageTypeId)
        {
            var criteria = new PropertyCriteriaCollection
            {
                new PropertyCriteria
                {
                    Name = "PageTypeID",
                    Type = PropertyDataType.PageType,
                    Condition = CompareCondition.Equal,
                    Value = pageTypeId.ToString(CultureInfo.InvariantCulture)
                }
            };

            if (!ProviderManager.Value.ProviderMap.CustomProvidersExist)
            {
                return PageCriteriaQueryService.Value.FindPagesWithCriteria(pageLink.ToPageReference(), criteria);
            }

            var contentProvider = ProviderManager.Value.ProviderMap.GetProvider(pageLink);
            if (contentProvider.HasCapability(ContentProviderCapabilities.Search))
            {
                criteria.Add(new PropertyCriteria
                {
                    Name = "EPI:MultipleSearch",
                    Value = contentProvider.ProviderKey
                });
            }

            return PageCriteriaQueryService.Value.FindPagesWithCriteria(pageLink.ToPageReference(), criteria);
        }

    }
}