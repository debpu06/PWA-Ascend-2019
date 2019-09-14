using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models
{
    [CatalogContentType(DisplayName = "Key Variant", GUID = "C5B021B6-49CD-4F1E-A516-6D6698674A7C")]
    [ImageUrl("~/content/icons/pages/CMS-icon-page-16.png")]
    public class KeyVariant : VirtualVariant
    {
        [Display(GroupName = SystemTabNames.Content, Order = 1)]
        [BackingType(typeof(PropertyString))]
        public virtual string Key { get; set; }
    }
}