using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models
{
    [ContentType(DisplayName = "Mosey Supply Profile Page", GUID = "4f1ad8f5-4d2d-4368-8f8f-96a3d410090d", Description = "", AvailableInEditMode = false)]
    [SiteImageUrl("~/content/icons/pages/elected.png")]
    public class MoseySupplyProfilePage : SitePageData
    {
        
    }
}