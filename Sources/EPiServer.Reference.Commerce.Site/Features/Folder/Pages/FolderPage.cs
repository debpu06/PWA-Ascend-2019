using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Folder.Pages
{
    [ContentType(DisplayName = "Folder page", GUID = "1bc8e78b-40cc-4efc-a561-a0bba89b51ac", Description = "A page which allows you to structure pages.", GroupName ="Content")]
    [AvailableContentTypes(IncludeOn = new[] { typeof(BaseStartPage), typeof(FolderPage) })]
    [ImageUrl("~/content/icons/pages/container.png")]
    public class FolderPage : SitePageData
    {
    }
}