using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.SpecializedProperties;

namespace EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models
{
    [CatalogContentType(
        GUID = "39776c13-26ec-48c4-a8c3-3d39d989029e",
        MetaClassName = "ToolProduct",
        DisplayName = "Tool Product",
        Description = "Tool product supports mutiple products")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-23.png")]
    public class ToolProduct : BaseProduct
    {
        [Display(GroupName = SystemTabNames.Content, Order = 0)]
        [BackingType(typeof(PropertyString))]
        public virtual string Asin { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 6)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string CeroAgeRating { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 7)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string Department { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 14)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string EpisodeSequence { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 15)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string EsrbAgeRating { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 18)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string Genre { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 19)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string HazardousMaterialType { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 20)]
        public virtual bool IsAdultProduct { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 21)]
        public virtual bool IsAutographed { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 22)]
        public virtual bool IsMemorabilia { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 23)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string Label { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 24)]
        [CultureSpecific]
        public virtual string LegalDisclaimer { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 25)]
        [BackingType(typeof(PropertyString))]
        public virtual string Manufacturer { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 26)]
        [CultureSpecific]
        public virtual string ManufacturerPartsWarrantyDescription { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 27)]
        [BackingType(typeof(PropertyString))]
        public virtual string Model { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 28)]
        [BackingType(typeof(PropertyString))]
        public virtual string ModelYear { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 31)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string ProductGroup { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 32)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string ProductTypeName { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 33)]
        [BackingType(typeof(PropertyString))]
        [CultureSpecific]
        public virtual string ProductTypeSubcategory { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 34)]
        [BackingType(typeof(PropertyString))]
        public virtual string Publisher { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 35)]
        [BackingType(typeof(PropertyString))]
        public virtual string ReleaseDate { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 36)]
        [BackingType(typeof(PropertyString))]
        public virtual string RunningTime { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 38)]
        [BackingType(typeof(PropertyString))]
        public virtual string Studio { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 40)]
        [BackingType(typeof(PropertyString))]
        public virtual string Warranty { get; set; }

        [Display(Name = "On Sale", Order = 41, Description = "Is on sale?")]
        public virtual bool OnSale { get; set; }

        [Display(Name = "New Arrival", Order = 42, Description = "Is on a new arroval?")]
        public virtual bool NewArrival { get; set; }


    }

}