using System.Linq;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Bookmark.Pages;
using EPiServer.Reference.Commerce.Site.Features.Bookmark.Services;
using EPiServer.Reference.Commerce.Site.Features.Bookmark.ViewModels;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Customers;

namespace EPiServer.Reference.Commerce.Site.Features.Bookmark.Controllers
{
    /// <summary>
    /// A page to list all gift card belonging to a customer
    /// </summary>
    public class BookmarksPageController : PageController<BookmarksPage>
    {
        private readonly IBookmarksService _bookmarksService;

        public BookmarksPageController(IBookmarksService bookmarksService)
        {
            _bookmarksService = bookmarksService;
        }

        public ActionResult Index(BookmarksPage currentPage)
        {
            var model = new BookmarksViewModel()
            {
                CurrentContent = currentPage,
                Bookmarks = _bookmarksService.Get()
            };

            return View(model);
        }
    }
}