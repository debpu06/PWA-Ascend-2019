using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Insight Payload Block", GUID = "d76ca260-988f-41ca-8cd8-9134fd9e79f8", Description = "Updates an Insight profile with the specified event and payload.")]
    [SiteImageUrl]
    public class InsightPayloadBlock : BlockData
    {
        [Display(Name = "Create Personalization Segment Automatically", Order = 20)]
        public virtual bool CreateSegmentAutomatically { get; set; }

        [Display(Name = "Tracking Key", Order = 0)]
        [Required]
        public virtual string ProfileValueKey { get; set; }

        [Display(Name = "Tracking Value", Order = 10)]
        [Required]
        public virtual string ProfileValueValue { get; set; }
    }
}