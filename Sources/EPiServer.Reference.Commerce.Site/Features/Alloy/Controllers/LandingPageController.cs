using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    public class LandingPageController : PageControllerBase<LandingPage>
    {
        public ActionResult Index(LandingPage currentPage)
        {
            var editHints = ViewData.GetEditHints<ContentViewModel<LandingPage>, LandingPage>();
            editHints.AddFullRefreshFor(m => m.PageIsNavigationRoot);

            return View(ContentViewModel.Create(currentPage));
        }
    }
}