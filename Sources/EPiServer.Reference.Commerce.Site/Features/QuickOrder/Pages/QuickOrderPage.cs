using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.QuickOrder.Blocks;

namespace EPiServer.Reference.Commerce.Site.Features.QuickOrder.Pages
{
    [ContentType(DisplayName = "Quick Order Page", GUID = "9F846F7D-2DFA-4983-815D-C09B12CEF993", Description = "", AvailableInEditMode = false, GroupName = "Commerce")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-14.png")]
    public class QuickOrderPage : SitePageData
    {
        [CultureSpecific]
        [Display(
            Name = "Quick Order Block Content Area",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        [AllowedTypes(typeof(QuickOrderBlock))]
        public virtual ContentArea QuickOrderBlockContentArea { get; set; }
    }
}