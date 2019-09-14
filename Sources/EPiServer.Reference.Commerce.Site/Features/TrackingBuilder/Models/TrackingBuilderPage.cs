using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;
using EPiServer.Reference.Commerce.Site.Features.Tracking.Models;

namespace EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Models
{
    [ContentType(DisplayName = "Tracking Builder Page", GUID = "17b69bbf-8b1c-44f0-a396-b056b5d88002", Description = "", AvailableInEditMode = false)]
    [ImageUrl("~/Content/icons/pages/contactcatalogue.png")]
    public class TrackingBuilderPage : PageData
    {
        [Display(
            Name = "Tracking Events",
            Description = "",
            GroupName = "Event Data",
            Order = 1)]
        [AllowedTypes(new [] { typeof(TrackingEventBlock)})]
        public virtual ContentArea TrackingItems { get; set; }
    }
}