using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Facebook Feed Block",
        GUID = "fe935bfb-44b0-4ce2-a448-1d366ff3bbc0",
        GroupName = "Social media")]
    [ImageUrl("~/content/icons/blocks/rss.png")]
    public class FacebookBlock : SiteBlockData
    {
        [Display(
            Name = "Accountname",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        public virtual string AccountName { get; set; }
    }
}