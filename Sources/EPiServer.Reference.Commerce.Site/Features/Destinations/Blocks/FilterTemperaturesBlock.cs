using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Find;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Models;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Blocks
{
    [ContentType(
        DisplayName = "Filter Temperatures Block", 
        GUID = "28629b4b-9475-4c44-9c15-31961391f166",
        Description = "Temperature slider for destinations", GroupName = "Destinations")]
    [ImageUrl("~/content/icons/blocks/map.png")]
    public class FilterTemperaturesBlock : BlockData, IFilterBlock
    {
        public virtual string FilterTitle { get; set; }

        public ITypeSearch<DestinationPage> AddFilter(ITypeSearch<DestinationPage> query)
        {
            return query;
        }

        public ITypeSearch<DestinationPage> ApplyFilter(ITypeSearch<DestinationPage> query, NameValueCollection filters)
        {
            var filterString = filters["t"];
            if (!string.IsNullOrWhiteSpace(filterString))
            {
                int f, t;
                var temperatures = filterString.Split(',').ToList();
                if (Int32.TryParse(temperatures.First(), out f) && Int32.TryParse(temperatures.Last(), out t) && f <= t && f >= -20 && t <= 40)
                {
                    query = query.Filter(x => x.AvgTempDbl.InRange(f, t));
                }
            }
            return query;
        }
    }
}