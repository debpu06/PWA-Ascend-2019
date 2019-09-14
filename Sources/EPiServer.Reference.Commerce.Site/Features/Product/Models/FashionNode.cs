using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    [CatalogContentType(
        GUID = "a23da2a1-7843-4828-9322-c63e28059f6a",
        MetaClassName = "FashionNode",
        DisplayName = "Fashion Node",
        Description = "Display fashion products.")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-21.png")]
    [AvailableContentTypes(Include = new[]
    {
        typeof(BaseProduct),
        typeof(FashionPackage),
        typeof(FashionBundle),
        typeof(VariantBase),
        typeof(NodeContent)
    })]
    public class FashionNode : BaseNode, IResourceable, IProductRecommendations
    {
        [CultureSpecific]
        [Display(
            Name = "Top content area",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 4)]
        public virtual ContentArea TopContentArea { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Bottom content area",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 3)]
        public virtual ContentArea BottomContentArea { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Featured products",
            GroupName = SystemTabNames.Content,
            Order = 4)]
        public virtual ContentArea FeaturedProducts { get; set; }

        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [Display(Name = "Description", Order = 1)]
        public virtual XhtmlString Description { get; set; }

        [Editable(false)]
        [ScaffoldColumn(false)]
        public virtual string NodeContentAssetId { get; set; }

        [Ignore]
        public Guid ContentAssetsID
        {
            get
            {
                return !string.IsNullOrEmpty(NodeContentAssetId) ? new Guid(NodeContentAssetId) : Guid.Empty;
            }
            set { NodeContentAssetId = value.ToString(); }
        }

        [CultureSpecific]
        [Display(Name = "Show Recommendations", Order = 50, Description = "This will determine whether or not to show recommendations.")]
        public virtual bool ShowRecommendations { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            ShowRecommendations = true;
            PartialPageSize = 4;
        }
    }
}