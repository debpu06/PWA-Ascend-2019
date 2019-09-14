using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Site.Features.Alloy;
using EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors;
using EPiServer.Shell.ObjectEditing;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    /// <summary>
    /// Used to insert a link which is styled as a button
    /// </summary>
    [SiteContentType(GUID = "EE7F2843-BB2C-4148-BC3D-099B2D7FD5CB")]
    [Infrastructure.Attributes.SiteImageUrl]
    public class PollResultsBlock : SiteBlockData
    {
        [Display(Order = 1, GroupName = SystemTabNames.Content)]
        [Required]
        public virtual string PollTitle { get; set; }

        [Display(Order = 2, GroupName = SystemTabNames.Content)]
        [Required]
        [SelectOne(SelectionFactoryType = typeof(FormFieldSelectionFactory))]
        public virtual string FormField { get; set; }
    }
}
