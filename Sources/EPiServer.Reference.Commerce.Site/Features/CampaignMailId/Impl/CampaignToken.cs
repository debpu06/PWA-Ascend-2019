using EPiServer.ConnectForCampaign.Core.Configuration;
using EPiServer.ConnectForCampaign.Services.Implementation;
using EPiServer.Reference.Commerce.Site.Features.CampaignMailId.Interfaces;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignMailId.Impl
{
    /// <inheritdoc />
    [ServiceConfiguration(typeof(ICampaignToken), Lifecycle = ServiceInstanceScope.Singleton)]
    internal class CampaignToken : ICampaignToken
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
                _campaignSettings.Password, true);
        }
    }
}