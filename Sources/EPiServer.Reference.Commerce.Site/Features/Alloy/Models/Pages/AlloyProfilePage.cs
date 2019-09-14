using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages
{
    [ContentType(DisplayName = "Alloy Profile Page", GUID = "b626c743-e043-4056-88fc-d8f8abe8d400", Description = "This page will be used to display a logged in user's profile.", AvailableInEditMode = false)]
    [SiteImageUrl("/content/icons/blocks/CMS-icon-block-17.png")]
    public class AlloyProfilePage : SitePageData
    {
        [CultureSpecific]
        [Display(
            Name = "Profile Header",
            Description = "This will display header text on top of the user's profile",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        public virtual String ProfileHeader { get; set; }

        
        [CultureSpecific]
        [Display(
            Name = "Main Content Area",
            Description = "This will display the main content area.",
            GroupName = SystemTabNames.Content,
            Order = 300)]
        public virtual ContentArea MainContentArea { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Left Hand Content Area",
            Description = "This will display the main content area.",
            GroupName = SystemTabNames.Content,
            Order = 300)]
        public virtual ContentArea LeftHandContentArea { get; set; }
    }
}