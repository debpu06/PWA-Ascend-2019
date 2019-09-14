using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    [CatalogContentType(DisplayName = "GenericNode", GUID = "4ac27ad4-bf60-4ea0-9a77-28a89d38d3fd", Description = "")]
    public class GenericNode : BaseNode, IResourceable, IProductRecommendations
    {
        [CultureSpecific]
        [Display(Name = "LongName", GroupName = SystemTabNames.Content)]
        [BackingType(typeof(PropertyString))]
        public virtual string LongName { get; set; }

        [CultureSpecific]
        [Display(Name = "Teaser", GroupName = SystemTabNames.Content)]
        public virtual string Teaser { get; set; }

        [Searchable]
        [CultureSpecific]
        [Tokenize]
        [IncludeInDefaultSearch]
        [Display(Name = "Description", GroupName = SystemTabNames.Content)]
        public virtual XhtmlString Description { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Top content area",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 4)]
        public virtual ContentArea TopContentArea { get; set; }

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