using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Personalization.Commerce.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Search.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using Mediachase.Search;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Catalog;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using StartPage = EPiServer.Reference.Commerce.Site.Features.Start.Pages.StartPage;

namespace EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories
{
    public class SearchViewModelFactory
    {
        private readonly ISearchService _searchService;
        private readonly LocalizationService _localizationService;
        private readonly IContentLoader _contentLoader;
        private readonly ReferenceConverter _referenceConverter;
        private readonly UrlResolver _urlResolver;
        private readonly HttpContextBase _httpContextBase;

        public SearchViewModelFactory(LocalizationService localizationService, ISearchService searchService,
            IContentLoader contentLoader,
            ReferenceConverter referenceConverter,
            UrlResolver urlResolver, 
            HttpContextBase httpContextBase)
        {
            _searchService = searchService;
            _contentLoader = contentLoader;
            _referenceConverter = referenceConverter;
            _urlResolver = urlResolver;
            _httpContextBase = httpContextBase;
            _localizationService = localizationService;
        }

        public virtual SearchViewModel<T> Create<T>(T currentContent, FilterOptionViewModel viewModel, string selectedFacets, int catalogId = 0) where T : IContent
        {
            if (viewModel.Q != null && (viewModel.Q.StartsWith("*") || viewModel.Q.StartsWith("?")))
            {
                return new SearchViewModel<T>
                {
                    CurrentContent = currentContent,
                    FilterOption = viewModel,
                    HasError = true,
                    ErrorMessage = _localizationService.GetString("/Search/BadFirstCharacter"),
                    Recommendations = new List<Recommendation>(),
                    CategoriesFilter = new CategoriesFilterViewModel(),
                    ShowRecommendations = true
                };
            }

            var customSearchResult = _searchService.Search(currentContent, viewModel, selectedFacets, catalogId);

            viewModel.TotalCount = customSearchResult.TotalCount;
            viewModel.FacetGroups = customSearchResult.FacetGroups.ToList();

            viewModel.Sorting = _searchService.GetSortOrder().Select(x => new SelectListItem
            {
                Text = _localizationService.GetString("/Category/Sort/" + x.Name),
                Value = x.Name.ToString(),
                Selected = string.Equals(x.Name.ToString(), viewModel.Sort)
            });
            var productRecommendations = currentContent as IProductRecommendations;
            return new SearchViewModel<T>
            {
                CurrentContent = currentContent,
                ProductViewModels = customSearchResult.ProductViewModels,
                Facets = customSearchResult.SearchResult != null ? customSearchResult.SearchResult.FacetGroups : new ISearchFacetGroup[0],
                FilterOption = viewModel,
                Recommendations = new List<Recommendation>(),
                CategoriesFilter = GetCategoriesFilter(currentContent),
                DidYouMeans = customSearchResult.DidYouMeans,
                Query = viewModel.Q,
                ShowRecommendations = productRecommendations != null ? productRecommendations.ShowRecommendations : true,
                IsMobile = _httpContextBase.GetOverriddenBrowser().IsMobileDevice
            };
        }

        private CategoriesFilterViewModel GetCategoriesFilter(IContent currentContent)
        {
            var catalogId = 0;
            var node = currentContent as NodeContent;
            if (node != null)
            {
                catalogId = node.CatalogId;
            }
            var catalog = _contentLoader.GetChildren<CatalogContentBase>(_referenceConverter.GetRootLink())
                .FirstOrDefault(x => catalogId == 0 && IsCategoryForSearch(x) || x.CatalogId == catalogId);

            if (catalog == null)
            {
                return new CategoriesFilterViewModel();
            }

            var viewModel = new CategoriesFilterViewModel();
            foreach (var nodeContent in _contentLoader.GetChildren<NodeContent>(catalog.ContentLink))
            {
                var nodeFilter = new CategoryFilter
                {
                    DisplayName = nodeContent.DisplayName,
                    Url = _urlResolver.GetUrl(nodeContent.ContentLink),
                    IsActive = currentContent != null && currentContent.ContentLink == nodeContent.ContentLink
                };
                viewModel.Categories.Add(nodeFilter);

                var nodeChildren = _contentLoader.GetChildren<NodeContent>(nodeContent.ContentLink);
                foreach (var nodeChild in nodeChildren)
                {
                    var nodeChildFilter = new CategoryFilter
                    {
                        DisplayName = nodeChild.DisplayName,
                        Url = _urlResolver.GetUrl(nodeChild.ContentLink),
                        IsActive = currentContent != null && currentContent.ContentLink == nodeChild.ContentLink
                    };

                    nodeFilter.Children.Add(nodeChildFilter);
                    if (nodeChildFilter.IsActive)
                    {
                        nodeFilter.IsActive = true;
                    }

                    var nodeChildrenOfNodeChild = _contentLoader.GetChildren<NodeContent>(nodeChild.ContentLink);
                    foreach (var nodeChildOfChild in nodeChildrenOfNodeChild)
                    {
                        var nodeChildOfChildFilter = new CategoryFilter
                        {
                            DisplayName = nodeChildOfChild.DisplayName,
                            Url = _urlResolver.GetUrl(nodeChildOfChild.ContentLink),
                            IsActive = currentContent != null && currentContent.ContentLink == nodeChildOfChild.ContentLink
                        };

                        nodeChildFilter.Children.Add(nodeChildOfChildFilter);
                        if (nodeChildOfChildFilter.IsActive)
                        {
                            nodeFilter.IsActive = nodeChildFilter.IsActive = true;
                        }
                    }
                }

            }
            return viewModel;
        }

        /// <summary>
        /// Each site should search product of its site
        /// This function check catalog content is belong current site or not?
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        private static bool IsCategoryForSearch(CatalogContentBase catalog)
        {
            IContentRepository factory = DataFactory.Instance;
            var navigationRoot = factory.Get<PageData>(Web.SiteDefinition.Current.StartPage);
            if (navigationRoot is MoseySupplyStartPage)
            {
                return catalog.Name == Infrastructure.Constants.CategoryName.MoseySupply;
            }
            else if (navigationRoot is StartPage)
            {
                return catalog.Name == Infrastructure.Constants.CategoryName.Fashion;
            }

            return true;
        }
    }
}