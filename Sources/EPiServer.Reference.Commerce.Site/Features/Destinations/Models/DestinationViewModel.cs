using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Models
{
    public class DestinationViewModel : ContentViewModel<DestinationPage>
    {
        public DestinationViewModel(DestinationPage currentPage)
            : base(currentPage)
        {
            DestinationNavigation = new DestinationNavigationModel
            {
                CurrentDestination = currentPage
            };
        }

        public ImageData Image { get; set; }

        public DestinationNavigationModel DestinationNavigation { get; set; }

        public IEnumerable<DestinationPage> SimilarDestinations { get; set; }
    }

    public class DestinationNavigationModel
    {
        public DestinationNavigationModel()
        {
            CloseBy = Enumerable.Empty<DestinationPage>();
            ContinentDestinations = Enumerable.Empty<DestinationPage>();
        }
        public IEnumerable<DestinationPage> CloseBy { get; set; }

        public IEnumerable<DestinationPage> ContinentDestinations { get; set; }

        public DestinationPage CurrentDestination { get; set; }

    }
}