using EPiServer.Reference.Commerce.Site.Features.Folder.Pages;
using EPiServer.Shell;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors
{
    /// <summary>
    /// Describes how the UI should appear for <see cref="CalendarEventFolderPage"/> content.
    /// </summary>
    [UIDescriptorRegistration]
    public class CalendarEventFolderUIDescriptor : UIDescriptor<CalendarEventFolderPage>
    {
        public CalendarEventFolderUIDescriptor()
            : base(ContentTypeCssClassNames.Container)
        {
            DefaultView = CmsViewNames.AllPropertiesView;
        }
    }
}