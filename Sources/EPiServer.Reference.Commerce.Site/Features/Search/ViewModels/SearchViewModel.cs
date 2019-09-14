using EPiServer.Core;
using EPiServer.Find.Statistics.Api;
using EPiServer.Personalization.Commerce.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using Mediachase.Search;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Search.ViewModels
{
    public class SearchViewModel<T> : ContentViewModel<T> where T : IContent
    {
        // ViewModel for products
        public IEnumerable<ProductTileViewModel> ProductViewModels { get; set; }
        public IEnumerable<Recommendation> Recommendations { get; set; }
        public FilterOptionViewModel FilterOption { get; set; }
        public ISearchFacetGroup[] Facets { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public CategoriesFilterViewModel CategoriesFilter { get; set; }
        public DidYouMeanResult DidYouMeans { get; set; }
        public string Query { get; set; }
        public bool ShowRecommendations { get; set; }
        public bool IsMobile { get; set; }
        // ViewModel for content (articles)
        public FindSearchContentModel ContentResult {get; set;}
        // ViewModel for PDF
        public FindSearchContentModel PdfResult { get; set; }

        public bool ShowCommerceControls
        {
            get
            {
                return StartPage.ShowCommerceHeaderComponents;
            }
        }
    }
}