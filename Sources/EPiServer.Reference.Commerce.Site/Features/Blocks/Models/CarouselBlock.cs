using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Alloy;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{

    [SiteContentType(
        GroupName = "Specialized",
        GUID = "532165C2-EBE6-43ef-B329-A24EA8C37E2A", DisplayName = "Slide Show Block", Description = "Slide Show with items of type item for slide show")]
    [SiteImageUrl]
    public class CarouselBlock : SiteBlockData
    {

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        [AllowedTypes(new[] { typeof(AlloyCarouselItemBlock) })]
        public virtual ContentArea MainContentArea { get; set; }

    }
}