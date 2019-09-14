using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Linkedin Feed Block",
        GUID = "419db9dd-44bc-4540-b446-fcb5f6d588fa",
        GroupName = "Social media")]
    [ImageUrl("~/content/icons/blocks/rss.png")]
    public class LinkedInCompanyBlock : SiteBlockData
    {
        [Display(
            Name = "Company name",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        public virtual string CompanyName { get; set; }
    }
}