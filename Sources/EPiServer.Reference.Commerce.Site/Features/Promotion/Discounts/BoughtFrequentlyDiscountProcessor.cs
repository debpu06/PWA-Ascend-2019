using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Security;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.Discounts
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Hybrid)]
    public class BoughtFrequentlyDiscountProcessor : EntryPromotionProcessorBase<BoughtFrequentlyDiscount>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IContentLoader _contentLoader;
        private readonly ReferenceConverter _referenceConverter;
        private readonly LocalizationService _localizationService;
        private readonly ICartService _cartService;
        private IList<IPurchaseOrder> _purchaseOrders = new List<IPurchaseOrder>();
        private IList<EntryContentBase> _entries = new List<EntryContentBase>();

        public BoughtFrequentlyDiscountProcessor(IOrderRepository orderRepository,
            IContentLoader contentLoader,
            ReferenceConverter referenceConverter,
            LocalizationService localizationService,
            RedemptionDescriptionFactory redemptionDescriptionFactory,
            CartService cartService) : base(redemptionDescriptionFactory)
        {
            _orderRepository = orderRepository;
            _contentLoader = contentLoader;
            _referenceConverter = referenceConverter;
            _localizationService = localizationService;
            _cartService = cartService;
        }

        protected override bool CanBeFulfilled(BoughtFrequentlyDiscount promotion, PromotionProcessorContext context)
        {
            return true;
        }

        protected override RewardDescription Evaluate(BoughtFrequentlyDiscount promotionData, PromotionProcessorContext context)
        {
            var fulfillmentStatus = GetFulfillmentStatus(promotionData);
            if (!fulfillmentStatus.HasFlag(FulfillmentStatus.Fulfilled))
            {
                return NotFulfilledRewardDescription(promotionData, context, fulfillmentStatus);
            }

            var lineItems = GetLineItems(context.OrderForm);
            var affectedCodes = lineItems.Where(x => _entries.Any(y => y.Code.Equals(x.Code)))
                .Select(x => x.Code)
                .ToList();

            if (!affectedCodes.Any())
            {
                return NotFulfilledRewardDescription(promotionData, context, FulfillmentStatus.NotFulfilled);
            }

            var redemptions = GetRedemptions(promotionData, context, affectedCodes);

            return RewardDescription.CreateMoneyOrPercentageRewardDescription(
                fulfillmentStatus,
                redemptions,
                promotionData,
                promotionData.Reward,
                context.OrderGroup.Currency,
                _localizationService);
        }

        protected override PromotionItems GetPromotionItems(BoughtFrequentlyDiscount promotionData)
        {
            var fulfillmentStatus = GetFulfillmentStatus(promotionData);
            if (!fulfillmentStatus.HasFlag(FulfillmentStatus.Fulfilled))
            {
                return new PromotionItems(promotionData, 
                    new CatalogItemSelection(null, CatalogItemSelectionType.All, true), 
                    new CatalogItemSelection(Enumerable.Empty<ContentReference>(), CatalogItemSelectionType.Specific, false));
            }
            return
                new PromotionItems(
                    promotionData,
                    new CatalogItemSelection(null, CatalogItemSelectionType.All, true),
                    new CatalogItemSelection(_entries.Select(x => x.ContentLink), CatalogItemSelectionType.Specific, true));
        }

        protected override RewardDescription NotFulfilledRewardDescription(BoughtFrequentlyDiscount promotionData, PromotionProcessorContext context, FulfillmentStatus fulfillmentStatus)
        {
            return RewardDescription.CreateMoneyOrPercentageRewardDescription(
                fulfillmentStatus,
                Enumerable.Empty<RedemptionDescription>(),
                promotionData,
                promotionData.Reward,
                context.OrderGroup.Currency,
                _localizationService);
        }

        protected override IEnumerable<RedemptionDescription> GetRedemptions(BoughtFrequentlyDiscount promotionData, PromotionProcessorContext context, IEnumerable<string> applicableCodes)
        {
            var affectedQuantity = Math.Min(decimal.MaxValue, GetLineItems(context.OrderForm).Where(li => applicableCodes.Contains(li.Code)).Sum(li => li.Quantity));
            var affectedEntries = context.EntryPrices.ExtractEntries(applicableCodes, affectedQuantity, promotionData);
            return affectedEntries != null
                ? new[] { CreateRedemptionDescription(affectedEntries) }
                : Enumerable.Empty<RedemptionDescription>();
        }

        protected FulfillmentStatus GetFulfillmentStatus(BoughtFrequentlyDiscount promotionData)
        {
            _purchaseOrders = _orderRepository.Load<IPurchaseOrder>(PrincipalInfo.Current.Principal.GetContactId(),
                _cartService.DefaultCartName)
                .Where(x => x.Created > DateTime.UtcNow.AddDays(-promotionData.Days))
                .ToList();

            var items = _purchaseOrders.SelectMany(x => x.GetAllLineItems()).
                GroupBy(x => x.Code);

            var codes = items.Where(x => x.Count() >= promotionData.NumberofPurchases)
                .Select(x => x.Key)
                .ToList();

            if (!codes.Any())
            {
                return FulfillmentStatus.NotFulfilled;
            }

            _entries = _contentLoader.GetItems(_referenceConverter.GetContentLinks(codes).Values, new LoaderOptions())
                .OfType<EntryContentBase>()
                .ToList();

            return FulfillmentStatus.Fulfilled;
        }
    }
}