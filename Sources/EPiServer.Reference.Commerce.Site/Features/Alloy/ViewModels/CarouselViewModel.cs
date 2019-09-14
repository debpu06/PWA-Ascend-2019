using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels
{
    public class CarouselViewModel
    {
        public List<AlloyCarouselItemBlock> Items { get; set; }

        public CarouselBlock CurrentBlock { get; set; }
    }

}
