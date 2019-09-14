using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CampaignDashboard.Models;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CampaignDashboard.Services
{
    public interface ICampaignDashboardService
    {
        List<EmailsSent> GetEmailsSentQuantity();
        int GetRecipientListQuantity();
    }
}