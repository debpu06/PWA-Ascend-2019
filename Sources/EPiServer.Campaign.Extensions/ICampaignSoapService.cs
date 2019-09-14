using EPiServer.Campaign.Extensions.CouponBlock;
using EPiServer.Campaign.Extensions.CouponCode;
using EPiServer.Campaign.Extensions.MailingService;
using EPiServer.Campaign.Extensions.MailId;
using EPiServer.Campaign.Extensions.RecipientListService;
using EPiServer.Campaign.Extensions.SessionService;
using EPiServer.Campaign.Extensions.MailingReporting;

namespace EPiServer.Campaign.Extensions
{
    public interface ICampaignSoapService
    {
        SessionWebserviceClient GetSessionWebserviceClient();
        MailingWebserviceClient GetMailingWebserviceClient();
        RecipientListWebserviceClient GetRecipientListWebserviceClient();
        MailIdWebserviceClient GetMailIdClient();
        CouponBlockWebserviceClient GetCouponBlockWebserviceClient();
        CouponCodeWebserviceClient GetCouponCodeWebserviceClient();
        MailingReportingWebserviceClient GetMailingReportingWebserviceClient();
    }
}