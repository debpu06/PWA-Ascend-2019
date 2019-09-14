using System;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.Pages;
using EPiServer.Reference.Commerce.Site.Features.Search.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Catalog;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content;
using EPiServer.Web.Routing;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using EPiServer.Web.Mvc.Html;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Controllers
{
    public class SearchController : PageController<SearchPage>
    {
        private readonly SearchViewModelFactory _viewModelFactory;
        private readonly ISearchService _searchService;
        private readonly IRecommendationService _recommendationService;
        private readonly ReferenceConverter _referenceConverter;
        private readonly IProfileStoreService _profileStoreService;
        private readonly HttpContextBase _httpContextBase;
        private readonly IContentLoader _contentLoader;
        private readonly IContentService _contentService;
        private readonly UrlResolver _urlResolver;

        public SearchController(
            SearchViewModelFactory viewModelFactory,
            ISearchService searchService,
            IRecommendationService recommendationService,
            ReferenceConverter referenceConverter,
            IProfileStoreService profileStoreService,
            HttpContextBase httpContextBase,
            IContentLoader contentLoader,
            IContentService contentService,
            UrlResolver urlResolver)
        {
            _viewModelFactory = viewModelFactory;
            _searchService = searchService;
            _recommendationService = recommendationService;
            _referenceConverter = referenceConverter;
            _profileStoreService = profileStoreService;
            _httpContextBase = httpContextBase;
            _contentLoader = contentLoader;
            _contentService = contentService;
            _urlResolver = urlResolver;
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public async Task<ActionResult> Index(SearchPage currentPage, FilterOptionViewModel filterOptions)
        {
            if(filterOptions.Q == null)
            {
                return Redirect(Url.ContentUrl(ContentReference.StartPage));
            }
            var products = _contentService.GetByContentType(22, "en", "Code");
            var matchProduct = products.FirstOrDefault(x => x.Properties["Code"].ToString().ToLower() == filterOptions.Q.ToLower());
            if (matchProduct != null)
            {
                return Redirect(_urlResolver.GetUrl(matchProduct.ContentReference));
            }

            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            var viewModel = _viewModelFactory.Create(currentPage, filterOptions, HttpContext.Request.QueryString["facets"], startPage.SearchCatalog);
            //var browsedItems = ContentExtensions.GetBrowseHistory();
            //var orderedProducts = viewModel.ProductViewModels.ToList().OrderByDescending(x => Convert.ToInt32(browsedItems.Any(y => y.Code.Equals(x.Code))));
            //viewModel.ProductViewModels = orderedProducts;

            var bestBestList = viewModel.ProductViewModels.Where(x => x.IsBestBetProduct);
            var notBestBestList = viewModel.ProductViewModels.Where(x => !x.IsBestBetProduct);
            viewModel.ProductViewModels = bestBestList.Union(notBestBestList);

            if (filterOptions.Page <= 1 && HttpContext.Request.HttpMethod == "GET")
            {
                var trackingResult =
                    await _recommendationService.TrackSearch(HttpContext, filterOptions.Q, filterOptions.PageSize,
                        viewModel.ProductViewModels.Select(x => x.Code));
                viewModel.Recommendations = trackingResult.GetSearchResultRecommendations(_referenceConverter);
            }
            _profileStoreService.TrackSearch(currentPage, _httpContextBase, filterOptions.Q);

            if (!startPage.ShowProductSearchResults)
            {
                viewModel.ProductViewModels = null;
            }

            if (startPage.ShowArticleSearchResults)
            {
                try
                {
                    viewModel.ContentResult = _searchService.SearchContent(new FilterOptionViewModel()
                    {
                        Q = filterOptions.Q,
                        PageSize = 5,
                        Page = filterOptions.SearchContent ? filterOptions.Page : 1
                    });
                }
                catch (Exception)
                {
                    viewModel.ContentResult = null;
                }
                
            }

            if (startPage.ShowPDFSearchResults)
            {
                viewModel.PdfResult = _searchService.SearchPDF(new FilterOptionViewModel()
                {
                    Q = filterOptions.Q,
                    PageSize = 5,
                    Page = filterOptions.SearchPdf ? filterOptions.Page : 1
                });
            }

            var productCount = viewModel.ProductViewModels?.Count() ?? 0;
            var contentCount = viewModel.ContentResult?.NumberOfHits ?? 0;
            var pdfCount = viewModel.PdfResult?.NumberOfHits ?? 0;

            if (productCount + contentCount + pdfCount == 1)
            {
                if (productCount == 1)
                {
                    var product = viewModel.ProductViewModels.FirstOrDefault();
                    return Redirect(product.Url);
                }
                if (contentCount == 1)
                {
                    var content = viewModel.ContentResult.Hits.FirstOrDefault();
                    return Redirect(content.Url);
                }
                if (pdfCount == 1)
                {
                    var pdf = viewModel.PdfResult.Hits.FirstOrDefault();
                    return Redirect(pdf.Url);
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult QuickSearch(string search = "")
        {
            var redirectUrl = "";
            var products = _contentService.GetByContentType(22, "en", "Code");
            var matchProduct = products.FirstOrDefault(x => x.Properties["Code"].ToString().ToLower() == search.ToLower());
            if (matchProduct != null)
            {
                redirectUrl = _urlResolver.GetUrl(matchProduct.ContentReference);
                return View("_QuickSearchAll", new CustomSearchResult
                {
                    RedirectUrl = redirectUrl
                });
            }

            var result = _searchService.QuickSearchAll(search);

            var totalResult = result.ProductViewModels?.Count() + result.ContentResult?.NumberOfHits + result.PdfResult?.NumberOfHits;
            if (totalResult != null && totalResult == 1)
            {
                if (result.ProductViewModels.Count() == 1)
                {
                    var product = result.ProductViewModels.FirstOrDefault();
                    redirectUrl = product.Url;
                }
                if (result.ContentResult.NumberOfHits == 1)
                {
                    var content = result.ContentResult.Hits.FirstOrDefault();
                    redirectUrl = content.Url;
                }
                if (result.PdfResult.NumberOfHits == 1)
                {
                    var pdf = result.PdfResult.Hits.FirstOrDefault();
                    redirectUrl = pdf.Url;
                }
            }
            result.RedirectUrl = redirectUrl;

            return View("_QuickSearchAll", result);
        }

        [ChildActionOnly]
        public ActionResult Facet(SearchPage currentPage, FilterOptionViewModel viewModel)
        {
            return PartialView("_Facet", viewModel);
        }

        [ChildActionOnly]
        public ActionResult RecentlyBrowsed(SearchPage currentPage)
        {
            return PartialView("_RecentlyBrowsed", ContentExtensions.GetBrowseHistory());
        }
    }
}