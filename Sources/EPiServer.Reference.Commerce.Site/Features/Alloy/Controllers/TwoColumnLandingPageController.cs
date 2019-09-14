using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    public class TwoColumnLandingPageController : PageControllerBase<TwoColumnLandingPage>
    {
        public ActionResult Index(TwoColumnLandingPage currentPage)
        {
            var editHints = ViewData.GetEditHints<ContentViewModel<TwoColumnLandingPage>, TwoColumnLandingPage>();
            editHints.AddFullRefreshFor(m => m.PageIsNavigationRoot);

            return View(ContentViewModel.Create(currentPage));
        }
    }
}