using System;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Models
{
    [CatalogContentType(DisplayName = "ToolNode", GUID = "0e4bcf69-09ac-4381-9b58-d2d07d0ddeaf", Description = "")]
    public class ToolNode : BaseNode, IResourceable
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
    }
}