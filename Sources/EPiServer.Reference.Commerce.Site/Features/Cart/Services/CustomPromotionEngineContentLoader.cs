using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Marketing.Internal;
using EPiServer.Commerce.Order;
using EPiServer.Commerce.Order.Internal;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Shared.Services;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;
using Mediachase.Commerce.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Services
{
    public class CustomPromotionEngineContentLoader : PromotionEngineContentLoader
    {
        private readonly IPriceService _priceService;
        private readonly ICurrentMarket _currentMarket;
        private readonly IContentLoader _contentLoader;

        public CustomPromotionEngineContentLoader(
                IContentLoader contentLoader, 
                CampaignInfoExtractor campaignInfoExtractor,
                IPriceService priceService,
                ReferenceConverter referenceConverter, ICurrentMarket currentMarket) 
            : base(contentLoader, campaignInfoExtractor, priceService, referenceConverter)
        {
            _contentLoader = contentLoader;
            _priceService = priceService;
            _currentMarket = currentMarket;
        }

        public override IOrderGroup CreateInMemoryOrderGroup(ContentReference entryLink, IMarket market, Currency marketCurrency)
        {
            var orderGroup = new InMemoryOrderGroup(market, marketCurrency);
            var entry =_contentLoader.Get<EntryContentBase>(entryLink);

            //this is where you can add your own pricing calculator to retrieve the right sale price
            var price = PriceCalculationService.GetSalePrice(entry.Code, market.MarketId, marketCurrency);

            if (price != null)
            {
                orderGroup.Forms.First().Shipments.First().LineItems.Add(new InMemoryLineItem
                {
                    Quantity = 1,
                    Code = price.CatalogKey.CatalogEntryCode,
                    PlacedPrice = price.UnitPrice.Amount
                });
            }

            return orderGroup;
        }

        public IPriceValue ExampleGetPrice(ContentReference varationContent)
        {
            var pricingTypeList = new List<CustomerPricing>();
            
            var variation = _contentLoader.GetItems(new List<ContentReference> { varationContent }, CultureInfo.CurrentCulture) as VariationContent;

            if (variation.IsNull)
                return null;

            //add all customers
            pricingTypeList.Add(new CustomerPricing(CustomerPricing.PriceType.AllCustomers, string.Empty));
            var currentUser = PrincipalInfo.CurrentPrincipal;
            if (currentUser != null)
            {
                if (!string.IsNullOrEmpty(currentUser.Identity.Name))
                {
                    pricingTypeList.Add(new CustomerPricing(CustomerPricing.PriceType.UserName, currentUser.Identity.Name));
                }

                var currentUserContact = currentUser.GetCustomerContact();
                if (currentUserContact != null && !string.IsNullOrEmpty(currentUserContact.EffectiveCustomerGroup))
                {
                    pricingTypeList.Add(new CustomerPricing(CustomerPricing.PriceType.PriceGroup, currentUserContact.EffectiveCustomerGroup));
                }
            }

            var catalogKey = new CatalogKey(variation.Code);
            Currency[] currencies = { "USD", "SEK" };
            var priceFilter = new PriceFilter()
            {
                Quantity = 1m,
                Currencies = currencies,
                CustomerPricing = pricingTypeList
            };

            var priceValues =
                _priceService.GetPrices(_currentMarket.GetCurrentMarket().MarketId, DateTime.UtcNow, catalogKey, priceFilter);

            var priceValue = priceValues.OrderBy(pv => pv.UnitPrice).FirstOrDefault();

            return priceValue;
        }

    }
}
