using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Full Width Carousel", GUID = "a63d6383-8512-42c4-829c-0ade4c681f0e",
        Description = "Carousel block that takes the full width of the screen.",
        GroupName = "Content")]
    [ImageUrl("~/content/icons/blocks/CMS-icon-block-07.png")]
    public class FullWidthCarousel : SiteBlockData
    {
        [Display(GroupName = SystemTabNames.Content, Order = 320)]
        [AllowedTypes(typeof(HeroBlock))]
        public virtual ContentArea MainContentArea { get; set; }
    }
}