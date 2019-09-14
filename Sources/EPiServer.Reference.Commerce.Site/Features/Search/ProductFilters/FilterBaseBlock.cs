using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Find.Api.Querying;
using EPiServer.Reference.Commerce.Site.Features.Search.EditorDescriptors;
using EPiServer.Shell.ObjectEditing;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Search.ProductFilters
{
    public abstract class FilterBaseBlock : BlockData
    {
        public abstract Filter GetFilter();

        [CultureSpecific]
        [Display(
            Name = "Field Name",
            Description = "Name of field in index",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [SelectOne(SelectionFactoryType = typeof(FindProductFilterFieldSelectionFactory))]
        public virtual string FieldName { get; set; }

    }
}