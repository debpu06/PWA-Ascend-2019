using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.Stores.Pages
{
    /// <summary>
    /// Cms page with a store locator to select store user want to be their selected store
    /// </summary>
    [ContentType(DisplayName = "StorePage", GUID = "77cf19e8-9a94-4c5b-a9be-ece53de563dc", Description = "", AvailableInEditMode = false)]
    [ImageUrl("~/content/icons/pages/CMS-icon-page-22.png")]
    public class StorePage : SitePageData
    {

    }
}