using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Image Block", GUID = "23e04919-b815-469f-8121-9df86f458c56", Description = "Image Block", GroupName = "Content")]
    [ImageUrl("~/content/icons/blocks/CMS-icon-block-08.png")]
    public class ImageBlock : SiteBlockData
    {
        [CultureSpecific]
        [UIHint(UIHint.Image)]
        public virtual ContentReference Image { get; set; }

        public virtual Url Link { get; set; }
    }
}