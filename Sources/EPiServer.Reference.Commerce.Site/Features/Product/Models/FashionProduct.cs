using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    [CatalogContentType(
        GUID = "550ebcfc-c989-4272-8f94-c6d079f56181",
        MetaClassName = "FashionProduct",
        DisplayName = "Fashion product",
        Description = "Display fashion product")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-21.png")]
    public class FashionProduct : BaseProduct
    {
        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [Display(Name = "Sizing", Order = 4)]
        public virtual XhtmlString Sizing { get; set; }

        [CultureSpecific]
        [Display(Name = "Product Teaser", Order = 5)]
        public virtual XhtmlString ProductTeaser { get; set; }

        [Searchable]
        [IncludeInDefaultSearch]
        [BackingType(typeof(PropertyDictionaryMultiple))]
        [Display(Name = "Available Sizes", Order = 6)]
        public virtual ItemCollection<string> AvailableSizes { get; set; }

        [Searchable]
        [IncludeInDefaultSearch]
        [BackingType(typeof(PropertyDictionaryMultiple))]
        [Display(Name = "Available Colors", Order = 6)]
        public virtual ItemCollection<string> AvailableColors { get; set; }

        [Display(Name = "On Sale", Order = 7, Description = "Is on sale?")]
        public virtual bool OnSale { get; set; }

        [Display(Name = "New Arrival", Order = 8, Description = "Is on a new arroval?")]
        public virtual bool NewArrival { get; set; }
    }
}