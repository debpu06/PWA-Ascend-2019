namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons
{
    public interface IMailId
    {
        string getRecipientId(string mailId);
        long getRecipientListId(string mailId);
    }
}