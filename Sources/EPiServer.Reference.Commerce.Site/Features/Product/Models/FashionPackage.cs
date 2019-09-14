using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    [CatalogContentType(
        GUID = "9568BD9F-FC9B-4004-A02C-5C0C90B68C43",
        DisplayName = "Fashion Package",
        MetaClassName = "FashionPackage",
        Description = "Displays a package, which is comparable to an individual SKU because Package item must be purchased as a whole.")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-21.png")]
    public class FashionPackage : PackageContent, IProductRecommendations
    {
        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [Display(Name = "Description", Order = 1)]
        public virtual XhtmlString Description { get; set; }

        [Display(Name = "On Sale", Order = 2, Description = "Is on sale?")]
        public virtual bool OnSale { get; set; }

        [Display(Name = "New Arrival", Order = 2, Description = "Is on a new arroval?")]
        public virtual bool NewArrival { get; set; }

        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [Display(Name = "Long Description", Order = 3)]
        public virtual XhtmlString LongDescription { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Content Area",
            Order = 4,
            Description = "This will display the content area."
            )]
        public virtual ContentArea ContentArea { get; set; }

        [CultureSpecific]
        [Display(Name = "Show Recommendations", Order = 50, Description = "This will determine whether or not to show recommendations.")]
        public virtual bool ShowRecommendations { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            ShowRecommendations = true;
        }
    }
}