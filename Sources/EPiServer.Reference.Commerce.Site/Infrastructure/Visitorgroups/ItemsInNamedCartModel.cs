using EPiServer.Business.Commerce.VisitorGroupsCriteria;
using EPiServer.Reference.Commerce.Shared.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Visitorgroups
{
    public class ItemsInNamedCartModel : ModelBase
    {
        [Required]
        [LocalizedDisplay("/Shared/CartName")]
        public string CartName { get; set; }

        [Required]
        [LocalizedDisplay("/Shared/NumberOfItems")]
        public int NumberOfItems { get; set; }

    }
}