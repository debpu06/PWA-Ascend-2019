using EPiServer.Find.Cms;
using EPiServer.Personalization;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Specialized;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Models
{
    public class DestinationsViewModel : ContentViewModel<DestinationsPage>
    {
        public DestinationsViewModel(DestinationsPage currentPage) : base(currentPage) { }

        public GeoCoordinate MapCenter { get; set; }
        public IGeolocationResult UserLocation { get; set; }
        public IContentResult<DestinationPage> Destinations { get; set; }
        public NameValueCollection QueryString { get; set; }
    }
}