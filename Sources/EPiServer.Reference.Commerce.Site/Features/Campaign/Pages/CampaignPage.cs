using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.Campaign.Pages
{
    [ContentType(DisplayName = "Campaign page", GUID = "bfba39b8-3161-4d01-a543-f4b0e18e995b", Description = "A Page which is used to show campaign details.", GroupName ="Content")]
    [ImageUrl("~/content/icons/pages/landing.png")]
    public class CampaignPage : SitePageData
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
    }
}