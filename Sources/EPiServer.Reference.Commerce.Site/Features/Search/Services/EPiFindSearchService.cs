using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Commerce;
using EPiServer.Find.Framework;
using EPiServer.Find.Framework.Statistics;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.B2B.Models.Search;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Services;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Search.Services
{
    [ServiceConfiguration(typeof(IEPiFindSearchService), Lifecycle = ServiceInstanceScope.Transient)]
    public class EPiFindSearchService : IEPiFindSearchService
    {
        private readonly IPriceService _priceService;
        private readonly IPromotionService _promotionService;
        private readonly ICurrentMarket _currentMarket;
        private readonly ICurrencyService _currencyservice;
        private readonly LanguageResolver _languageResolver;

        public EPiFindSearchService(
            IPriceService priceService,
            IPromotionService promotionService,
            ICurrentMarket currentMarket,
            ICurrencyService currencyservice,
            LanguageResolver languageResolver)
        {
            _priceService = priceService;
            _promotionService = promotionService;
            _currentMarket = currentMarket;
            _currencyservice = currencyservice;
            _languageResolver = languageResolver;
        }

        public IEnumerable<UserSearchResultModel> SearchUsers(string query, int page = 1, int pageSize = 50)
        {
            var searchQuery = SearchClient.Instance.Search<UserSearchResultModel>();
            if (!String.IsNullOrEmpty(query))
            {
                searchQuery = searchQuery.For(query);
            }
            var results = searchQuery.Skip((page - 1) * pageSize).Take(pageSize).GetResult();
            if (results != null && results.Any())
                return results.Hits.AsEnumerable().Select(x => x.Document);
            return Enumerable.Empty<UserSearchResultModel>();
        }

        public IEnumerable<SkuSearchResultModel> SearchSkus(string query)
        {
            var market = _currentMarket.GetCurrentMarket();
            var currency = _currencyservice.GetCurrentCurrency();
            
            var searchQuery = SearchClient.Instance.Search<VariationContent>();
            searchQuery = searchQuery.Filter(x => x.Code.PrefixCaseInsensitive(query));
            searchQuery = searchQuery.FilterMarket(market);
            searchQuery = searchQuery.Filter(x => x.Language.Name.Match(_languageResolver.GetPreferredCulture().Name));
            searchQuery = searchQuery.Track();
            searchQuery = searchQuery.FilterForVisitor();
            var searchResults = searchQuery.GetContentResult();

            if (searchResults != null && searchResults.Any())
            {
                var searchResult = searchResults.Items;
                return searchResult.Select(variation =>
                {
                    var defaultPrice = _priceService.GetDefaultPrice(market.MarketId, DateTime.Now,
                        new CatalogKey(variation.Code), currency);
                    var discountedPrice = defaultPrice != null ? _promotionService.GetDiscountPrice(defaultPrice.CatalogKey, market.MarketId,
                        currency) : null;
                    return new SkuSearchResultModel
                    {
                        Sku = variation.Code,
                        ProductName = variation.DisplayName,
                        UnitPrice = discountedPrice?.UnitPrice.Amount ?? 0
                    };
                });
            }
            return Enumerable.Empty<SkuSearchResultModel>();
        }
    }
}