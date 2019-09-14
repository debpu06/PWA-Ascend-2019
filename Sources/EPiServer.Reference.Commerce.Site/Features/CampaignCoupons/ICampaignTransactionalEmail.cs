namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons
{
    public interface ICampaignTransactionalEmail
    {
        bool SendTrasactionalEmail(string authorisationCode, string email, string mailingId);
        bool SendTrasactionalEmail(string authorisationCode, string email, string mailingId, string htmlEmailContent);
    }
}