using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Social.Pages;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Controllers
{
    /// <summary>
    /// The CommunityPageController handles the rendering of social community pages and the corresponding blocks embedded on those pages
    /// </summary>
    public class CommunityPageController : PageController<CommunityPage>
    {
        /// <summary>
        /// Renders the social community page view.
        /// </summary>
        /// <param name="currentPage">The current page</param>
        public ActionResult Index(CommunityPage currentPage)
        {
            var pageViewModel = new ContentViewModel<CommunityPage>(currentPage);
            return View("~/Features/Social/Views/CommunityPage/Index.cshtml", pageViewModel);
        }
    }
}