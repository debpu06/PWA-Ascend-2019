using EPiServer.Core;
using EPiServer.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Models
{
    [ContentType(DisplayName = "Tracking User Setup Page", GUID = "365cf930-d9b3-446a-b28c-02c9c982983f", Description = "", AvailableInEditMode = false)]
    [ImageUrl("~/Content/icons/pages/contactcatalogue.png")]
    public class TrackingUserPage : PageData
    {
        public virtual string UserName { get; set; }

        [Display(
            Name = "Link to page for adding tracking events",
            Description = "",
            GroupName = "Event Data",
            Order = 1)]
        [AllowedTypes(new[] { typeof(TrackingBuilderPage) })]
        public virtual ContentReference TrackingPageLink { get; set; }
    }
}