using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Models;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System.Collections.Specialized;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Blocks
{
    [ContentType(
        DisplayName = "Filter Continents Block", 
        GUID = "9103a763-4c9c-431e-bc11-f2794c3b4b80", 
        Description = "Continent facets for destinations", GroupName = "Destinations")]
    [ImageUrl("~/content/icons/blocks/map.png")]
    public class FilterContinentsBlock : BlockData, IFilterBlock
    {
        public virtual string FilterTitle { get; set; }

        public ITypeSearch<DestinationPage> AddFilter(ITypeSearch<DestinationPage> query)
        {
            return query.TermsFacetFor(x => x.Continent);
        }

        public ITypeSearch<DestinationPage> ApplyFilter(ITypeSearch<DestinationPage> query, NameValueCollection filters)
        {
            var filterString = filters["c"];
            if (!string.IsNullOrWhiteSpace(filterString))
            {
                var continents = filterString.Split(',').ToList();
                var continentsFilter = SearchClient.Instance.BuildFilter<DestinationPage>();
                continentsFilter = continents.Aggregate(continentsFilter,
                                                        (current, name) => current.Or(x => x.Continent.Match(name)));
                query = query.Filter(x => continentsFilter);
            }
            return query;
        }
    }
}