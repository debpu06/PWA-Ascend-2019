using EPiServer.Campaign.Extensions.CouponBlock;
using EPiServer.Campaign.Extensions.CouponCode;
using EPiServer.Campaign.Extensions.MailId;
using EPiServer.Campaign.Extensions.MailingReporting;
using EPiServer.Campaign.Extensions.MailingService;
using EPiServer.Campaign.Extensions.RecipientListService;
using EPiServer.Campaign.Extensions.SessionService;
using System.Configuration;
using System.ServiceModel;

namespace EPiServer.Campaign.Extensions
{
    public class CampaignSoapService : ICampaignSoapService
    {
        private readonly string _configUsername;
        private readonly string _configPassword;
        private readonly string _configClientid;

        // The Campaign API URLs are never going to change and this approach means there is 
        // no need to mess around with ugly bindings in .config. The bindings currently in 
        // Mosey don't work so this resolves that issue
        private readonly string baseApiUrl = "https://api.campaign.episerver.net";

        public CampaignSoapService()
        {
            _configUsername = ConfigurationManager.AppSettings["campaign:Username"];
            _configPassword = ConfigurationManager.AppSettings["campaign:Password"];
            _configClientid = ConfigurationManager.AppSettings["campaign:Clientid"];
        }

        public SessionWebserviceClient GetSessionWebserviceClient()
        {
            return new SessionWebserviceClient(
                new BasicHttpBinding(BasicHttpSecurityMode.Transport),
                new EndpointAddress(baseApiUrl + "/soap11/Session"));
        }

        public MailingWebserviceClient GetMailingWebserviceClient()
        {
            return new MailingWebserviceClient();
        }

        public RecipientListWebserviceClient GetRecipientListWebserviceClient()
        {
            return new RecipientListWebserviceClient(
                new BasicHttpBinding(BasicHttpSecurityMode.Transport),
                new EndpointAddress(baseApiUrl + "/soap11/RecipientList"));
        }

        public MailIdWebserviceClient GetMailIdClient()
        {
            return new MailIdWebserviceClient(
                new BasicHttpBinding(BasicHttpSecurityMode.Transport),
                new EndpointAddress(baseApiUrl + "/soap11/MailId"));
        }

        public CouponBlockWebserviceClient GetCouponBlockWebserviceClient()
        {
            return new CouponBlockWebserviceClient(
                new BasicHttpBinding(BasicHttpSecurityMode.Transport),
                new EndpointAddress(baseApiUrl + "/soap11/addons/CouponBlock"));
        }

        public CouponCodeWebserviceClient GetCouponCodeWebserviceClient()
        {
            return new CouponCodeWebserviceClient(
                new BasicHttpBinding(BasicHttpSecurityMode.Transport),
                new EndpointAddress(baseApiUrl + "/soap11/addons/CouponCode"));
        }

        public MailingReportingWebserviceClient GetMailingReportingWebserviceClient()
        {
            return new MailingReportingWebserviceClient();
        }
    }
}
