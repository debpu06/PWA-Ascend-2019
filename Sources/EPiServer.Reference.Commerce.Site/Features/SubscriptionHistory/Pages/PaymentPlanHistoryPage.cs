using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.Pages
{
    [ContentType(DisplayName = "PaymentPlanHistory", GUID = "9770edaf-2da0-4522-a446-302d084975c1", Description = "", AvailableInEditMode = false)]
    [SiteImageUrl("~/content/icons/pages/CMS-icon-page-14.png")]
    public class PaymentPlanHistoryPage : SitePageData
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