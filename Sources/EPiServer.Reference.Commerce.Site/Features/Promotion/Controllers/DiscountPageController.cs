using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Promotion.Pages;
using EPiServer.Reference.Commerce.Site.Features.Promotion.Services;
using EPiServer.Reference.Commerce.Site.Features.Promotion.ViewModels;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.Controllers
{
    public class DiscountPageController : PageController<DiscountPage>
    {
        private readonly IPromotionService _promotionService;

        public DiscountPageController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        public ActionResult Index(DiscountPage currentPage, string promotionId)
        {
            var model = new DiscountViewModel<DiscountPage>(currentPage)
            {
                Items = _promotionService.GetProductsForPromotionByPromotionId(promotionId, out var promotion),
                Promotion = promotion
            };

            return View(model);
        }
    }
}