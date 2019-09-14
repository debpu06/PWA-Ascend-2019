using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.CalendarEvent.Pages
{
    [ContentType(DisplayName = "Calendar event", GUID = "f086fd08-4e54-4eb9-8367-c45630415226", Description = "", GroupName = "Calendar Event")]
    [SiteImageUrl("~/content/icons/pages/calendar.png")]
    public class CalendarEventPage : SitePageData
    {
        [CultureSpecific]
        [Display(Name = "Main content area", GroupName = SystemTabNames.Content, Order = 100)]
        public virtual ContentArea MainContentArea { get; set; }

        [CultureSpecific]
        [Display(Name = "Start date", GroupName = SystemTabNames.Content, Order = 200)]
        public virtual DateTime StartDate { get; set; }

        [CultureSpecific]
        [Display(Name = "End date", GroupName = SystemTabNames.Content, Order = 300)]
        public virtual DateTime EndDate { get; set; }

        [CultureSpecific]
        [Display(Name = "Location", GroupName = SystemTabNames.Content, Order = 400)]
        public virtual string Location { get; set; }
    }
}