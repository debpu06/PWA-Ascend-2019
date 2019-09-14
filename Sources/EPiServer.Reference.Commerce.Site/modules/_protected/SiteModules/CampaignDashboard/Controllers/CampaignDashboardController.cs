using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CampaignDashboard.Services;
using EPiServer.Shell.Navigation;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CampaignDashboard.Controllers
{
    public class CampaignDashboardController : Controller
    {
        private readonly ICampaignDashboardService _campaignDashboardService;

        public CampaignDashboardController(ICampaignDashboardService campaignDashboardService)
        {
            _campaignDashboardService = campaignDashboardService;
        }

        [MenuItem("/global/campaign/campaigndashboard", TextResourceKey = "/Shared/CampaignDashboard")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetEmailsSentQuantity()
        {
            var data = _campaignDashboardService.GetEmailsSentQuantity();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public ActionResult GetRecipientListQuantity()
        {
            var data = _campaignDashboardService.GetRecipientListQuantity();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }
    }
}