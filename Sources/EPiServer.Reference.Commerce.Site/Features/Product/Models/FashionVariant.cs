using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    [CatalogContentType(
        GUID = "c1bc1c4e-4a46-4a03-8e90-71bec3576aff",
        MetaClassName = "FashionVariant",
        DisplayName = "Fashion Variant",
        Description = "Display fashion variant")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-21.png")]
    public class FashionVariant : VariantBase
    {

    }
}