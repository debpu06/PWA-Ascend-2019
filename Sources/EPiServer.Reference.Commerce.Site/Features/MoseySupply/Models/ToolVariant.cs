using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    [CatalogContentType(DisplayName = "Tool Variant", GUID = "08c70c6b-921e-4a2c-ac1e-ef0682c5f996", Description = "Tool variant supports multiple variation types")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-23.png")]
    public class ToolVariant : VariantBase
    {
        [Display(GroupName = SystemTabNames.Content, Order = 1)]
        [BackingType(typeof(PropertyString))]
        public virtual string AspectRatio { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 2)]
        [BackingType(typeof(PropertyString))]
        public virtual string AudienceRating { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 3)]
        [BackingType(typeof(PropertyString))]
        public virtual string Binding { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 5)]
        [BackingType(typeof(PropertyString))]
        public virtual string Ean { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 6)]
        [BackingType(typeof(PropertyString))]
        public virtual string Edition { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 7)]
        public virtual bool IsEligibleForTradeIn { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 8)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string HardwarePlatform { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 9)]
        [BackingType(typeof(PropertyString))]
        public virtual string Isbn { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 10)]
        [BackingType(typeof(PropertyString))]
        public virtual string IssuesPerYear { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 11)]
        [BackingType(typeof(PropertyString))]
        public virtual string ItemPartNumber { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 12)]
        [BackingType(typeof(PropertyString))]
        public virtual string MagazineType { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 13)]
        [BackingType(typeof(PropertyString))]
        public virtual string NumberOfDiscs { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 14)]
        [BackingType(typeof(PropertyString))]
        public virtual string NumberOfIssues { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 15)]
        [BackingType(typeof(PropertyString))]
        public virtual string NumberOfItems { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 16)]
        [BackingType(typeof(PropertyString))]
        public virtual string NumberOfPages { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 17)]
        [BackingType(typeof(PropertyString))]
        public virtual string NumberOfTracks { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 18)]
        [BackingType(typeof(PropertyString))]
        public virtual string OperatingSystem { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 19)]
        [BackingType(typeof(PropertyString))]
        public virtual string MediaType { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 20)]
        [BackingType(typeof(PropertyString))]
        public virtual string Mpn { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 21)]
        [BackingType(typeof(PropertyString))]
        public virtual string PackageQuantity { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 22)]
        [BackingType(typeof(PropertyString))]
        public virtual string PartNumber { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 23)]
        [BackingType(typeof(PropertyString))]
        public virtual string PublicationDate { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 24)]
        [BackingType(typeof(PropertyString))]
        public virtual string RegionCode { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 25)]
        [BackingType(typeof(PropertyString))]
        public virtual string TrackSequence { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 26)]
        [BackingType(typeof(PropertyString))]
        public virtual string Sku { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 27)]
        [BackingType(typeof(PropertyString))]
        public virtual string SubscriptionLength { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 29)]
        [BackingType(typeof(PropertyString))]
        public virtual string Upc { get; set; }
    }
}