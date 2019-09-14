using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Recent Page Category Recommendation", GUID = "d1728a48-764a-4a02-bfb6-4e004fb4ac92", Description = "")]
    [SiteImageUrl("~/content/icons/blocks/CMS-icon-block-23.png")]
    public class RecentPageCategoryRecommendation : SiteBlockData
    {
        [Display(Name = "Number of Recommendations")]
        public virtual int NumberOfRecommendations { get; set; }

        [Display(Name = "Inspiration Folder")]
        public virtual ContentReference InspirationFolder { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            NumberOfRecommendations = 2;
        }
    }
}