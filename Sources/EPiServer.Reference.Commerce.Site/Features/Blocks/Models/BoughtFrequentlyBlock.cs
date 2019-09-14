using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Bought Frequently Block", GUID = "bb4b4fd5-daaa-4f65-94ab-1b5013605d99", GroupName = "Commerce", Description = "A block that will show if you bought something frequently and give you a discount")]
    [ImageUrl("~/content/icons/blocks/CMS-icon-block-15.png")]
    public class BoughtFrequentlyBlock : BlockData
    {
        
        /// <summary>
        /// Configures the heading that should be used when displaying the block view in the frontend.
        /// </summary>
        [Display(
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
        [CultureSpecific]
        public virtual string Heading { get; set; }
    }
}