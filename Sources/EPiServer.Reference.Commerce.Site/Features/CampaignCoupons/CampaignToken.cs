using EPiServer.ConnectForCampaign.Core.Configuration;
using EPiServer.ConnectForCampaign.Services.Implementation;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons
{
    [ServiceConfiguration(typeof(ICampaignToken), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CampaignToken : ICampaignToken
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICampaignSettings _campaignSettings;

        public CampaignToken(IAuthenticationService authenticationService, ICampaignSettings campaignSettings)
        {
            _authenticationService = authenticationService;
            _campaignSettings = campaignSettings;
        }

        public string GetToken()
        {
            return _authenticationService.GetToken(
                _campaignSettings.MandatorId, 
                _campaignSettings.UserName,
                _campaignSettings.Password, false);
        }
    }
}