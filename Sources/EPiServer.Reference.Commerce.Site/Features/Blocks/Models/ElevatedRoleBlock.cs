using EPiServer.Core;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{

    [ContentType(DisplayName = "Elevated Role Block", GUID = "DD114EBB-2027-4B81-816E-3B228D121DD8", Description = "Elevated Role Block that uses access rights for read", GroupName = "Content")]
    [ImageUrl("~/content/icons/pages/elected.png")]
    public class ElevatedRoleBlock : BlockData
    {
        [Display(Name = "Content")]
        public virtual XhtmlString Content { get; set; }
    }
}