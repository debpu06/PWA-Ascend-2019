using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignWide.Pages
{
    [ContentType(DisplayName = "Inspirational content page", GUID = "D5D42446-4CAA-41E9-9CDC-5E75BB24258D", Description = "A large full bleed picture at the top followed by a padded section with header, mainbody and secondary content area", GroupName = "Content")]
    [ImageUrl("~/content/icons/pages/landing.png")]
    public class CampaignWidePage : SitePageData
    {
        [Display(Name = "Page Title",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        [CultureSpecific]
        public virtual String PageTitle { get; set; }

        [Display(Name = "Main Content Area",
          Description = "This is the main content area",
          GroupName = SystemTabNames.Content,
          Order = 20)]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(Name = "Secondary Content Area",
  Description = "This is the secondary content area",
  GroupName = SystemTabNames.Content,
  Order = 21)]
        public virtual ContentArea SecondaryContentArea { get; set; }
    }
}