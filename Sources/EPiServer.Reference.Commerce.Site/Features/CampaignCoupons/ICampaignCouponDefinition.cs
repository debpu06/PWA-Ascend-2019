using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons
{
    public interface ICampaignCouponDefinition
    {
        IList<CouponDefinition> GetAllCouponDefinitions();
    }
}