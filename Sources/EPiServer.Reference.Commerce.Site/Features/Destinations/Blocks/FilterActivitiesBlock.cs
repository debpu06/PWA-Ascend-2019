using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Models;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Blocks
{
    [ContentType(
        DisplayName = "Filter Activities Block", 
        GUID = "918c590e-b2cd-4b87-9116-899b1db19117", 
        Description = "Activity facets for destinations", GroupName = "Destinations")]
    [ImageUrl("~/content/icons/blocks/map.png")]
    public class FilterActivitiesBlock : BlockData, IFilterBlock
    {
        public virtual string FilterTitle { get; set; }

        public ITypeSearch<DestinationPage> AddFilter(ITypeSearch<DestinationPage> query)
        {
            return query.TermsFacetFor(x => x.TagString(), facet => facet.Size = 25);
        }

        public ITypeSearch<DestinationPage> ApplyFilter(ITypeSearch<DestinationPage> query, NameValueCollection filters)
        {
            var filterString = filters["a"];
            if (!string.IsNullOrWhiteSpace(filterString))
            {
                var activities = filters["a"].Split(',').ToList();
                var activitiesFilter = SearchClient.Instance.BuildFilter<DestinationPage>();
                activitiesFilter = activities.Aggregate(activitiesFilter,
                                                        (current, name) =>
                                                        current.Or(
                                                            x => x.TagString().Match(HttpUtility.UrlDecode(name))));
                query = query.Filter(x => activitiesFilter);
            }
            return query;
        }
    }
}