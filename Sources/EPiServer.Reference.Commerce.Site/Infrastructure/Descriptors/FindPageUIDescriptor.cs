using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Folder.Pages;
using EPiServer.Shell;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors
{
    /// <summary>
    /// Describes how the UI should appear for <see cref="FolderPage"/> content.
    /// </summary>
    [UIDescriptorRegistration]
    public class FindPageUIDescriptor : UIDescriptor<FindPage>
    {
        public FindPageUIDescriptor()
            : base("epi-icon__search")
        {
            
        }
    }
}
