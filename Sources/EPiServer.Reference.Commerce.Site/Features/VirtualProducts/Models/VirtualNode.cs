using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models
{
    [CatalogContentType(
        GUID = "8B6C878B-AD4B-41FF-94C0-8B06E6B41AAD",
        MetaClassName = "VirtualNode",
        DisplayName = "Virtual Node",
        Description = "Display Virtual products.")]
    [ImageUrl("~/content/icons/pages/cms-icon-page-04.png")]
    [AvailableContentTypes(Include = new[]
{
        typeof(BaseProduct),
        typeof(VariantBase),
        typeof(NodeContent)
    })]
    public class VirtualNode : NodeContent, IResourceable, IProductRecommendations
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
        }
    }
}