using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Pages
{
    [ContentType(DisplayName = "Gift card page", GUID = "845a7ade-4cac-4efd-86fd-a71ac3cfa2b6", Description = "This page displays all gift cards belonging to an user", GroupName = "Commerce", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/CMS-icon-page-12.png")]
    public class GiftCardPage : SitePageData
    {
    }
}