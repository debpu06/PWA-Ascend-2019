using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Twitter Feed Block",
        GUID = "8ed98895-c4a5-4d4d-8abf-43853bd46bc8",
        GroupName = "Social media")]
    [ImageUrl("~/content/icons/blocks/twitter.png")]
    public class TwitterBlock : SiteBlockData
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 100, 
            Name = "Twitter account")]
        public virtual string AccountName { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 110,
            Name = "Max Items")]
        [Range(3, 10)]
        public virtual int MaxItems { get; set; }

        public override void SetDefaultValues(ContentType pageType)
        {
            base.SetDefaultValues(pageType);

            MaxItems = 5;
        }
    }
}