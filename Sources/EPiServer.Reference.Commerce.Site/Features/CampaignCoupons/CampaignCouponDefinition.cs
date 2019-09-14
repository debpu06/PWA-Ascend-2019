using System.Collections.Generic;
using EPiServer.Campaign.Extensions;
using EPiServer.Campaign.Extensions.CouponBlock;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons
{
    [ServiceConfiguration(typeof(ICampaignCouponDefinition))]
    public class CampaignCouponDefinition : ICampaignCouponDefinition
    {
        private readonly ICampaignToken _campaignToken;
        private readonly ICampaignSoapService _campaignSoapService;

        public CampaignCouponDefinition(ICampaignToken campaignToken, 
            ICampaignSoapService campaignSoapService)
        {
            _campaignToken = campaignToken;
            _campaignSoapService = campaignSoapService;
        }

        public IList<CouponDefinition> GetAllCouponDefinitions()
        {
            var couponBlockService = _campaignSoapService.GetCouponBlockWebserviceClient();

            var allCoupons = new List<CouponDefinition>();

            allCoupons.AddRange(
                ParseCoupons(couponBlockService, CouponDefinition.CouponSources.Manual));

            allCoupons.AddRange(
                ParseCoupons(couponBlockService, CouponDefinition.CouponSources.Automatic));

            return allCoupons;
        }

        private List<CouponDefinition> ParseCoupons(CouponBlockWebserviceClient couponBlockWebserviceClient,
            CouponDefinition.CouponSources couponSource)
        {
            var allCoupons = new List<CouponDefinition>();

            var couponSourceString = string.Empty;
            switch (couponSource)
            {
                case CouponDefinition.CouponSources.Manual:
                    couponSourceString = "static";
                    break;

                case CouponDefinition.CouponSources.Automatic:
                    couponSourceString = "generated";
                    break;
            }

            var allCouponIds = couponBlockWebserviceClient.getAllIds(_campaignToken.GetToken(), couponSourceString);
            if (allCouponIds != null)
            {
                foreach (var couponId in allCouponIds)
                {
                    var couponName = couponBlockWebserviceClient.getName(_campaignToken.GetToken(), couponId);
                    allCoupons.Add(new CouponDefinition()
                    {
                        CouponSource = couponSource,
                        CouponId = couponId,
                        CouponName = couponName
                    });
                }
            }

            return allCoupons;
        }
    }
}
