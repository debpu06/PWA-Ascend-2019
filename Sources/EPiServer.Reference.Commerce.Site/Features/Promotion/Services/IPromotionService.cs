using System.Collections.Generic;
using EPiServer.Commerce.Marketing;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Promotion.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.Services
{
    public interface IPromotionService
    {
        IEnumerable<PromotionViewModel> GetActivePromotions();
        List<ProductTileViewModel> GetProductsForPromotionByPromotionId(string promotionId, out PromotionData selectedPromotion);
    }
}
