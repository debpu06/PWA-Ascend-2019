using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages
{
    /// <summary>
    /// Used for campaign or landing pages, commonly used for pages linked in online advertising such as AdWords
    /// </summary>
    [SiteContentType(
       GUID = "F94571B0-65C4-4E49-8A88-5930D045E19D",
       DisplayName = "Two column landing page",
       Description = "Two column landing page with one wide (two thirds) column on the left and a narrow on the right (one third)",
       GroupName = SiteTabs.Content)]
    [SiteImageUrl("~/content/gfx/page-type-thumbnail-landingpage-twocol.png")]
    public class TwoColumnLandingPage : LandingPage
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order=350)]
        [CultureSpecific]
        public virtual ContentArea RightHandContentArea { get; set; }
    }
}
