using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Api.Querying;
using EPiServer.Find.Api.Querying.Filters;
using EPiServer.Find.Cms;
using EPiServer.Find.Commerce;
using EPiServer.Find.Framework.BestBets;
using EPiServer.Find.Framework.Statistics;
//using EPiServer.Find.Personalization;
using EPiServer.Find.Statistics;
using EPiServer.Find.UI;
using EPiServer.Find.UnifiedSearch;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Facets;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Social.Common.Exceptions;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Reference.Commerce.Site.Infrastructure.Indexing;
using EPiServer.Security;
using EPiServer.Web;
using Mediachase.Commerce;
using Mediachase.Search;
using Mediachase.Search.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using Mediachase.Commerce.Security;
using Mediachase.Commerce.Catalog;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Media;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Shared;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Services
{
    public class FindSearchService : ISearchService
    {
        private readonly ICurrentMarket _currentMarket;
        private readonly ICurrencyService _currencyService;
        private readonly LanguageResolver _languageResolver;
        private readonly SearchFacade _search;
        private readonly IClient _findClient;
        private readonly IFacetRegistry _facetRegistry;
        private readonly IReviewService _reviewService;
        private const int DefaultPageSize = 18;
        private readonly IFindUIConfiguration _findUIConfiguration;
        private readonly ReferenceConverter _referenceConverter;
        private readonly IContentRepository _contentRepository;

        public FindSearchService(ICurrentMarket currentMarket,
            ICurrencyService currencyService,
            LanguageResolver languageResolver,
            IClient findClient,
            SearchFacade search,
            IFacetRegistry facetRegistry,
            IFindUIConfiguration findUIConfiguration,
            IReviewService reviewService,
            ReferenceConverter referenceConverter,
            IContentRepository contentRepository
            )
        {
            _currentMarket = currentMarket;
            _currencyService = currencyService;
            _languageResolver = languageResolver;
            _findClient = findClient;
            _search = search;
            _facetRegistry = facetRegistry;
            _reviewService = reviewService;
            _findUIConfiguration = findUIConfiguration;
            //_findClient.Personalization().Refresh();
            _referenceConverter = referenceConverter;
            _contentRepository = contentRepository;
        }

        public CustomSearchResult Search(IContent currentContent,
            FilterOptionViewModel filterOptions,
            string selectedFacets,
            int catalogId = 0)
        {
            return filterOptions == null ? CreateEmptyResult() : GetSearchResults(currentContent, filterOptions, selectedFacets, null, catalogId);
        }

        public CustomSearchResult SearchWithFilters(IContent currentContent,
            FilterOptionViewModel filterOptions,
            IEnumerable<Filter> filters,
            int catalogId = 0)
        {
            return filterOptions == null ? CreateEmptyResult() : GetSearchResults(currentContent, filterOptions, "", filters, catalogId);
        }

        public IEnumerable<ProductTileViewModel> QuickSearch(string query)
        {
            var filterOptions = new FilterOptionViewModel
            {
                Q = query,
                PageSize = 5,
                Sort = string.Empty,
                FacetGroups = new List<FacetGroupOption>(),
                Page = 1,
                TrackData = false
            };
            return QuickSearch(filterOptions);
        }

        /// <summary>
        /// Quicksearch cms content, ignore products
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public FindSearchContentModel QuickSearchContent(string query)
        {
            var filterOptions = new FilterOptionViewModel
            {
                Q = query,
                PageSize = 5,
                Page = 1,
                TrackData = false
            };
            return SearchContent(filterOptions);
        }

        public FindSearchContentModel QuickSearchPDF(string query)
        {
            var filterOptions = new FilterOptionViewModel
            {
                Q = query,
                PageSize = 5,
                Page = 1,
                TrackData = false
            };
            return SearchPDF(filterOptions);
        }

        public IEnumerable<ProductTileViewModel> QuickSearch(FilterOptionViewModel filterOptions)
        {
            return string.IsNullOrEmpty(filterOptions.Q) ? Enumerable.Empty<ProductTileViewModel>() : GetSearchResults(null, filterOptions, "").ProductViewModels;
        }

        public IEnumerable<SortOrder> GetSortOrder()
        {
            var market = _currentMarket.GetCurrentMarket();
            var currency = _currencyService.GetCurrentCurrency();

            return new List<SortOrder>
            {
                new SortOrder {Name = ProductSortOrder.PriceAsc, Key = IndexingHelper.GetPriceField(market.MarketId, currency), SortDirection = SortDirection.Ascending},
                new SortOrder {Name = ProductSortOrder.Popularity, Key = "", SortDirection = SortDirection.Ascending},
                new SortOrder {Name = ProductSortOrder.NewestFirst, Key = "created", SortDirection = SortDirection.Descending}
            };
        }

        private CustomSearchResult GetSearchResults(IContent currentContent,
            FilterOptionViewModel filterOptions,
            string selectedfacets,
            IEnumerable<Filter> filters = null,
            int catalogId = 0)
        {

            //If contact belong organization, only find product that belong the categories that has owner is this organization
            var contact = PrincipalInfo.CurrentPrincipal.GetCustomerContact();
            var organizationId = contact?.ContactOrganization?.PrimaryKeyId ?? Guid.Empty;
            CatalogContent catalogOrganization = null;
            if (organizationId != Guid.Empty)
            {
                //get category that has owner id = organizationId
                catalogOrganization = _contentRepository.GetChildren<CatalogContent>(_referenceConverter.GetRootLink()).FirstOrDefault(x => (!string.IsNullOrEmpty(x.Owner) && x.Owner.ToLower().Equals(organizationId.ToString().ToLower())));
            }

            var pageSize = filterOptions.PageSize > 0 ? filterOptions.PageSize : DefaultPageSize;
            var market = _currentMarket.GetCurrentMarket();

            var query = _findClient.Search<EntryContentBase>();
            query = ApplyTermFilter(query, filterOptions.Q, filterOptions.TrackData);
            query = query.Filter(x => x.Language.Name.Match(_languageResolver.GetPreferredCulture().Name));

            if (organizationId != Guid.Empty && catalogOrganization != null)
            {
                query = query.Filter(x => x.Outline().PrefixCaseInsensitive(catalogOrganization.Name));
            }

            var nodeContent = currentContent as NodeContent;
            if (nodeContent != null)
            {
                var outline = _search.GetOutline(nodeContent.Code);
                query = query.FilterOutline(new[] { outline });
            }

            query = query.FilterMarket(market);
            var facetQuery = query;

            query = FilterSelected(query, filterOptions.FacetGroups);
            query = ApplyFilters(query, filters);
            query = OrderBy(query, filterOptions);
            //Exclude products from search
            query = query.Filter(x => (x as BaseProduct).ExcludeFromSearch.Match(false));

            if (catalogId != 0)
            {
                query = query.Filter(x => x.CatalogId.Match(catalogId));
            }

            query = query.ApplyBestBets()
                .Skip((filterOptions.Page - 1) * pageSize)
                .Take(pageSize)
                .StaticallyCacheFor(TimeSpan.FromMinutes(1));

            var result = query.GetContentResult();

            return new CustomSearchResult
            {
                ProductViewModels = CreateProductViewModels(result, currentContent, filterOptions.Q),
                FacetGroups = GetFacetResults(filterOptions.FacetGroups, facetQuery, selectedfacets),
                SearchResult = new SearchResults(null, null)
                {
                    FacetGroups = Enumerable.Empty<ISearchFacetGroup>().ToArray(),
                },
                TotalCount = result.TotalMatching,
                DidYouMeans = string.IsNullOrEmpty(filterOptions.Q) ? null : _findClient.Statistics().GetDidYouMean(filterOptions.Q),
                Query = filterOptions.Q,
            };

        }

        /// <summary>
        /// Search content of cms, ignore EntryContentBase
        /// </summary>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        public FindSearchContentModel SearchContent(FilterOptionViewModel filterOptions)
        {
            var model = new FindSearchContentModel();
            model.FilterOption = filterOptions;
            var siteId = SiteDefinition.Current.Id;

            if (!string.IsNullOrWhiteSpace(filterOptions.Q))
            {
                var query =
                    _findClient.UnifiedSearchFor(filterOptions.Q, _findClient.Settings.Languages.GetSupportedLanguage(ContentLanguage.PreferredCulture) ??
                                                  Language.None)
                                .UsingSynonyms()
                                //Include a facet whose value we can use to show the total number of hits
                                //regardless of section. The filter here is irrelevant but should match *everything*.
                                .TermsFacetFor(x => x.SearchSection)
                                .FilterFacet("AllSections", x => x.SearchSection.Exists())
                                .Filter(x => (x.MatchTypeHierarchy(typeof(IContent)) & (((IContent)x).SiteId().Match(siteId.ToString()))) | !x.MatchTypeHierarchy(typeof(IContent)))
                                .Filter(x => !x.MatchType(typeof(EntryContentBase)))
                                //Fetch the specific paging page.
                                .Skip((filterOptions.Page - 1) * filterOptions.PageSize)
                                .Take(filterOptions.PageSize)
                                //Allow editors (from the Find/Optimizations view) to push specific hits to the top 
                                //for certain search phrases.
                                .ApplyBestBets();

                //Exclude content from search
                query = query.Filter(x => (x as SitePageData).ExcludeFromSearch.Match(false));

                // obey DNT
                var doNotTrackHeader = System.Web.HttpContext.Current.Request.Headers.Get("DNT");
                // Should Not track when value equals 1
                if ((doNotTrackHeader == null || doNotTrackHeader.Equals("0")) && filterOptions.TrackData)
                {
                    query = query.Track();
                }

                //If a section filter exists (in the query string) we apply
                //a filter to only show hits from a given section.
                if (!string.IsNullOrWhiteSpace(filterOptions.SectionFilter))
                {
                    query = query.FilterHits(x => x.SearchSection.Match(filterOptions.SectionFilter));
                }

                //We can (optionally) supply a hit specification as argument to the GetResult
                //method to control what each hit should contain. Here we create a 
                //hit specification based on values entered by an editor on the search page.
                var hitSpec = new HitSpecification
                {
                    HighlightTitle = true, // filterOptions.HighlightTitle,
                    HighlightExcerpt = true, // filterOptions.HighlightExcerpt
                };

                //Execute the query and populate the Result property which the markup (aspx)
                //will render.
                model.Hits = query.GetResult(hitSpec);
                model.FilterOption.TotalCount = model.Hits.TotalMatching;
            }

            return model;
        }

        /// <summary>
        /// Search content of cms, ignore EntryContentBase
        /// </summary>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        public FindSearchContentModel SearchPDF(FilterOptionViewModel filterOptions)
        {
            var model = new FindSearchContentModel();
            model.FilterOption = filterOptions;
            var siteId = SiteDefinition.Current.Id;

            if (!string.IsNullOrWhiteSpace(filterOptions.Q))
            {
                var query =
                    _findClient.UnifiedSearchFor(filterOptions.Q, _findClient.Settings.Languages.GetSupportedLanguage(ContentLanguage.PreferredCulture) ??
                                                  Language.None)
                                .UsingSynonyms()
                                //Include a facet whose value we can use to show the total number of hits
                                //regardless of section. The filter here is irrelevant but should match *everything*.
                                .TermsFacetFor(x => x.SearchSection)
                                .FilterFacet("AllSections", x => x.SearchSection.Exists())
                                .Filter(x => x.MatchType(typeof(PDFFile)))
                                //Fetch the specific paging page.
                                .Skip((filterOptions.Page - 1) * filterOptions.PageSize)
                                .Take(filterOptions.PageSize)
                                //Allow editors (from the Find/Optimizations view) to push specific hits to the top 
                                //for certain search phrases.
                                .ApplyBestBets();

                // obey DNT
                var doNotTrackHeader = System.Web.HttpContext.Current.Request.Headers.Get("DNT");
                // Should Not track when value equals 1
                if ((doNotTrackHeader == null || doNotTrackHeader.Equals("0")) && filterOptions.TrackData)
                {
                    query = query.Track();
                }

                //If a section filter exists (in the query string) we apply
                //a filter to only show hits from a given section.
                if (!string.IsNullOrWhiteSpace(filterOptions.SectionFilter))
                {
                    query = query.FilterHits(x => x.SearchSection.Match(filterOptions.SectionFilter));
                }

                //We can (optionally) supply a hit specification as argument to the GetResult
                //method to control what each hit should contain. Here we create a 
                //hit specification based on values entered by an editor on the search page.
                var hitSpec = new HitSpecification
                {
                    HighlightTitle = true, // filterOptions.HighlightTitle,
                    HighlightExcerpt = true, // filterOptions.HighlightExcerpt
                };

                //Execute the query and populate the Result property which the markup (aspx)
                //will render.
                model.Hits = query.GetResult(hitSpec);
                model.FilterOption.TotalCount = model.Hits.TotalMatching;
            }

            return model;
        }

        public IEnumerable<ProductTileViewModel> CreateProductViewModels(IContentResult<EntryContentBase> searchResult, IContent content, string searchQuery)
        {
            List<ProductTileViewModel> productViewModels;
            var market = _currentMarket.GetCurrentMarket();
            var currency = _currencyService.GetCurrentCurrency();

            if (searchResult == null)
            {
                throw new ArgumentNullException(nameof(searchResult));
            }

            try
            {
                var ratings = _reviewService.GetRatings(searchResult.Select(x => x.Code));
                productViewModels = searchResult.Select(document => document.GetProductTileViewModel(market, currency, ratings)).ToList();
                ApplyBoostedProperties(ref productViewModels, searchResult, content, searchQuery);
                return productViewModels;
            }
            catch (SocialRepositoryException)
            {
                //DO SOMETHING
            }

            productViewModels = searchResult.Select(document => document.GetProductTileViewModel(market, currency, new List<ReviewStatisticsViewModel>())).ToList();
            ApplyBoostedProperties(ref productViewModels, searchResult, content, searchQuery);
            return productViewModels;
        }

        private static ITypeSearch<EntryContentBase> ApplyTermFilter(ITypeSearch<EntryContentBase> query, string searchTerm, bool trackData)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return query;
            }

            query = query.For(searchTerm);
            if (trackData)
            {
                query = query.Track();
            }

            return query;
        }

        private ITypeSearch<EntryContentBase> OrderBy(ITypeSearch<EntryContentBase> query, FilterOptionViewModel filterOptionViewModel)
        {
            if (string.IsNullOrEmpty(filterOptionViewModel.Sort) || filterOptionViewModel.Sort.Equals("Position"))
            {
                if (filterOptionViewModel.SortDirection.Equals("Asc"))
                {
                    query = query.OrderBy(x => x.SortOrder());
                    return query;
                }
                query = query.OrderByDescending(x => x.SortOrder());
                return query;
            }

            if (filterOptionViewModel.Sort.Equals("Price"))
            {
                if (filterOptionViewModel.SortDirection.Equals("Asc"))
                {
                    query = query.OrderBy(x => x.DefaultPrice());
                    return query;
                }
                query = query.OrderByDescending(x => x.DefaultPrice());
                return query;
            }

            if (filterOptionViewModel.Sort.Equals("Name"))
            {
                if (filterOptionViewModel.SortDirection.Equals("Asc"))
                {
                    query = query.OrderBy(x => x.DisplayName);
                    return query;
                }
                query = query.OrderByDescending(x => x.DisplayName);
                return query;
            }

            //if (filterOptionViewModel.Sort.Equals("Recommended"))
            //{
            //    query = query.UsingPersonalization();
            //    return query;
            //}

            return query;
        }

        private IEnumerable<FacetGroupOption> GetFacetResults(List<FacetGroupOption> options,
            ITypeSearch<EntryContentBase> query,
            string selectedfacets)
        {
            if (options == null)
            {
                return Enumerable.Empty<FacetGroupOption>();
            }

            var facets = _facetRegistry.GetFacetDefinitions();
            var facetGroups = facets.Select(x => new FacetGroupOption
            {
                GroupFieldName = x.FieldName,
                GroupName = x.DisplayName,

            }).ToList();

            query = facets.Aggregate(query, (current, facet) => facet.Facet(current, GetSelectedFilter(options, facet.FieldName)));

            var productFacetsResult = query.Take(0).GetContentResult();
            if (productFacetsResult.Facets == null)
            {
                return facetGroups;
            }

            foreach (var facetGroup in facetGroups)
            {
                var filter = facets.FirstOrDefault(x => x.FieldName.Equals(facetGroup.GroupFieldName));
                if (filter == null)
                {
                    continue;
                }

                var facet = productFacetsResult.Facets.FirstOrDefault(x => x.Name.Equals(facetGroup.GroupFieldName));
                if (facet == null)
                {
                    continue;
                }

                filter.PopulateFacet(facetGroup, facet, selectedfacets);
            }
            return facetGroups;
        }

        private Filter GetSelectedFilter(List<FacetGroupOption> options, string currentField)
        {
            var filters = new List<Filter>();
            var facets = _facetRegistry.GetFacetDefinitions();
            foreach (var facetGroupOption in options)
            {
                if (facetGroupOption.GroupFieldName.Equals(currentField))
                {
                    continue;
                }

                var filter = facets.FirstOrDefault(x => x.FieldName.Equals(facetGroupOption.GroupFieldName));
                if (filter == null)
                {
                    continue;
                }

                if (!facetGroupOption.Facets.Any(x => x.Selected))
                {
                    continue;
                }

                if (filter is FacetStringDefinition)
                {
                    filters.Add(new TermsFilter(_findClient.GetFullFieldName(facetGroupOption.GroupFieldName, typeof(string)),
                        facetGroupOption.Facets.Where(x => x.Selected).Select(x => FieldFilterValue.Create(x.Name))));
                }
                else if (filter is FacetStringListDefinition)
                {
                    var termFilters = facetGroupOption.Facets.Where(x => x.Selected)
                        .Select(s => new TermFilter(facetGroupOption.GroupFieldName, FieldFilterValue.Create(s.Name)))
                        .Cast<Filter>()
                        .ToList();

                    filters.AddRange(termFilters);
                }
                else if (filter is FacetNumericRangeDefinition)
                {
                    var rangeFilters = filter as FacetNumericRangeDefinition;
                    foreach (var selectedRange in facetGroupOption.Facets.Where(x => x.Selected))
                    {
                        var rangeFilter = rangeFilters.Range.FirstOrDefault(x => x.Id.Equals(selectedRange.Key.Split(':')[1]));
                        if (rangeFilter == null)
                        {
                            continue;
                        }
                        filters.Add(RangeFilter.Create(_findClient.GetFullFieldName(facetGroupOption.GroupFieldName, typeof(double)),
                            rangeFilter.From ?? 0,
                            rangeFilter.To ?? double.MaxValue));
                    }
                }
            }

            if (!filters.Any())
            {
                return null;
            }

            if (filters.Count == 1)
            {
                return filters.FirstOrDefault();
            }

            var boolFilter = new BoolFilter();
            foreach (var filter in filters)
            {
                boolFilter.Should.Add(filter);
            }
            return boolFilter;

        }

        private ITypeSearch<T> FilterSelected<T>(ITypeSearch<T> query, List<FacetGroupOption> options)
        {
            var facets = _facetRegistry.GetFacetDefinitions();

            foreach (var facetGroupOption in options)
            {
                var filter = facets.FirstOrDefault(x => x.FieldName.Equals(facetGroupOption.GroupFieldName));
                if (filter == null)
                {
                    continue;
                }

                if (facetGroupOption.Facets != null && !facetGroupOption.Facets.Any(x => x.Selected))
                {
                    continue;
                }

                if (filter is FacetStringDefinition)
                {
                    var stringFilter = filter as FacetStringDefinition;
                    query = stringFilter.Filter(query, facetGroupOption.Facets
                        .Where(x => x.Selected)
                        .Select(x => x.Name).ToList());
                }
                else if (filter is FacetStringListDefinition)
                {
                    var stringListFilter = filter as FacetStringListDefinition;
                    query = stringListFilter.Filter(query, facetGroupOption.Facets
                        .Where(x => x.Selected)
                        .Select(x => x.Name).ToList());
                }
                else if (filter is FacetNumericRangeDefinition)
                {
                    var numericFilter = filter as FacetNumericRangeDefinition;
                    var ranges = new List<SelectableNumericRange>();
                    var selectedFacets = facetGroupOption.Facets.Where(x => x.Selected);
                    foreach (var facetOption in selectedFacets)
                    {
                        var range = numericFilter.Range.FirstOrDefault(x => x.Id.Equals(facetOption.Key.Split(':')[1]));
                        if (range == null)
                        {
                            continue;
                        }
                        ranges.Add(new SelectableNumericRange
                        {
                            From = range.From,
                            Id = range.Id,
                            Selected = range.Selected,
                            To = range.To
                        });
                    }

                    query = numericFilter.Filter(query, ranges);
                }
            }
            return query;
        }

        private ITypeSearch<EntryContentBase> ApplyFilters(ITypeSearch<EntryContentBase> query,
            IEnumerable<Filter> filters)
        {
            if (filters == null || !filters.Any())
            {
                return query;
            }

            foreach (var filter in filters)
            {
                query = query.Filter(filter);
            }
            return query;

        }

        private static CustomSearchResult CreateEmptyResult()
        {
            return new CustomSearchResult
            {
                ProductViewModels = Enumerable.Empty<ProductTileViewModel>(),
                FacetGroups = Enumerable.Empty<FacetGroupOption>(),
                SearchResult = new SearchResults(null, null)
                {
                    FacetGroups = Enumerable.Empty<ISearchFacetGroup>().ToArray()
                }
            };
        }

        /// <summary>
        /// Sets Featured Product property and Best Bet Product property to ProductViewModels.
        /// </summary>
        /// <param name="searchResult">The search result (product list).</param>
        /// <param name="currentContent">The product category.</param>
        /// <param name="searchQuery">The search query string to filter Best Bet result.</param>
        /// <param name="productViewModels">The ProductViewModels is added two properties: Featured Product and Best Bet.</param>
        private void ApplyBoostedProperties(ref List<ProductTileViewModel> productViewModels, IContentResult<EntryContentBase> searchResult, IContent currentContent, string searchQuery)
        {
            var node = currentContent as FashionNode;
            var products = new List<EntryContentBase>();

            if (node != null)
            {
                products = node.FeaturedProducts?.FilteredItems?.Select(x => x.GetContent() as EntryContentBase).ToList() ?? new List<EntryContentBase>();

                var featuredProductList = productViewModels.Where(v => products.Any(p => p.ContentLink.ID == v.ProductId)).ToList();
                featuredProductList.ForEach(x => { x.IsFeaturedProduct = true; });

                productViewModels.RemoveAll(v => products.Any(p => p.ContentLink.ID == v.ProductId));
                productViewModels.InsertRange(0, featuredProductList);
            }

            var bestBetList = new BestBetRepository().List().Where(i => i.PhraseCriterion.Phrase.CompareTo(searchQuery) == 0);
            //Filter for product best bet only.
            var productBestBet = bestBetList.Where(i => i.BestBetSelector is CommerceBestBetSelector);
            productViewModels.ToList()
                             .ForEach(p =>
                             {
                                 if (productBestBet.Any(i => ((CommerceBestBetSelector)i.BestBetSelector).ContentLink.ID == p.ProductId))
                                 {
                                     p.IsBestBetProduct = true;
                                 }
                             });
        }

        /// <summary>
        /// Quicksearch both products, cms contents and PDF files
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public CustomSearchResult QuickSearchAll(string query)
        {
            var startPage = _contentRepository.Get<BaseStartPage>(ContentReference.StartPage);
            var result = new CustomSearchResult();

            if (startPage.ShowProductSearchResults == true)
            {
                result.ProductViewModels = QuickSearch(query);
            }
            if (startPage.ShowArticleSearchResults == true)
            {
                result.ContentResult = QuickSearchContent(query);
            }
            if (startPage.ShowPDFSearchResults == true)
            {
                result.PdfResult = QuickSearchPDF(query);
            }

            return result;
        }

        /// <summary>
        /// Quicksearch both products and cms contents
        /// </summary>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        public CustomSearchResult QuickSearchAll(FilterOptionViewModel filterOptions)
        {
            CustomSearchResult result = new CustomSearchResult();
            result.ProductViewModels = QuickSearch(filterOptions);
            result.ContentResult = SearchContent(filterOptions);
            return result;
        }
    }
}