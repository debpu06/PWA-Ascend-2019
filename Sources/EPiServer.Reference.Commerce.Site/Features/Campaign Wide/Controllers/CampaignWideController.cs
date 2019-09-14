using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.CampaignWide.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignWide.Controllers
{
    public class CampaignWideController : PageController<CampaignWidePage>
    {
        [HttpGet]
        public ActionResult Index(CampaignWidePage currentPage)
        {
            return View(new ContentViewModel<CampaignWidePage>(currentPage));
        }
    }
}