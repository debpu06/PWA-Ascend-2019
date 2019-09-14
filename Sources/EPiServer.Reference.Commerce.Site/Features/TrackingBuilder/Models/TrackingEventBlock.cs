using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Tracking.Models
{
    [ContentType(DisplayName = "Tracking Event Block", 
                GUID = "515ce47a-85e9-4db1-ae40-5d4bc5342967", Description = "")]
    [ImageUrl("~/Content/icons/blocks/CMS-icon-block-18.png")]
    public class TrackingEventBlock : BlockData
    {
        [Display(
            Name = "Date And Time",
            Description = "",
            GroupName = "Event Data",
            Order = 1)]
        [Required]
        public virtual DateTime DateAndTime{ get; set; }

        [Display(
            Name = "Event Name (Event Title in Insight)",
            Description = "",
            GroupName = "Event Data",
            Order = 2)]
        [Required]
        public virtual string EventType { get; set; }

        [Display(
            Name = "Event Description (Event Description in Insight)",
            Description = "",
            GroupName = "Event Data",
            Order = 3)]
        [Required]
        public virtual string Description { get; set; }
    }
}