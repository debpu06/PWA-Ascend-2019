using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Site.Features.Alloy;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    /// <summary>
    /// Used to insert a link which is styled as a button
    /// </summary>
    [SiteContentType(GUID = "426CF12F-1F01-4EA0-922F-0778314DDAF0")]
    [SiteImageUrl]
    public class ButtonBlock : SiteBlockData
    {
        [Display(Order = 1, Name = "Text", GroupName = SystemTabNames.Content)]
        public virtual string ButtonText { get; set; }

        [Display(Order = 2, Name = "Link", GroupName = SystemTabNames.Content)]
        public virtual Url ButtonLink { get; set; }

        [Display(Order = 3, Name = "CSS Style", GroupName = SystemTabNames.Content)]
        public virtual string ButtonStyle { get; set; }

        [Display(Order = 4, Name = "Reassuring caption ", GroupName = SystemTabNames.Content, Prompt = "cancel anytime...")]
        public virtual string ButtonCaption { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            ButtonStyle = "btn-blue";
        }

    }
}
