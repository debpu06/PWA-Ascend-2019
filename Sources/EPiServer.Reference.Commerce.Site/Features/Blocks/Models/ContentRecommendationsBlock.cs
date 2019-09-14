using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Content Recommendations Block",
        GUID = "05e741ce-3d7d-4d62-ba2c-c4d94556534a", 
        Description = "List of recommendation contents of the parent content",
        GroupName = "Personalization")]
    [SiteImageUrl("~/content/icons/blocks/CMS-icon-block-24.png")]
    public class ContentRecommendationsBlock : BlockData
    {
        [Display(Name = "Number of recommendations")]
        public virtual int NumberOfRecommendations { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            NumberOfRecommendations = 5;
        }
    }
}