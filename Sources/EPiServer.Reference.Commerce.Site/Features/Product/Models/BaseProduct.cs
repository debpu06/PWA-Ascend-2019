using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.DataAbstraction;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    public abstract class BaseProduct : ProductContent, IResourceable, IProductRecommendations
    {
        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [BackingType(typeof(PropertyString))]
        [Display(Name = "Brand", Order = 5)]
        public virtual string Brand { get; set; }

        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [Display(Name = "Description", Order = 9)]
        public virtual XhtmlString Description { get; set; }

        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [Display(Name = "Long Description", Order = 43)]
        public virtual XhtmlString LongDescription { get; set; }

        [Editable(false)]
        [ScaffoldColumn(false)]
        public virtual string EntryContentAssetId { get; set; }

        [Ignore]
        [ScaffoldColumn(false)]
        public Guid ContentAssetsID
        {
            get
            {
                return !string.IsNullOrEmpty(EntryContentAssetId) ? new Guid(EntryContentAssetId) : Guid.Empty;
            }
            set { EntryContentAssetId = value.ToString(); }
        }

        [CultureSpecific]
        [Display(Name = "Content Area", Order = 44, Description = "This will display the content area.")]
        public virtual ContentArea ContentArea { get; set; }

        [CultureSpecific]
        [Display(Name = "Show Recommendations", Order = 50, Description = "This will determine whether or not to show recommendations.")]
        public virtual bool ShowRecommendations { get; set; }

        [CultureSpecific]
        [Display(Name = "Exclude From Search", Order = 60, Description = "This will determine whether or not to show on search.")]
        public virtual bool ExcludeFromSearch { get; set; }

        [CultureSpecific]
        [Display(Name = "Use Images For Variants Selection", Order = 70, Description = "This will determine whether or not to use images for variants selection.")]
        public virtual bool UseImagesForVariantsSelection { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            ShowRecommendations = true;
        }
    }
}