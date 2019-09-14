using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Video Block", GUID = "03D454F9-3BE8-4421-9A5D-CBBE8E38443D", Description = "Video Block", GroupName = "Content")]
    [ImageUrl("~/content/icons/blocks/CMS-icon-block-05.png")]
    public class VideoBlock : SiteBlockData
    {
        [CultureSpecific]
        [UIHint(UIHint.Video)]
        public virtual ContentReference Video { get; set; }

    }
}