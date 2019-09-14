using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Web.Routing;
using System.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class RequestContextExtensions
    {
        public static RouteValueDictionary GetPageRoute(this RequestContext requestContext, ContentReference contentLink)
        {
            var values = new RouteValueDictionary
            {
                [RoutingConstants.NodeKey] = contentLink,
                [RoutingConstants.LanguageKey] = ContentLanguage.PreferredCulture.Name
            };
            return values;
        }
    }
}