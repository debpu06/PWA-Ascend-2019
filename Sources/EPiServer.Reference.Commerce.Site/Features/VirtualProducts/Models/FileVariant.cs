using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models
{
    [CatalogContentType(DisplayName = "File Variant", GUID = "6D556CBC-09E7-4F99-B7FF-70379432FBBE")]
    [ImageUrl("~/content/icons/pages/container.png")]
    public class FileVariant : VirtualVariant
    {
        [Display(GroupName = SystemTabNames.Content, Order = 1)]
        [UIHint(UIHint.MediaFile)]
        public virtual ContentReference File { get; set; }
    }
}