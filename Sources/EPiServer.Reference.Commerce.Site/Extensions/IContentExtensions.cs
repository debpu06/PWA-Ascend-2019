using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.Ratings;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Filters;
using EPiServer.Framework.Web;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class IContentExtensions
    {
        private static Lazy<IPageRatingRepository> _socialRatingRepository = new Lazy<IPageRatingRepository>(() => ServiceLocator.Current.GetInstance<IPageRatingRepository>());

        public static double GetRatingAverage(this IContent content)
        {
            return _socialRatingRepository.Value.GetRatingStatistics(content.ContentGuid.ToString())?.Average ?? 0;
        }

        public static IEnumerable<T> FilterForDisplay<T>(this IEnumerable<T> contents, bool requirePageTemplate = false, bool requireVisibleInMenu = false)
            where T : IContent
        {
            var accessFilter = new FilterAccess();
            var publishedFilter = new FilterPublished();
            contents = contents.Where(x => !publishedFilter.ShouldFilter(x) && !accessFilter.ShouldFilter(x));
            if (requirePageTemplate)
            {
                var templateFilter = ServiceLocator.Current.GetInstance<FilterTemplate>();
                templateFilter.TemplateTypeCategories = TemplateTypeCategories.Page;
                contents = contents.Where(x => !templateFilter.ShouldFilter(x));
            }
            if (requireVisibleInMenu)
            {
                contents = contents.Where(x => VisibleInMenu(x));
            }
            return contents;
        }

        private static bool VisibleInMenu(IContent content)
        {
            var page = content as PageData;
            return page == null || page.VisibleInMenu;
        }
    }
}