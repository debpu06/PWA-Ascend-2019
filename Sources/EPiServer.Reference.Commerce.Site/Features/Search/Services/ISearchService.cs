using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using System.Collections.Generic;
using EPiServer.Find.Api.Querying;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Find;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="currentContent"></param>
        /// <param name="filterOptions"></param>
        /// <param name="selectedFacets"></param>
        /// <returns></returns>
        CustomSearchResult Search(IContent currentContent, FilterOptionViewModel filterOptions, string selectedFacets, int catalogId = 0);
        /// <summary>
        /// Search products with filter
        /// </summary>
        /// <param name="currentContent"></param>
        /// <param name="filterOptions"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        CustomSearchResult SearchWithFilters(IContent currentContent, FilterOptionViewModel filterOptions, IEnumerable<Filter> filters, int catalogId = 0);
        /// <summary>
        /// Search cms content, ignore products
        /// </summary>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        FindSearchContentModel SearchContent(FilterOptionViewModel filterOptions);
        /// <summary>
        /// Search PDF File, ignore products
        /// </summary>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        FindSearchContentModel SearchPDF(FilterOptionViewModel filterOptions);
        /// <summary>
        /// QuickSearch products
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<ProductTileViewModel> QuickSearch(string query);
        /// <summary>
        /// Quicksearch products
        /// </summary>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        IEnumerable<ProductTileViewModel> QuickSearch(FilterOptionViewModel filterOptions);
        /// <summary>
        /// Quicksearch both products and cms contents
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        CustomSearchResult QuickSearchAll(string query);
        /// <summary>
        /// Quicksearch both products and cms contents
        /// </summary>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        CustomSearchResult QuickSearchAll(FilterOptionViewModel filterOptions);
        
        IEnumerable<SortOrder> GetSortOrder();
    }
}