using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models
{
    [CatalogContentType(
        GUID = "ECE2D0A1-C009-48B7-9D64-3765ECA43612",
        MetaClassName = "VirtualProduct",
        DisplayName = "Virtual Product",
        Description = "Display virtual product")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-04.png")]
    public class VirtualProduct : BaseProduct
    {

        [Display(Name = "Max Redemptions", Order = 4)]
        public virtual int MaxRedemptions { get; set; }

        [Display(Name = "Used Redemptions", Order = 5)]
        public virtual int UsedRedemptions { get; set; }
    }
}