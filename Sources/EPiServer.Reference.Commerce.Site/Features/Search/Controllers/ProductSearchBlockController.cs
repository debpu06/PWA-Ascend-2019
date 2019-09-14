using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Search.Blocks;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.Web.Mvc;
using Mediachase.Commerce;
using Mediachase.Search;
using Mediachase.Search.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Controllers
{
    public class ProductSearchResult
    {
        public string Heading { get; set; }
        public List<ProductTileViewModel> Products { get; set; }
    }

    public class ProductSearchBlockController : BlockController<ProductSearchBlock>
    {
        private readonly LanguageService _languageService;
        private readonly IReviewService _reviewService;
        private readonly ICurrentMarket _currentMarket;
        private readonly ICurrencyService _currencyService;

        public ProductSearchBlockController(LanguageService languageService,
            IReviewService reviewService,
            ICurrentMarket currentMarket,
            ICurrencyService currencyService)
        {
            _languageService = languageService;
            _reviewService = reviewService;
            _currentMarket = currentMarket;
            _currencyService = currencyService;
        }

        // GET: RelatedProductsBlock
        public override ActionResult Index(ProductSearchBlock currentContent)
        {
            var currentLang = _languageService.GetCurrentLanguage();

            CustomSearchResult result;
            try
            {
                result = currentContent.GetSearchResults(currentLang.Name);
            }
            catch (ServiceException)
            {
                return View("FindError");
            }

            if (result == null)
            {
                result = new CustomSearchResult
                {
                    ProductViewModels = Enumerable.Empty<ProductTileViewModel>(),
                    FacetGroups = Enumerable.Empty<FacetGroupOption>(),
                    SearchResult = new SearchResults(null, null)
                    {
                        FacetGroups = Enumerable.Empty<ISearchFacetGroup>().ToArray()
                    }
                };
            }

            MergePriorityProducts(currentContent, result);

            if (!result.ProductViewModels.Any())
            {
                return View("EmptyResult");
            }

            var productSearchResult = new ProductSearchResult
            {
                Heading = currentContent.Heading,
                Products = result.ProductViewModels.ToList()
            };

            return View(productSearchResult);
        }

        private void MergePriorityProducts(ProductSearchBlock currentContent, CustomSearchResult result)
        {
            var products = new List<EntryContentBase>();
            if (currentContent != null)
            {
                products = currentContent.PriorityProducts?.FilteredItems?.Select(x => x.GetContent() as EntryContentBase).ToList() ?? new List<EntryContentBase>();
            }

            products = products.Where(x => !result.ProductViewModels.Any(y => y.Code.Equals(x.Code)))
                .Select(x => x)
                .ToList();

            if (!products.Any())
            {
                return;
            }

            var market = _currentMarket.GetCurrentMarket();
            var currency = _currencyService.GetCurrentCurrency();
            var ratings = _reviewService.GetRatings(products.Select(x => x.Code)) ?? null;
            var newList = result.ProductViewModels.ToList();
            newList.InsertRange(0, products.Select(document => document.GetProductTileViewModel(market, currency, ratings)));
            result.ProductViewModels = newList;
        }
    }
}
