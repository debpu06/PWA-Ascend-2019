using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Mosey.Helper;
using EPiServer.Reference.Commerce.Site.Infrastructure;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Mosey.Models.Pages
{
    [ContentType(DisplayName = "Article Page", GUID = "c0a25bb7-199c-457d-98c6-b0179c7acae8", GroupName = SiteTabs.Content)]
    [SiteImageUrl("~/content/icons/pages/CMS-icon-page-23.png")]
    public class ArticlePage : SitePageData
    {
        [CultureSpecific]
        [Display(Name = "Main Background Image", GroupName = SystemTabNames.Content)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference MainBackgroundImage { get; set; }

        [CultureSpecific]
        [Display(Name = "Main Background Video", GroupName = SystemTabNames.Content)]
        [UIHint(UIHint.Video)]
        public virtual ContentReference MainBackgroundVideo { get; set; }

        [Display(Name = "Top Padding Mode", Description = "Sets how much padding should be at the top of the article content", GroupName = SystemTabNames.Content)]
        [SelectOne(SelectionFactoryType = typeof(MoseyArticlePageTopPaddingModeSelectionFactory))]
        public virtual string TopPaddingMode { get; set; }

        [CultureSpecific]
        [Display(Name = "Main Content Items", GroupName = SystemTabNames.Content)]
        public virtual ContentArea MainContentItems { get; set; }

        [CultureSpecific]
        [Display(Name = "Secondary Content Items", GroupName = SystemTabNames.Content)]
        public virtual ContentArea SecondaryContentItems { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            TopPaddingMode = MoseyArticlePageTopPaddingModeSelectionFactory.MoseyArticlePageTopPaddingModes.None;
        }
    }
}