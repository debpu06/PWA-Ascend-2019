using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Container Block", GUID = "8bdfac81-1dbd-43b9-a012-522bd67ee8b3", Description = "")]
    [ImageUrl("~/content/icons/blocks/CMS-icon-block-04.png")]
    public class ContainerBlock : SiteBlockData
    {
        public virtual ContentArea MainContentArea { get; set; }

        public virtual string WrappingClass { get; set; }
    }
}