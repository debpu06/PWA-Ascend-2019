using EPiServer.Campaign.Extensions;
using EPiServer.ConnectForCampaign.Services.Implementation;
using EPiServer.Reference.Commerce.Site.Features.CampaignInitialisation;
using EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.ViewModels;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.Services
{
    [ServiceConfiguration(typeof(ICampaignTriggerService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CampaignTriggerService : ICampaignTriggerService
    {
        private readonly ICampaignSoapService _campaignSoapService;
        private readonly IAuthenticationService _authenticationService;
        private readonly string _clientId;
        private readonly string _username;
        private readonly string _password;

        public CampaignTriggerService(ICampaignSoapService campaignSoapService, IAuthenticationService authenticationService)
        {
            _campaignSoapService = campaignSoapService;
            _authenticationService = authenticationService;
            _clientId = ConfigurationManager.AppSettings[InitCampaignFromConfig.ConfigClientid];
            _username = ConfigurationManager.AppSettings[InitCampaignFromConfig.ConfigUsername];
            _password = ConfigurationManager.AppSettings[InitCampaignFromConfig.ConfigPassword];
        }

        public async Task<List<CampaignMail>> GetCampaignMailList(string mailingType)
        {
            var sessionId = _authenticationService.GetToken(_clientId, _username, _password, true);
            var mailIds = await _campaignSoapService.GetMailingWebserviceClient().getIdsAsync(sessionId, mailingType);

            var campaignMailList = new List<CampaignMail>();

            foreach (var id in mailIds.getIdsReturn)
            {
                campaignMailList.Add(new CampaignMail
                {
                    Id = id.ToString(),
                    Name = await _campaignSoapService.GetMailingWebserviceClient().getNameAsync(sessionId, id)
                });
            }

            return campaignMailList;
        }

        public async Task<List<CampaignRecipient>> GetCampaignRecipientList()
        {
            var sessionId = _authenticationService.GetToken(_clientId, _username, _password, true);
            var recipientIds = await _campaignSoapService.GetRecipientListWebserviceClient().getAllIdsAsync(sessionId);

            var campaignRecipientList = new List<CampaignRecipient>();

            foreach (var id in recipientIds.getAllIdsReturn)
            {
                campaignRecipientList.Add(new CampaignRecipient
                {
                    Id = id.ToString(),
                    Name = _campaignSoapService.GetRecipientListWebserviceClient().getName(sessionId, id),
                    IsTestList = _campaignSoapService.GetRecipientListWebserviceClient().isTestRecipientList(sessionId, id)
                });
            }

            return campaignRecipientList;
        }

        public async Task<int[]> SendTestMails(string mailingId, string recipientListId, string[] recipientId)
        {
            var sessionId = _authenticationService.GetToken(_clientId, _username, _password, true);
            var result = await _campaignSoapService.GetMailingWebserviceClient().sendTestMailsAsync(sessionId, long.Parse(mailingId), long.Parse(recipientListId), recipientId);
            return result.sendTestMailsReturn;
        }
    }
}