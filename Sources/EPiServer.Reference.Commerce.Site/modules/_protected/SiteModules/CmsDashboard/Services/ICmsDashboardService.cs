using EPiServer.Marketing.Testing.Core.DataClass;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Services
{
    public interface ICmsDashboardService
    {
        Task<List<CmsDashboardModel>> GetAmountOfFormsSubmitted();
        Task<List<SiteAudit>> GetSiteAudits();
        Task<List<ContentTypeAudit>> GetPageTypeAudits();
        Task<List<ContentTypeAudit>> GetBlockTypeAudits();
        Task<List<VGAudit>> GetVisitorGroupAudit();
        Task<List<CmsDashboardModel>> GetBounceRates();
        Task<List<CmsDashboardModel>> GetVisitors();
        Task<List<CmsDashboardModel>> GetTopLocations();
        Task<List<CmsDashboardModel>> GetTopLandingPages();
        Task<List<IMarketingTest>> GetAbTestContents();
        Task<List<ABTestingModel>> GetAbTestingMostParticipationPercentage();
        Task<List<ABTestingModel>> GetAbTestingMostPageViews();
        Task<List<ABTestingModel>> GetAbTestingMostConversions();
    }
}
