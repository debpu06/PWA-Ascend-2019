using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [ContentType(DisplayName = "Users In Segment Block", GUID = "46359776-1bd5-48a7-b1da-ce08f5607ce9", Description = "")]
    [SiteImageUrl]
    public class UsersInSegmentBlock : SiteBlockData
    {
        [Display(Order = 0)]
        public virtual string Heading { get; set; }

        [Display(Name = "Segment Name", Order = 5)]
        public virtual string SegmentName { get; set; }

        [Display(Name = "Segment ID", Order = 10)]
        public virtual string SegmentId { get; set; }

        [Display(Name = "Tracking Block Segment", Order = 20)]
        [AllowedTypes(AllowedTypes = new Type[] { typeof(InsightPayloadBlock) })]
        public virtual ContentReference SegmentTrackingBlock { get; set; }
    }
}