using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Pages
{
    [ContentType(DisplayName = "AccountInformationPage", GUID = "aab8be44-023e-4f21-88bc-4fb262dfa85f", Description = "", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/cms-icon-page-08.png")]
    public class AccountInformationPage : SitePageData
    {
        public virtual ContentArea MainContentArea { get; set; }
    }
}