using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mediachase.Commerce.Security;

namespace EPiServer.Reference.Commerce.Site.Features.QuickOrder.Blocks
{
    [ContentType(DisplayName = "Quick Order Block", GUID = "003076FD-659C-485E-9480-254A447CC809", Description = "", GroupName = "Commerce")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-14.png")]
    public class QuickOrderBlock : BlockData
    {
        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString MainBody { get; set; }

        [Ignore]
        public List<ProductViewModel> ProductsList { get; set; }
        [Ignore]
        public List<string> ReturnedMessages { get; set; }

        [Ignore]
        public bool HasOrganization
        {
            get
            {
                var contact = PrincipalInfo.CurrentPrincipal.GetCustomerContact();
                return contact?.OwnerId != null;
            }
        }
    }
}