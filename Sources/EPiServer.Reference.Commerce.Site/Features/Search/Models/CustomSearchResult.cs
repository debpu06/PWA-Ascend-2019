using EPiServer.Find.Statistics.Api;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using Mediachase.Search;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Models
{
    public class CustomSearchResult
    {
        public IEnumerable<ProductTileViewModel> ProductViewModels { get; set; }
        public ISearchResults SearchResult { get; set; }
        public IEnumerable<FacetGroupOption> FacetGroups { get; set; }
        public int TotalCount { get; set; }
        public DidYouMeanResult DidYouMeans { get; set; }
        public string Query { get; set; }
        public FindSearchContentModel ContentResult { get; set; }
        public FindSearchContentModel PdfResult { get; set; }
        public string RedirectUrl { get; set; }
    }
}