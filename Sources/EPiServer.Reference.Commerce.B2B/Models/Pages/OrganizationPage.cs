using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.B2B.Models.Pages
{
    [ContentType(DisplayName = "OrganizationPage", GUID = "e50f0e69-0851-40dc-b00c-38f0acec3f32", Description = "", AvailableInEditMode = false, GroupName = "B2B")]
    [ImageUrl("~/content/icons/pages/elected.png")]
    public class OrganizationPage : SitePageData
    {

    }
}