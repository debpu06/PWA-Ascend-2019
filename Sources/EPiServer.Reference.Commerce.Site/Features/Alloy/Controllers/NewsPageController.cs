using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers
{
    public class NewsPageController : PageControllerBase<NewsPage>
    {
        public ActionResult Index(NewsPage currentPage)
        {
            var editHints = ViewData.GetEditHints<ContentViewModel<NewsPage>, NewsPage>();
            editHints.AddFullRefreshFor(m => m.PageIsNavigationRoot);

            return View(ContentViewModel.Create(currentPage));
        }
    }
}