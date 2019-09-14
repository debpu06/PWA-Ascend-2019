namespace EPiServer.Reference.Commerce.Site.Features.Campaign.Services
{
    public interface ICampaignService
    {
        void UpdateLastLoginDate(string email);
        void UpdateLastOrderDate();
        void UpdatePoint(int point);
        void AddNewRecipient(string email);
    }
}
