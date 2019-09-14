using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.Pages
{
    [ContentType(DisplayName = "PaymentPlanDetailPage", GUID = "8eaf6fe8-3bf3-4f54-9b4a-06a1569087e1", Description = "", AvailableInEditMode = false)]
    [SiteImageUrl("~/content/icons/pages/CMS-icon-page-14.png")]
    public class PaymentPlanDetailPage : SitePageData
    {
        /*
                [CultureSpecific]
                [Display(
                    Name = "Main body",
                    Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual XhtmlString MainBody { get; set; }
         */
    }
}