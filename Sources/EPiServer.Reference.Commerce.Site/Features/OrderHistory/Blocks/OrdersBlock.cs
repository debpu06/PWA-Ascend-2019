using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.OrderHistory.Pages
{
    [ContentType(DisplayName = "Order history block", GUID = "6b910185-7270-43bf-90e5-fc57cc0d1b5c", Description = "", AvailableInEditMode = true)]
    [SiteImageUrl("~/content/icons/blocks/CMS-icon-block-18.png")]
    public class OrderHistoryBlock : BlockData
    {
        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString MainBody { get; set; }
    }
}