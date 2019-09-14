using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Extensions;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages
{
    [ContentType(GroupName = "Blog",
        GUID = "EAACADF4-3E89-4117-ADEB-F8D43565D2F4", 
        DisplayName="Blog Start", 
        Description="Blog Start Page with blog items underneath")]
    [AvailableContentTypes(Availability.Specific, Include = new[] { typeof(BlogListPage), typeof(BlogItemPage) })]
    [ImageUrl("~/content/icons/pages/cms-icon-page-19.png")]
    public class BlogStartPage : BaseBlogList
    {
        
        #region IInitializableContent

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            BlogList.PageTypeFilter = typeof(BlogItemPage).GetPageType();
            BlogList.Recursive = true;
        }

        #endregion
    }
}