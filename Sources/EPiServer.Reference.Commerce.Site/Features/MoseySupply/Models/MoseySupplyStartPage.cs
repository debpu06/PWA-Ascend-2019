using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models
{
    [ContentType(DisplayName = "Mosey Supply Start Page", GUID = "50ea483c-613e-4d38-bb52-52ab398363fb", Description = "", AvailableInEditMode = false)]
    [SiteImageUrl("~/content/icons/pages/CMS-icon-page-02.png")]
    public class MoseySupplyStartPage : BaseStartPage
    {

    }
}