using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Pages
{
    [ContentType(
        DisplayName = "Destinations Overview",
        GroupName = "Destinations",
        GUID = "597afd14-391b-4e99-8e4f-8827e3e82354",
        Description = "Used to display a list of all destinations")]
    [AvailableContentTypes(
        Availability = Availability.Specific,
        Include = new[] { typeof(DestinationPage) })]
    [ImageUrl("~/content/icons/pages/cms-icon-page-27.png")]
    public class DestinationsPage : SitePageData
    {
        public virtual ContentArea FilterArea { get; set; }
    }
}