using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;

namespace EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models
{
    [CatalogContentType(
        GUID = "25BD8A3D-5926-4072-ADA6-C7BD16A2F980",
        Description = "Virtual variant supports multiple virtual variation types")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-04.png")]
    public class VirtualVariant : VariantBase
    {
    }
}