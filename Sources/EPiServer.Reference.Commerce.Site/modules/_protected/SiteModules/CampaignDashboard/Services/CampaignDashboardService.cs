using EPiServer.Campaign.Extensions;
using EPiServer.ConnectForCampaign.Services.Implementation;
using EPiServer.Reference.Commerce.Site.Features.CampaignInitialisation;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CampaignDashboard.Models;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Configuration;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CampaignDashboard.Services
{
    [ServiceConfiguration(typeof(ICampaignDashboardService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CampaignDashboardService : ICampaignDashboardService
    {
        private readonly ICampaignSoapService _campaignSoapService;
        private readonly IAuthenticationService _authenticationService;
        private readonly string _clientId;
        private readonly string _username;
        private readonly string _password;

        public CampaignDashboardService(ICampaignSoapService campaignSoapService, IAuthenticationService authenticationService)
        {
            _campaignSoapService = campaignSoapService;
            _authenticationService = authenticationService;
            _clientId = ConfigurationManager.AppSettings[InitCampaignFromConfig.ConfigClientid];
            _username = ConfigurationManager.AppSettings[InitCampaignFromConfig.ConfigUsername];
            _password = ConfigurationManager.AppSettings[InitCampaignFromConfig.ConfigPassword];
        }

        public List<EmailsSent> GetEmailsSentQuantity()
        {
            // Hardcode for mailingId list
            var mailingIds = new long[] { 232416793581, 208892186976, 232660893289 };
            var result = new List<EmailsSent>();
            var sessionId = _authenticationService.GetToken(_clientId, _username, _password, true);

            foreach (var mailingId in mailingIds)
            {
                result.Add(new EmailsSent()
                {
                    EmailName = _campaignSoapService.GetMailingWebserviceClient().getName(sessionId, mailingId),
                    EmailQuantity = _campaignSoapService.GetMailingReportingWebserviceClient().getSentRecipientCount(sessionId, mailingId),
                    ClickCount = _campaignSoapService.GetMailingReportingWebserviceClient().getClickCount(sessionId, mailingId, false)
                });
            }

            return result;
        }

        public int GetRecipientListQuantity()
        {
            var sessionId = _authenticationService.GetToken(_clientId, _username, _password, true);
            return _campaignSoapService.GetRecipientListWebserviceClient().getCount(sessionId, true);
        }
    }
}