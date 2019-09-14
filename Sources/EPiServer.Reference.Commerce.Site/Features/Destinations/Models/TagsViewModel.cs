using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Models
{
    public class TagsViewModel : ContentViewModel<TagPage>
    {
        public TagsViewModel(TagPage currentPage) : base(currentPage)
        {
        }

        public string Continent { get; set; }

        public string[] AdditionalCategories { get; set; }

        public CarouselViewModel Carousel { get; set; }

        public List<DestinationPage> Destinations { get; set; }
    }
}