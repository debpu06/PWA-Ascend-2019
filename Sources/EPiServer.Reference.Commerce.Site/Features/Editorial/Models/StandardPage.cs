using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure;

namespace EPiServer.Reference.Commerce.Site.Features.Editorial.Models
{
    
    [ContentType(GUID = "9CCC8A41-5C8C-4BE0-8E73-520FF3DE8267",
        DisplayName = "Standard Page",
        GroupName = "Content",
        Description = "Stanard page with left menu of child pages")]
    [ImageUrl("~/content/icons/pages/Landing.png")]
    public class StandardPage : SitePageData, IUseDirectLayout
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 330)]
        public virtual ContentArea TopContentArea { get; set; }
    }
}
