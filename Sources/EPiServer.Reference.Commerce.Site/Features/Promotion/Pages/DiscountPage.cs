using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.Pages
{
    [ContentType(DisplayName = "DiscountPage", GUID = "d78cf1ea-4d94-4d5d-8b4f-365e2814e631", Description = "Discount Page to display products belonging to a promotion", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/CMS-icon-page-08.png")]
    public class DiscountPage : SitePageData
    {
        [CultureSpecific]
        [Display(Name = "Main content area", Description = "", GroupName = SystemTabNames.Content, Order = 100)]
        public virtual ContentArea MainContentArea { get; set; }
    }
}