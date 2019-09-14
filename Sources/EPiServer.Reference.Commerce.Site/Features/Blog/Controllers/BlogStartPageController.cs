using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Blog.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Blog.Controllers
{
    public class BlogStartPageController : PageController<BlogStartPage>
    {

        public ActionResult Index(BlogStartPage currentPage)
        {
            var model = new ContentViewModel<BlogStartPage>(currentPage);
            return View(model);
        }


    }
}
