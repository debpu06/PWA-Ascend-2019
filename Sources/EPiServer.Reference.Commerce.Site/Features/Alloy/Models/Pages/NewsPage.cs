using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Filters;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Editorial.Models;
using EPiServer.Reference.Commerce.Site.Features.Folder.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages
{
    /// <summary>
    /// Presents a news section including a list of the most recent articles on the site
    /// </summary>
    [SiteContentType(GUID = "638D8271-5CA3-4C72-BABC-3E8779233263")]
    [SiteImageUrl]
    [AvailableContentTypes(
        Availability.Specific,
        Exclude = new[] { typeof(FolderPage), typeof(ProductPage), typeof(FindPage), typeof(AlloySearchPage), typeof(LandingPage), typeof(ContentFolder), typeof(BlogItemPage), typeof(BlogStartPage), typeof(BlogListPage) },
        Include = new[] { typeof(AlloyArticlePage), typeof(StandardPage), typeof(NewsPage) })] // Pages we can create under the news page...
    public class NewsPage : StandardPage
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 305)]
        public virtual PageListBlock NewsList { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            NewsList.Count = 20;
            NewsList.Heading = ServiceLocator.Current.GetInstance<LocalizationService>().GetString("/newspagetemplate/latestnews");
            NewsList.IncludeIntroduction = true;
            NewsList.IncludePublishDate = true;
            NewsList.Recursive = true;
            NewsList.PageTypeFilter = typeof(AlloyArticlePage).GetPageType();
            NewsList.SortOrder = FilterSortOrder.PublishedDescending;
        }
    }
}
