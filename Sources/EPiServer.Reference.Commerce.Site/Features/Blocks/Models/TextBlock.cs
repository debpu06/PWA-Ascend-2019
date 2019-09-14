using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Text Block", GUID = "32782B29-278B-410A-A402-9FF46FAF32B9", Description = "Simple Rich Text Block", GroupName ="Content")]
    [ImageUrl("~/content/icons/blocks/CMS-icon-block-03.png")]
    public class TextBlock : SiteBlockData
    {
        [CultureSpecific]
        public virtual XhtmlString Text { get; set; }
    }
}