using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure;

namespace EPiServer.Reference.Commerce.Site.Features.SocialProfile.Pages
{
    [ContentType(DisplayName = "Social profile page", GUID = "C25EED9B-8D61-4502-BDDB-3B1BDED526AD", Description = "Public social profile page",GroupName =SiteTabs.Social, AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/CMS-icon-page-12.png")]
    public class SocialProfilePage : SitePageData
    {

    }
}