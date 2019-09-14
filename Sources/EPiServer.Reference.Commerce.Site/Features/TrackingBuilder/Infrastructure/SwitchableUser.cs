using EPiServer.Reference.Commerce.Site.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Infrastructure
{
    public class SwitchableUser
    {
        [Display(Name = "Username", GroupName = SiteTabs.SiteSettings, Order = 300)]
        public string UserName { get; set; }

        [Display(Name = "First Name", GroupName = SiteTabs.SiteSettings, Order = 301)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name", GroupName = SiteTabs.SiteSettings, Order = 305)]
        public string LastName { get; set; }

        [Display(Name = "User Description", GroupName = SiteTabs.SiteSettings, Order = 310)]
        public string UserDescription { get; set; }

        [Display(Name = "Tracking Code", GroupName = SiteTabs.SiteSettings, Order = 315)]
        public string UserTrackingCode { get; set; }
    }
}