using System.ComponentModel;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.Blocks
{
    [ContentType(GUID = "73F610A5-D705-4BCA-960A-3CA03F312D30", DisplayName="Blog Archive Block", GroupName ="Blog")]
    [ImageUrl("~/content/icons/blocks/cms-icon-block-19.png")]
    public class BlogArchiveBlock : BlockData
    {
        [DefaultValue("Archive")]
        public virtual string Heading { get; set; }

        public virtual ContentReference BlogStart { get; set; }
    }
}