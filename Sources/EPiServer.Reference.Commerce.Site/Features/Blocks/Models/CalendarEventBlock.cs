using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared.EditorDescriptors;
using EPiServer.Shell.ObjectEditing;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    /// <summary>
    /// Used to insert a list of events on calendar
    /// </summary>
    [ContentType(GUID = "D5148C01-DFB0-4E57-8399-6CEEBF48F38E",
        DisplayName = "Calendar event block",
        Description = "Display list of events on calendar",
        GroupName = "Calendar Event")]
    [ImageUrl("~/content/icons/pages/calendar.png")]
    public class CalendarEventBlock : SiteBlockData
    {
        [Required]
        [CultureSpecific]
        [Display(GroupName = SystemTabNames.Content, Name = "View Mode", Order = 1)]
        [SelectOne(SelectionFactoryType = typeof(CalendarViewModeSelectionFactory))]
        public virtual string ViewMode { get; set; }

        [Required]
        [Display(GroupName = SystemTabNames.Content, Order = 2)]
        public virtual PageReference Root { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 3)]
        public virtual int Count { get; set; }

        [Display(GroupName = SystemTabNames.Content, Name = "Category Filter", Order = 4)]
        public virtual CategoryList CategoryFilter { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 5)]
        public virtual bool Recursive { get; set; }

        #region IInitializableContent

        /// <summary>
        /// Sets the default property values on the content data.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            ViewMode = "month";
        }

        #endregion
    }
}
