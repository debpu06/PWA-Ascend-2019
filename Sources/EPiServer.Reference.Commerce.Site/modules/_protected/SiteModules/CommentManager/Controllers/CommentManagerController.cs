using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommentManager.Services;
using EPiServer.Shell.Navigation;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommentManager.Controllers
{
    public class CommentManagerController : Controller
    {
        private readonly ICommentManagerService _commentManagerService;

        public CommentManagerController(ICommentManagerService commentManagerService)
        {
            _commentManagerService = commentManagerService;
        }

        [MenuItem("/global/commerce/commentsmanager", TextResourceKey = "/Shared/CommentsManager")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Get(int page, int limit)
        {
            long total = 0;
            var comments = _commentManagerService.Get(page, limit, out total);
            var result = new
            {
                Comments = comments,
                NumberOfItems = (page - 1) * limit + comments.Count(),
                Total = total
            };
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(result),
                ContentType = "application/json",
            };
        }

        [HttpPost]
        public ActionResult Approve(string id)
        {
            var comment = _commentManagerService.Approve(id);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(comment),
                ContentType = "application/json",
            };
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            _commentManagerService.Delete(id);

            return new ContentResult
            {
                Content = "Delete successfully.",
            };
        }

    }
}
