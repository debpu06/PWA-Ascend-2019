using EPiServer.Reference.Commerce.Site.Features.Editorial.Models;
using EPiServer.Reference.Commerce.Site.Features.Editorial.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Web.Mvc;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Editorial.Controllers
{
    public class StandardPageController : PageController<StandardPage>
    {
        private readonly IContentLoader _contentLoader;

        public StandardPageController(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public ActionResult Index(StandardPage currentPage)
        {
            var alloy = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage) as AlloyStartPage;
            if (alloy == null)
            {
                return View(new StandardPageViewModel(currentPage));
            }

            var editHints = ViewData.GetEditHints<ContentViewModel<StandardPage>, StandardPage>();
            editHints.AddFullRefreshFor(m => m.PageIsNavigationRoot);

            return View(new StandardPageViewModel(currentPage)
            {
                IsAlloy = true
            });
        }
    }
}