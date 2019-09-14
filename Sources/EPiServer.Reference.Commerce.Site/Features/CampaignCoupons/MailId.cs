using EPiServer.Campaign.Extensions;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons
{
    [ServiceConfiguration(typeof(IMailId))]
    public class MailId : IMailId
    {
        private readonly ICampaignToken _campaignToken;
        private readonly ICampaignSoapService _campaignSoapService;

        public MailId(ICampaignToken campaignToken,
            ICampaignSoapService campaignSoapService)
        {
            _campaignToken = campaignToken;
            _campaignSoapService = campaignSoapService;
        }

        public string getRecipientId(string mailId)
        {
            var mailIdClient = _campaignSoapService.GetMailIdClient();
            return mailIdClient.getRecipientId(_campaignToken.GetToken(), mailId);
        }

        public long getRecipientListId(string mailId)
        {
            var mailIdClient = _campaignSoapService.GetMailIdClient();
            return mailIdClient.getRecipientListId(_campaignToken.GetToken(), mailId);
        }

    }
}
