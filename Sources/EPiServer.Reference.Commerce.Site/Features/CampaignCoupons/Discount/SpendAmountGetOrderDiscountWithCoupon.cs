using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Marketing.DataAnnotations;
using EPiServer.Commerce.Marketing.Promotions;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.Discount
{
    [ContentType(GUID = "F6B2B6EC-AA3F-415D-8802-C3FE18DA74A8"
        , DisplayName = "Get a discount using a unique coupon code created by Episerver Campaign"
        , Description = "Used to give discounts for individual coupon codes that have been sent to the user by Episerver Campaign")]
    [AvailableContentTypes(Include = new[] {typeof(PromotionData)})]
    [ImageUrl("~/Features/CampaignCoupons/campaign.png")]
    public class SpendAmountGetOrderDiscountWithCoupon : SpendAmountGetOrderDiscount, ICampaignCouponDiscount
    {
        // Hide the default coupon code
        [ScaffoldColumn(false)]
        public override CouponData Coupon { get; set; }

        [PromotionRegion(PromotionRegionName.Condition)]
        [SelectOne(SelectionFactoryType = typeof(CouponCodeTypesSelectionFactory))]
        [Display(
            Order = 1,
            Name = "Coupon of this type has been sent to the user")]
        public virtual string CouponType { get; set; }
    }
}