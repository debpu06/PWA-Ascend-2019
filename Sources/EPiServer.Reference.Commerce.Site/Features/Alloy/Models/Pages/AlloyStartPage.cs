using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Folder.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure;
using EPiServer.SpecializedProperties;
using System.ComponentModel.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Editorial.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages
{
    /// <summary>
    /// Used for the site's start page and also acts as a container for site settings
    /// </summary>
    [ContentType(
        GUID = "19671657-B684-4D95-A61F-8DD4FE60D559",
        GroupName = SiteTabs.Specialized, AvailableInEditMode = false)]
    [SiteImageUrl]
    [AvailableContentTypes(
        Availability.Specific,
        Include = new[] { typeof(FolderPage), typeof(ProductPage), typeof(StandardPage), typeof(AlloySearchPage), typeof(LandingPage), typeof(ContentFolder), typeof(FindPage), typeof(AlloyProfilePage) }, // Pages we can create under the start page...
        ExcludeOn = new[] { typeof(ProductPage), typeof(StandardPage), typeof(AlloySearchPage), typeof(LandingPage) })] // ...and underneath those we can't create additional start pages
    public class AlloyStartPage : BaseStartPage
    {
        
        [Display(GroupName = SiteTabs.SiteSettings, Order = 300)]
        public virtual LinkItemCollection ProductPageLinks { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings, Order = 350)]
        public virtual LinkItemCollection CompanyInformationPageLinks { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings, Order = 400)]
        public virtual LinkItemCollection NewsPageLinks { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings, Order = 450)]
        public virtual LinkItemCollection CustomerZonePageLinks { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings)]
        public virtual PageReference GlobalNewsPageLink { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings)]
        public virtual PageReference ContactsPageLink { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings)]
        public virtual PageReference SearchPageLink { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings)]
        public virtual PageReference NewsPageLink { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings)]
        public virtual PageReference BlogPageLink { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings, Name = "Employees Root Page", Order = 600)]
        public virtual ContentReference EmployeeContainerPageLink { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings, Name = "Location Root Page", Order = 610)]
        public virtual PageReference EmployeeLocationPageLink { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings, Name = "Expertise Root Page", Order = 620)]
        public virtual ContentReference EmployeeExpertiseLink { get; set; }

        [Display(GroupName = SiteTabs.SiteSettings)]
        public virtual SiteLogotypeBlock SiteLogotype { get; set; }

    }
}
