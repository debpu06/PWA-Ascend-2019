using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Services;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EPiServer.Reference.Commerce.Site.Extensions
{
    public static class ProductContentExtensions
    {
        private static readonly Lazy<IPromotionService> PromotionService = new Lazy<IPromotionService>(() => ServiceLocator.Current.GetInstance<IPromotionService>());
        private static readonly Lazy<AssetUrlResolver> AssetUrlResolver = new Lazy<AssetUrlResolver>(() => ServiceLocator.Current.GetInstance<AssetUrlResolver>());
        private static readonly Lazy<UrlResolver> UrlResolver = new Lazy<UrlResolver>(() => ServiceLocator.Current.GetInstance<UrlResolver>());
        private static readonly Lazy<EPiServer.Commerce.Shell.Catalog.ReadOnlyPricingLoader> PricingLoader = new Lazy<EPiServer.Commerce.Shell.Catalog.ReadOnlyPricingLoader>(() => ServiceLocator.Current.GetInstance<EPiServer.Commerce.Shell.Catalog.ReadOnlyPricingLoader>());
        private static readonly Lazy<IRelationRepository> RelationRepository = new Lazy<IRelationRepository>(() => ServiceLocator.Current.GetInstance<IRelationRepository>());
        private static readonly Lazy<IContentLoader> ContentLoader = new Lazy<IContentLoader>(() => ServiceLocator.Current.GetInstance<IContentLoader>());
        private static readonly Lazy<SearchFacade> SearchFacade = new Lazy<SearchFacade>(() => ServiceLocator.Current.GetInstance<SearchFacade>());
        private static readonly Lazy<EPiServer.Commerce.Shell.Catalog.InventoryLoader> InventoryLoader = new Lazy<EPiServer.Commerce.Shell.Catalog.InventoryLoader>(() => ServiceLocator.Current.GetInstance<EPiServer.Commerce.Shell.Catalog.InventoryLoader>());

        public static IEnumerable<Inventory> Inventories(this EntryContentBase entryContentBase)
        {
            return Inventories(
                entryContentBase,
                ContentLoader.Value,
                InventoryLoader.Value,
                RelationRepository.Value);
        }

        public static IEnumerable<Inventory> Inventories(this EntryContentBase entryContentBase, 
            IContentLoader contentLoader,
            EPiServer.Commerce.Shell.Catalog.InventoryLoader inventoryLoader, 
            IRelationRepository relationRepository)
        {
            var productContent = entryContentBase as ProductContent;
            if (productContent != null)
            {
                var variations = contentLoader.GetItems(productContent.GetVariants(relationRepository), productContent.Language).OfType<VariationContent>();
                return variations.SelectMany(EPiServer.Commerce.Shell.Catalog.StockPlacementExtensions.GetStockPlacements);
            }

            var packageContent = entryContentBase as PackageContent;
            if (packageContent != null)
            {
                return EPiServer.Commerce.Shell.Catalog.StockPlacementExtensions.GetStockPlacements(packageContent);
            }

            var variationContent = entryContentBase as VariantBase;
            return variationContent != null ? EPiServer.Commerce.Shell.Catalog.StockPlacementExtensions.GetStockPlacements(variationContent) : Enumerable.Empty<Inventory>();
        }

        public static decimal DefaultPrice(this EntryContentBase entryContentBase)
        {
            return DefaultPrice(entryContentBase, PricingLoader.Value, RelationRepository.Value);
        }

        public static decimal DefaultPrice(this EntryContentBase entryContentBase,
            EPiServer.Commerce.Shell.Catalog.ReadOnlyPricingLoader pricingLoader, 
            IRelationRepository relationRepository)
        {
            var maxPrice = new Price();
            var productContent = entryContentBase as ProductContent;
            if (productContent != null)
            {
                var variationLinks = productContent.GetVariants(relationRepository);
                foreach (var variationLink in variationLinks)
                {
                    var defaultPrice = pricingLoader.GetDefaultPrice(variationLink);
                    if (defaultPrice.UnitPrice.Amount > maxPrice.UnitPrice.Amount)
                    {
                        maxPrice = defaultPrice;
                    }
                }
                return maxPrice.UnitPrice.Amount;
            }

            var packageContent = entryContentBase as PackageContent;
            if (packageContent != null)
            {
                return pricingLoader.GetDefaultPrice(packageContent.ContentLink)?.UnitPrice.Amount ?? 0m; 
            }

            var variationContent = entryContentBase as VariantBase;
            if (variationContent != null)
            {
                return pricingLoader.GetDefaultPrice(variationContent.ContentLink)?.UnitPrice.Amount ?? 0m;
            }

            return 0m;
        }

        public static IEnumerable<Price> Prices(this EntryContentBase productContent)
        {
            return Prices(productContent, PricingLoader.Value, RelationRepository.Value);
        }

        public static IEnumerable<Price> Prices(this EntryContentBase entryContentBase,
            EPiServer.Commerce.Shell.Catalog.ReadOnlyPricingLoader pricingLoader, 
            IRelationRepository relationRepository)
        {
            var productContent = entryContentBase as ProductContent;
            if (productContent != null)
            {
                var variationLinks = productContent.GetVariants(relationRepository);
                return variationLinks.SelectMany(variationLink => pricingLoader.GetPrices(variationLink, null, Enumerable.Empty<CustomerPricing>()));
            }

            var packageContent = entryContentBase as PackageContent;
            if (packageContent != null)
            {
                return pricingLoader.GetPrices(packageContent.ContentLink, null, Enumerable.Empty<CustomerPricing>());
            }

            var variationContent = entryContentBase as VariantBase;
            return variationContent != null ? pricingLoader.GetPrices(variationContent.ContentLink, null, Enumerable.Empty<CustomerPricing>()) : Enumerable.Empty<Price>();
        }

        public static IEnumerable<VariantBase> VariationContents(this ProductContent productContent)
        {
            return VariationContents(productContent, ContentLoader.Value, RelationRepository.Value);
        }

        public static IEnumerable<VariantBase> VariationContents(this ProductContent productContent, 
            IContentLoader contentLoader, 
            IRelationRepository relationRepository)
        {
            return contentLoader.GetItems(productContent.GetVariants(relationRepository), productContent.Language).OfType<VariantBase>();
        }

        public static IEnumerable<string> Outline(this EntryContentBase productContent)
        {
            return Outline(productContent,
                ContentLoader.Value,
                SearchFacade.Value);
        }

        public static IEnumerable<string> Outline(this EntryContentBase productContent, IContentLoader contentLoader, SearchFacade searchFacade)
        {
            var nodes = contentLoader.GetItems(productContent.GetNodeRelations().
                    Select(x => x.Parent), productContent.Language).
                OfType<NodeContent>();

            return nodes.Select(x => searchFacade.GetOutline(x.Code));
        }

        public static int SortOrder(this EntryContentBase productContent)
        {
            var node = productContent.GetNodeRelations().FirstOrDefault();
            return node?.SortOrder ?? 0;
        }

        public static CatalogKey GetCatalogKey(this EntryContentBase productContent)
        {
            return new CatalogKey(productContent.Code);
        }

        public static double GetRatingAverage(this BaseProduct content)
        {
            IReviewService socialRatingRepository =
                ServiceLocator.Current.GetInstance<IReviewService>();
            return socialRatingRepository.Get(content.Code).Statistics.OverallRating;
        }

        public static ProductTileViewModel GetProductTileViewModel(this EntryContentBase product, IMarket market, Currency currency, IEnumerable<ReviewStatisticsViewModel> ratings)
        {
            var placedPrice = product.Prices()
                .Where(id => id.MarketId == market.MarketId)
                .OrderBy(sort => sort.UnitPrice.Amount)
                .FirstOrDefault(x => x.UnitPrice.Currency == currency);
            var productRecommendations = product as IProductRecommendations;

            return new ProductTileViewModel
            {
                ProductId = product.ContentLink.ID,
                Brand = product.Property.Keys.Contains("Brand") ? product.Property["Brand"]?.Value?.ToString() ?? "" : "",
                Code = product.Code,
                DisplayName = product.DisplayName,
                Description = product.Property.Keys.Contains("Description") ? product.Property["Description"]?.Value != null ? ((XhtmlString)product.Property["Description"].Value).ToHtmlString() : "" : "",
                LongDescription = ShortenLongDescription(product.Property.Keys.Contains("LongDescription") ? product.Property["LongDescription"]?.Value != null ? ((XhtmlString)product.Property["LongDescription"].Value).ToHtmlString() : "" : ""),
                PlacedPrice = placedPrice?.UnitPrice ?? new Money(0, currency),
                DiscountedPrice = placedPrice == null ? new Money(0, currency) : PromotionService.Value.GetDiscountPrice(placedPrice.EntryContent.GetCatalogKey(), market.MarketId, currency).UnitPrice,
                FirstVariationCode = placedPrice != null && placedPrice.EntryContent is VariationContent ? (placedPrice.EntryContent as VariationContent).Code : "",
                ImageUrl = AssetUrlResolver.Value.GetAssetUrl<IContentImage>(product),
                Url = UrlResolver.Value.GetUrl(product.ContentLink),
                IsAvailable = product.Prices().Where(price => price.MarketId == market.MarketId)
                    .Any(x => x.UnitPrice.Currency == currency),
                ReviewStatistics = ratings.FirstOrDefault(x => x.Code.Equals(product.Code)) ?? new ReviewStatisticsViewModel(),
                OnSale = product.Property.Keys.Contains("OnSale") && ((bool?)product.Property["OnSale"]?.Value ?? false),
                NewArrival = product.Property.Keys.Contains("NewArrival") && ((bool?)product.Property["NewArrival"]?.Value ?? false),
                ShowRecommendations = productRecommendations != null ? productRecommendations.ShowRecommendations : true
            };
        }

        private static string ShortenLongDescription(string LongDescription)
        {
            MatchCollection wordColl = Regex.Matches(LongDescription, @"[\S]+");
            StringBuilder sb = new StringBuilder();

            if (wordColl.Count > 40)
            {
                foreach (var subWord in wordColl.Cast<Match>().Select(r => r.Value).Take(40))
                {
                    sb.Append(subWord);
                    sb.Append(" ");
                }
            }

            return (sb.Length > 0) ? sb.Append("...").ToString() : "";
        }
    }
}