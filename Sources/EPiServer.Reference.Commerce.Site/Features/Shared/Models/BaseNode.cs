using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce.Catalog.ContentTypes;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Models
{
    public abstract class BaseNode : NodeContent
    {
        [Display(Name = "Partial page size", Order = 1)]
        public virtual int PartialPageSize { get; set; }
    }
}