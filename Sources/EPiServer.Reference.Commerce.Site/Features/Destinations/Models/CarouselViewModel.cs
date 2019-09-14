using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Blocks;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Models
{
    public class CarouselViewModel
    {
        public List<CarouselItemBlock> Items { get; set; }

        public ContentReference BackgroundImage { get; set; }
    }
}