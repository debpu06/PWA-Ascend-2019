using EPiServer.Commerce.Marketing;
using EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.Discount;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Coupons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.Reporting
{
    public class CouponUsageImpl : ICouponUsage
    {
        private readonly ICampaignCoupons _campaignCoupons;
        private readonly UniqueCouponService _uniqueCouponService;
        private readonly IContentRepository _contentRepository;

        public CouponUsageImpl(IContentRepository contentRepository, 
            ICampaignCoupons campaignCoupons, 
            UniqueCouponService uniqueCouponService)
        {
            _contentRepository = contentRepository;
            _campaignCoupons = campaignCoupons;
            _uniqueCouponService = uniqueCouponService;
        }

        public void Report(IEnumerable<PromotionInformation> appliedPromotions)
        {
            // This method allows us to report couple usage so we      
            // will look for all promotions with a coupon applied
            foreach (var promotion in appliedPromotions)
            {
                var content = _contentRepository.Get<PromotionData>(promotion.PromotionGuid);
                if (!CheckCampaign(content, promotion))
                {
                    CheckMultiple(content, promotion);
                }
            }
        }

        private bool CheckCampaign(PromotionData promotion, PromotionInformation promotionInformation)
        {
            var promotionData = promotion as SpendAmountGetOrderDiscountWithCoupon;
            long couponTypeId;
            if (promotionData == null || !long.TryParse(promotionData.CouponType, out couponTypeId))
            {
                return false;
            }

            // Safe to mark the coupon as used
            _campaignCoupons.MarkCouponAsUsed(couponTypeId, promotionInformation.CouponCode);
            return true;
        }

        private void CheckMultiple(PromotionData promotion, PromotionInformation promotionInformation)
        {
            var uniqueCodes = _uniqueCouponService.GetByPromotionId(promotion.ContentLink.ID);
            if (uniqueCodes == null || !uniqueCodes.Any())
            {
                return;
            }

            var uniqueCode = uniqueCodes.FirstOrDefault(x =>
                x.Code.Equals(promotionInformation.CouponCode, StringComparison.OrdinalIgnoreCase));
            if (uniqueCode == null)
            {
                return;
            }

            uniqueCode.UsedRedemptions++;
            _uniqueCouponService.SaveCoupons(new List<UniqueCoupon> {uniqueCode});
        }
    }
}