using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Pages
{
    [ContentType(DisplayName = "CreditCardPage", GUID = "adad362c-4f73-4592-abb9-093f6e7bb7c6", Description = "", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/CMS-icon-page-14.png")]
    public class CreditCardPage : SitePageData
    {
        [Display(GroupName = SystemTabNames.Content, Order = 200)]
        [CultureSpecific]
        public virtual bool B2B { get; set; }
    }
}