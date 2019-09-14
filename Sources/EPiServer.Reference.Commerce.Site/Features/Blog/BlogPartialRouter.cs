using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System.Web;
using System.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Features.Blog
{
    public class BlogPartialRouter : IPartialRouter<BlogStartPage, Category>
    {

        public PartialRouteData GetPartialVirtualPath(Category cat, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            var contentLink = requestContext.GetRouteValue("node", routeValues) as ContentReference;

            if (PageEditing.PageIsInEditMode)
            {
                return null;
            }

            return new PartialRouteData
            {
                BasePathRoot = contentLink,
                PartialVirtualPath = $"{HttpUtility.UrlEncode(cat.Name)}/"
            };

        }


        public object RoutePartial(BlogStartPage content, EPiServer.Web.Routing.Segments.SegmentContext segmentContext)
        {
            //Expected format is Name/<otional>Header/
            var namePart = segmentContext.GetNextValue(segmentContext.RemainingPath);
            if (string.IsNullOrEmpty(namePart.Next))
            {
                return null;
            }
            var categoryName = HttpUtility.UrlDecode(namePart.Next);

            if (categoryName == null)
            {
                return null;
            }
            var remaingPath = namePart.Remaining;
            //Update RemainingPath on context.
            segmentContext.RemainingPath = remaingPath;
            var categoryRepository = ServiceLocator.Current.GetInstance<CategoryRepository>();
            var category = categoryRepository.Get(categoryName);
            segmentContext.RoutedContentLink = content.ContentLink;

            segmentContext.SetCustomRouteData("category", category);


            return content;
        }
    }
}