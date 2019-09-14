using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Folder.Pages;
using EPiServer.Shell;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Descriptors
{
    /// <summary>
    /// Describes how the UI should appear for <see cref="FolderPage"/> content.
    /// </summary>
    [UIDescriptorRegistration]
    public class BlogPageUIDescriptor : UIDescriptor<BlogItemPage>
    {
        public BlogPageUIDescriptor()
            : base("epi-icon__blog")
        {
            
        }
    }
}
