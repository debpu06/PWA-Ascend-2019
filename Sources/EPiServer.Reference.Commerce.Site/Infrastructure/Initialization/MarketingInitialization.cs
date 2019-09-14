using System;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Marketing.Promotions;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class MarketingInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            context.Locate.ContentEvents().SavingContent += Events_SavingContent;
        }

        private void Events_SavingContent(object sender, ContentEventArgs e)
        {
            var orderDiscount = e.Content as SpendAmountGetOrderDiscount;
            if (orderDiscount != null)
            {
                orderDiscount.Condition.PartiallyFulfilledThreshold = 0.75m;
                return;
            }

            var items = e.Content as SpendAmountGetGiftItems;
            if (items != null)
            {
                items.Condition.PartiallyFulfilledThreshold = 0.75m;
                return;
            }

            var itemDiscount = e.Content as SpendAmountGetItemDiscount;
            if (itemDiscount != null)
            {
                itemDiscount.Condition.PartiallyFulfilledThreshold = 0.75m;
                return;
            }

            var discount = e.Content as SpendAmountGetShippingDiscount;
            if (discount != null)
            {
                discount.Condition.PartiallyFulfilledThreshold = 0.75m;
                return;
            }

            var catalog = e.Content as CatalogContent;
            if (catalog != null && (catalog.Name.Equals("Smith Construction") || catalog.CatalogId == 4))
            {
                catalog.Owner = "2FB3117E-8AA0-469D-B0FF-56DD1A3329BF";
            }
        }

        private static void UpdateThresholds()
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var campaigns = contentLoader.GetChildren<SalesCampaign>(SalesCampaignFolder.CampaignRoot);
            foreach (var salesCampaign in campaigns)
            {
                var orderPromotions = contentLoader.GetChildren<OrderPromotion>(salesCampaign.ContentLink);
                foreach (var orderPromotion in orderPromotions)
                {
                    if (orderPromotion is SpendAmountGetOrderDiscount)
                    {
                        var clone = orderPromotion.CreateWritableClone() as SpendAmountGetOrderDiscount;
                        clone.Condition.PartiallyFulfilledThreshold = 0.75m;
                        contentRepository.Save(clone, SaveAction.Publish, AccessLevel.NoAccess);
                    }

                }

                var entryPromotions = contentLoader.GetChildren<EntryPromotion>(salesCampaign.ContentLink);
                foreach (var entryPromotion in entryPromotions)
                {
                    if (entryPromotion is SpendAmountGetGiftItems)
                    {
                        var clone = entryPromotion.CreateWritableClone() as SpendAmountGetGiftItems;
                        clone.Condition.PartiallyFulfilledThreshold = 0.75m;
                        contentRepository.Save(clone, SaveAction.Publish, AccessLevel.NoAccess);
                    }

                    if (entryPromotion is SpendAmountGetItemDiscount)
                    {
                        var clone = entryPromotion.CreateWritableClone() as SpendAmountGetItemDiscount;
                        clone.Condition.PartiallyFulfilledThreshold = 0.75m;
                        contentRepository.Save(clone, SaveAction.Publish, AccessLevel.NoAccess);
                    }
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            context.Locate.ContentEvents().SavingContent -= Events_SavingContent;
        }
    }
}