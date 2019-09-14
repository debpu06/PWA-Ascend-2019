using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Blocks;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages
{
    public abstract class BaseBlogList : SitePageData
    {
        [Display(GroupName = SystemTabNames.Content)]
        public virtual string Heading { get; set; }

        [Display(GroupName = SystemTabNames.Content)]
        public virtual BlogListBlock BlogList { get; set; }

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
    }
}