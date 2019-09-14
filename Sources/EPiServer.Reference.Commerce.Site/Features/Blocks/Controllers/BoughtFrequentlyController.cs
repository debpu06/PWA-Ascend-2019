using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Promotion.Discounts;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    public class BoughtFrequentlyController : BlockController<BoughtFrequentlyBlock>
    {
        private readonly BoughtFrequentlyDiscountProcessor _boughtFrequentlyDiscountProcessor;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentModelUsage _contentModelUsage;
        private readonly IContentLoader _contentLoader;

        public BoughtFrequentlyController(BoughtFrequentlyDiscountProcessor boughtFrequentlyDiscountProcessor,
            IContentTypeRepository contentTypeRepository,
            IContentModelUsage contentModelUsage,
            IContentLoader contentLoader)
        {
            _boughtFrequentlyDiscountProcessor = boughtFrequentlyDiscountProcessor;
            _contentTypeRepository = contentTypeRepository;
            _contentModelUsage = contentModelUsage;
            _contentLoader = contentLoader;
        }

        public override ActionResult Index(BoughtFrequentlyBlock currentBlock)
        {
            var discounts = GetDiscounts();
            var entries = new Dictionary<MonetaryReward, List<EntryContentBase>>();
            foreach (var discount in discounts)
            {
                
                var contentLinks = _boughtFrequentlyDiscountProcessor.GetPromotionItems(discount).Reward.Items;
                var items = _contentLoader.GetItems(contentLinks, new LoaderOptions())
                    .OfType<EntryContentBase>()
                    .ToList();

                entries.Add(discount.Reward, items);
            }
            var viewModel = new BoughtFrequentlyViewModel(currentBlock)
            {
                Entries = entries
            };
            return PartialView("~/Views/Blocks/BoughtFrequently.cshtml", viewModel);
        }

        private List<BoughtFrequentlyDiscount> GetDiscounts()
        {
            var fieldType = _contentTypeRepository.Load<BoughtFrequentlyDiscount>();
            var contentUsages = _contentModelUsage.ListContentOfContentType(fieldType);
            var discounts = _contentLoader.GetItems(contentUsages.Select(x => x.ContentLink.ToReferenceWithoutVersion()), 
                new LoaderOptions())
                .OfType<BoughtFrequentlyDiscount>()
                .ToList();
                    
            return discounts;
        }

    }
}
