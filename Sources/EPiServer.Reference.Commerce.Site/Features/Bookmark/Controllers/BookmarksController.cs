using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Bookmark.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly IBookmarksService _bookmarksService;

        public BookmarksController(IBookmarksService bookmarksService)
        {
            _bookmarksService = bookmarksService;
        }

        [HttpPost]
        public ActionResult Bookmark(int contentId)
        {
            _bookmarksService.Add(contentId);
            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult Remove(Guid contentGuid)
        {
            _bookmarksService.Remove(contentGuid);
            return Json(new { Success = true });
        }
    }
}