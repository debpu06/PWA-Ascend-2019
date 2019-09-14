namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.Discount
{
    /// <summary>
    /// Used to identify promotion types that use coupons issued by Episerver Campaign
    /// </summary>
    public interface ICampaignCouponDiscount
    {
        string CouponType { get; set; }
    }
}