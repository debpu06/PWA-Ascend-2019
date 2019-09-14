using EPiServer.Shell.Navigation;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Controllers
{
    public class CommerceDashboardController : Controller
    {
        private readonly ICommerceDashboardService _commerceDashboardService;

        public CommerceDashboardController(ICommerceDashboardService commerceDashboardService)
        {
            _commerceDashboardService = commerceDashboardService;
        }

        [MenuItem("/global/commerce/commercedashboard", TextResourceKey = "/Shared/CommerceDashboard")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetOrders()
        {
            var data = _commerceDashboardService.GetOrders(0, 10000);
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
        }

        public ActionResult GetCarts()
        {
            var data = _commerceDashboardService.GetCarts(0, 10000).Where(x => !x.Name.Contains("Wishlist"));
            return new ContentResult
            {
                Content = "{ \"result\" : " + data.Count() + "}",
                ContentType = "application/json"
            };
        }

        public ActionResult GetContacts()
        {
            var data = _commerceDashboardService.GetContacts(0, 10000);
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
        }

        public ActionResult GetRevenues()
        {
            var data = _commerceDashboardService.GetRevenues();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
        }

        public ActionResult GetProductsSold()
        {
            var data = _commerceDashboardService.GetProductsSold();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
        }

        public ActionResult GetNewCustomers()
        {
            var data = _commerceDashboardService.GetNewCustomers();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
        }

        public ActionResult GetTopProducts()
        {
            var data = _commerceDashboardService.GetTopProducts();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
        }

        public ActionResult GetTopPromotions()
        {
            var data = _commerceDashboardService.GetTopPromotions();
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
        }
    }
}
