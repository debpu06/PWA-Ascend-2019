using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Blocks;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages
{
    [ContentType(GroupName = "Blog",
        GUID = "EAACADF2-3E89-4117-ADEB-F8D43565D2F4", 
        DisplayName = "Blog Item", 
        Description = "Blog Item created underneath the start page and moved to the right area")]
    [AvailableContentTypes(Availability.Specific, Include = new[] { typeof(BlogListPage), typeof(BlogItemPage) })]
    [ImageUrl("~/content/icons/pages/cms-icon-page-18.png")]
    public class BlogItemPage : SitePageData
    {
        [Display(GroupName = SystemTabNames.Content)]
        public virtual string Author { get; set; }

        [Display(GroupName = SystemTabNames.Content)]
        public virtual ContentArea RightContentArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(GroupName = SystemTabNames.Content)]
        public virtual TagCloudBlock TagCloud { get; set; }

        [Display(GroupName = SystemTabNames.Content)]
        public virtual BlogArchiveBlock Archive { get; set; }

        /// <summary>
        /// The comment section of the page. Local comment block will display comments only for this page
        /// </summary>
        [Display(
            Name = "Comment Block",
            Description = "The comment section of the page. Local comment block will display comments only for this page",
            GroupName = SystemTabNames.Content,
            Order = 2)]
        public virtual BlogCommentBlock Comments { get; set; }

    }
}