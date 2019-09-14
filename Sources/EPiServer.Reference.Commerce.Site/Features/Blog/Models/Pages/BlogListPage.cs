using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages
{
    [ContentType(GroupName = "Blog",
        GUID = "EAADAFF2-3E89-4117-ADEB-F8D43565D2F4", 
        DisplayName = "Blog Item List", 
        Description = "Blog Item List for dates such as year and month")]
    [AvailableContentTypes(Availability.Specific,    Include = new[] { typeof(BlogListPage), typeof(BlogItemPage) })]
    [ImageUrl("~/content/icons/pages/cms-icon-page-20.png")]
    public class BlogListPage : BaseBlogList
    {

        
    }
}