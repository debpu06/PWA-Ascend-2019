using System.Collections.Generic;
using System.Threading.Tasks;
using EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignTriggers.Services
{
    public interface ICampaignTriggerService
    {
        Task<List<CampaignMail>> GetCampaignMailList(string mailingType);
        Task<List<CampaignRecipient>> GetCampaignRecipientList();
        Task<int[]> SendTestMails(string mailingId, string recipientListId, string[] recipientId);
    }
}
