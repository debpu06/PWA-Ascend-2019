using EPiServer.Campaign.Extensions;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons
{
    [ServiceConfiguration(typeof(ICampaignCoupons))]
    public class CampaignCoupons : ICampaignCoupons
    {
        private readonly ICampaignToken _campaignToken;
        private readonly ICampaignSoapService _campaignSoapService;

        public CampaignCoupons(ICampaignToken campaignToken, 
            ICampaignSoapService campaignSoapService)
        {
            _campaignToken = campaignToken;
            _campaignSoapService = campaignSoapService;
        }

        public bool IsCouponAssigned(string userEmail, long couponBlockId, string couponCode)
        {
            var couponClient = _campaignSoapService.GetCouponCodeWebserviceClient();

            var assignedRecipientId = couponClient.getAssignedRecipientId(_campaignToken.GetToken(), couponBlockId, couponCode);
            return userEmail == assignedRecipientId;
        }

        public bool IsCouponUsed(long couponBlockId, string couponCode)
        {
            var couponClient = _campaignSoapService.GetCouponCodeWebserviceClient();

            return couponClient.isUsed(_campaignToken.GetToken(), couponBlockId, couponCode);
        }

        public void MarkCouponAsUsed(long couponBlockId, string couponCode)
        {
            var couponClient = _campaignSoapService.GetCouponCodeWebserviceClient();

            couponClient.markAsUsed(_campaignToken.GetToken(), couponBlockId, couponCode);
        }
    }
}