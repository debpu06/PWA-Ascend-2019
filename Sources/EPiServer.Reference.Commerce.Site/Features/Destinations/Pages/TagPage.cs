using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Pages
{
    [ContentType(
        DisplayName = "Tags", 
        GUID = "fc83ded1-be4a-40fe-99b2-9ab739b018d5", 
        GroupName = "Destinations",
        Description = "Used to define a Tag")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-27.png")]
    public class TagPage : SitePageData
    {
        [Display(
            Name = "Images", 
            GroupName = SystemTabNames.Content,
            Description = "",
            Order = 5)]
        [AllowedTypes(typeof(ImageData))]
        public virtual ContentArea Images { get; set; }

        [Display(
            Name = "Top area",
            GroupName = SystemTabNames.Content,
            Description = "",
            Order = 100)]
        public virtual ContentArea TopArea { get; set; }

        [Display(
            Name = "Bottom area",
            GroupName = SystemTabNames.Content,
            Description = "",
            Order = 200)]
        public virtual ContentArea BottomArea { get; set; }

        [Display(
            Name = "Intro text",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        [StringLength(5000)]
        //[UIHint(UIHint.Textarea)]
        public virtual string MainIntro { get; set; }

        [Ignore]
        public string SearchSection
        {
            get { return "Tags"; }
        }

        [Ignore]
        public string SearchHitType
        {
            get { return "Tag"; }
        }
    }
}