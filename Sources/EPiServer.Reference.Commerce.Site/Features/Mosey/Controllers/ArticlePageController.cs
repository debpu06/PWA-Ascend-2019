using EPiServer.DataAbstraction;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Controllers;
using EPiServer.Reference.Commerce.Site.Features.Mosey.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Mosey.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Mosey.Controllers
{
    public class ArticlePageController : PageControllerBase<ArticlePage>
    {
        private readonly CategoryRepository _categoryRepository;

        public ArticlePageController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public ActionResult Index(ArticlePage currentPage)
        {
            var editHints = ViewData.GetEditHints<ContentViewModel<ArticlePage>, ArticlePage>();
            editHints.AddFullRefreshFor(m => m.PageIsNavigationRoot);

            return View("~/Features/Mosey/Views/ArticlePage/Index.cshtml", new ArticlePageViewModel(currentPage, _categoryRepository));
        }
    }
}