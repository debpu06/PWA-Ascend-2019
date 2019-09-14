using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Models.Blocks
{
    [ContentType(GUID = "C5B623C6-8930-4F97-98F2-0E0B6965DEDF", DisplayName="Tag Cloud Block", GroupName = "Blog")]
    [ImageUrl("~/content/icons/blocks/cms-icon-block-18.png")]
    public class TagCloudBlock : BlockData
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [CultureSpecific]
        [DefaultValue("Tags")]
        public virtual string Heading { get; set; }

        [Display(Name="Blog Tag Link Page",  GroupName = SystemTabNames.Content)]
        public virtual ContentReference BlogTagLinkPage { get; set; }
    }
}