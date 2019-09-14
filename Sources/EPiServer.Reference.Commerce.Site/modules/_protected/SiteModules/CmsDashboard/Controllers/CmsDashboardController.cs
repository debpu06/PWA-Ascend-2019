using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Services;
using EPiServer.Shell.Navigation;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Controllers
{
    public class CmsDashboardController : Controller
    {
        private readonly ICmsDashboardService _cmsDashboardService;

        public CmsDashboardController(ICmsDashboardService cmsDashboardService)
        {
            _cmsDashboardService = cmsDashboardService;
        }

        [MenuItem("/global/cms/cmsdashboard", TextResourceKey = "/Shared/CmsDashboard")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetAmountOfFormsSubmitted()
        {
            var data = await _cmsDashboardService.GetAmountOfFormsSubmitted();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetSiteAudits()
        {
            var data = await _cmsDashboardService.GetSiteAudits();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetPageTypeAudits()
        {
            var data = await _cmsDashboardService.GetPageTypeAudits();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetBlockTypeAudits()
        {
            var data = await _cmsDashboardService.GetBlockTypeAudits();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetVisitorGroupAudit()
        {
            var data = await _cmsDashboardService.GetVisitorGroupAudit();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetBounceRates()
        {
            var data = await _cmsDashboardService.GetBounceRates();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetTopLandingPages()
        {
            var data = await _cmsDashboardService.GetTopLandingPages();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetTopLocations()
        {
            var data = await _cmsDashboardService.GetTopLocations();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetVisitors()
        {
            var data = await _cmsDashboardService.GetVisitors();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetAbTestContents()
        {
            var data = await _cmsDashboardService.GetAbTestContents();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    // Apparently there is some loop referenceing problem with the 
                    // KeyPerformace indicators, this gets rid of that issue so we can actually convert the 
                    // data to json
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetAbTestingMostParticipationPercentage()
        {
            var data = await _cmsDashboardService.GetAbTestingMostParticipationPercentage();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetAbTestingMostPageViews()
        {
            var data = await _cmsDashboardService.GetAbTestingMostPageViews();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }),
                ContentType = "application/json",
            };
        }

        [HttpGet]
        public async Task<ActionResult> GetAbTestingMostConversions()
        {
            var data = await _cmsDashboardService.GetAbTestingMostConversions();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }),
                ContentType = "application/json",
            };
        }
    }
}
