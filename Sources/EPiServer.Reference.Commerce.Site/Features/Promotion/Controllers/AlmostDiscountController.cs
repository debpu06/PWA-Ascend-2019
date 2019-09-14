using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Promotion.Blocks;
using EPiServer.Reference.Commerce.Site.Features.Promotion.ViewModels;
using EPiServer.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.Controllers
{
    [TemplateDescriptor(AvailableWithoutTag = true, Default = true, Inherited = false)]
    public class AlmostDiscountController : BlockController<AlmostDiscountBlock>
    {
        private readonly IPromotionEngine _promotionEngine;
        readonly IOrderGroupCalculator _orderGroupCalculator;
        private readonly ICartService _cartService;
        private ICart _cart;

        public AlmostDiscountController(ICartService cartService, IPromotionEngine promotionEngine, IOrderGroupCalculator orderGroupCalculator,
            IOrderRepository orderRepository)
        {
            _cartService = cartService;
            _promotionEngine = promotionEngine;
            _orderGroupCalculator = orderGroupCalculator;
        }

        public override ActionResult Index(AlmostDiscountBlock currentBlock)
        {
            if (Cart == null)
            {
                return null;
            }
            var model = new AlmostDiscountViewModel(currentBlock)
            {
                Rewards = CheckAlmostFulfilled(),
                Total = _orderGroupCalculator.GetSubTotal(Cart).Amount
            };

            return model.Rewards != null ? PartialView(model) : null;
        }

        private IEnumerable<RewardDescription> CheckAlmostFulfilled()
        {
            
            if (Cart == null)
            {
                return Enumerable.Empty<RewardDescription>();
            }
            return _promotionEngine.Run(Cart, new PromotionEngineSettings

            {
                ApplyReward = false,
                RequestedStatuses = RequestFulfillmentStatus.PartiallyFulfilled
            });
        }

        private ICart Cart => _cart ?? (_cart = _cartService.LoadCart(_cartService.DefaultCartName, true)?.Cart);
    }
}