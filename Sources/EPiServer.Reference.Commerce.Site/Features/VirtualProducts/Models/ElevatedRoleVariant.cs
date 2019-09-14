using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Search.EditorDescriptors;
using EPiServer.Shell.ObjectEditing;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models
{
    [CatalogContentType(DisplayName = "Elevated Role Variant", GUID = "909BFD08-495A-4449-904F-607233EE98DC")]
    [ImageUrl("~/content/icons/pages/elected.png")]
    public class ElevatedRoleVariant : VirtualVariant
    {
        [Display(GroupName = SystemTabNames.Content, Order = 1)]
        [SelectOne(SelectionFactoryType = typeof(ElevatedRoleSelectionFactory))]
        [BackingType(typeof(PropertyString))]
        public virtual string Role { get; set; }
    }
}