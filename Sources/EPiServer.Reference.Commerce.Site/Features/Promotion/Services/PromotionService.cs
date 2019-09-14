using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Marketing.Promotions;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Product.Services;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Promotion.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Search.Models;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.Services
{
    [ServiceConfiguration(typeof(IPromotionService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class PromotionService : IPromotionService
    {
        private readonly IContentLoader _contentLoader;
        private readonly ICurrentMarket _currentMarket;
        private readonly IProductService _productService;
        private readonly MarketContentLoader _marketContentFilter;
        private readonly SearchViewModelFactory _searchViewModelFactory;

        public PromotionService(IContentLoader contentLoader, ICurrentMarket currentMarket, IProductService productService, MarketContentLoader marketContentFilter, SearchViewModelFactory searchViewModelFactory)
        {
            _contentLoader = contentLoader;
            _currentMarket = currentMarket;
            _productService = productService;
            _marketContentFilter = marketContentFilter;
            _searchViewModelFactory = searchViewModelFactory;
        }

        public IEnumerable<PromotionViewModel> GetActivePromotions()
        {
            var promotions = new List<PromotionViewModel>();

            var promotionItemGroups = _marketContentFilter.GetPromotionItemsForMarket(_currentMarket.GetCurrentMarket()).GroupBy(x => x.Promotion);

            foreach (var promotionGroup in promotionItemGroups)
            {
                var promotionItems = promotionGroup.First();
                promotions.Add(new PromotionViewModel()
                {
                    Name = promotionGroup.Key.Name,
                    BannerImage = promotionGroup.Key.Banner,
                    SelectionType = promotionItems.Condition.Type,
                    Items = GetProductsForPromotion(promotionItems).Take(3)
                });
            }

            return promotions;
        }

        public List<ProductTileViewModel> GetProductsForPromotionByPromotionId(string promotionId, out PromotionData selectedPromotion)
        {
            var models = new List<ProductTileViewModel>();
            selectedPromotion = _marketContentFilter.GetPromotions().FirstOrDefault(o => o.ContentGuid == new System.Guid(promotionId));

            if (selectedPromotion == null)
            {
                return models;
            }

            var pd = selectedPromotion.Property.FirstOrDefault(p => p.PropertyValueType == typeof(DiscountItems));

            if (!(pd?.Value is DiscountItems dicsouItems))
            {
                return models;
            }

            foreach (var conditionBlock in dicsouItems.Items)
            {
                ProductTileViewModel model;

                if (_contentLoader.TryGet(conditionBlock, out ProductContent product))
                {
                    model = _productService.GetProductTileViewModel(product);
                    models.Add(model);
                    continue;
                }

                if (_contentLoader.TryGet(conditionBlock, out VariationContent variation))
                {
                    model = _productService.GetProductTileViewModel(variation);
                    models.Add(model);
                    continue;
                }

                if (!_contentLoader.TryGet(conditionBlock, out NodeContent node))
                {
                    continue;
                }

                var nodeContent = _searchViewModelFactory.Create(node, new FilterOptionViewModel { FacetGroups = new List<FacetGroupOption>(), Page = 1, PageSize = 9 }, null);

                models.AddRange(nodeContent.ProductViewModels);
            }

            return models;
        }

        private IEnumerable<CatalogContentBase> GetProductsForPromotion(PromotionItems itemsOnPromotion)
        {
            var conditionProducts = new List<CatalogContentBase>();

            foreach (var conditionItemReference in itemsOnPromotion.Condition.Items)
            {
                var conditionItem = _contentLoader.Get<CatalogContentBase>(conditionItemReference);
                AddIfProduct(conditionItem, conditionProducts);

                if (conditionItem is NodeContentBase nodeContent)
                {
                    AddItemsRecursive(nodeContent, itemsOnPromotion, conditionProducts);
                }
            }

            return conditionProducts;
        }

        private void AddItemsRecursive(NodeContentBase nodeContent, PromotionItems itemsOnPromotion, List<CatalogContentBase> conditionProducts)
        {
            foreach (var child in _contentLoader.GetChildren<CatalogContentBase>(nodeContent.ContentLink))
            {
                AddIfProduct(child, conditionProducts);

                var childNode = child as NodeContentBase;
                if (childNode != null && itemsOnPromotion.Condition.IncludesSubcategories)
                {
                    AddItemsRecursive(childNode, itemsOnPromotion, conditionProducts);
                }
            }
        }

        private static void AddIfProduct(CatalogContentBase content, List<CatalogContentBase> productsInPromotion)
        {
            if (content is ProductContent)
            {
                productsInPromotion.Add(content);
            }
        }
    }
}