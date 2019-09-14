using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    public class ProductAnalyticsController : Controller
    {
        private readonly IContentLoader _contentLoader;

        public ProductAnalyticsController(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public ActionResult Index(string id)
        {
            ContentReference contentLink;
            var url = "https://app.powerbi.com/view?r=eyJrIjoiMzhkNjk0MzUtOWI1NC00ZGVjLWE2MDctYTFkNjg0MzA3M2JmIiwidCI6IjNlYzAwZDc5LTAyMWEtNDJkNC1hYWM4LWRjYjM1OTczZGZmMiIsImMiOjh9";
            if (!ContentReference.TryParse(id, out contentLink))
            {
                return View("index", null, url);
            }

            NodeContent node;
            if (_contentLoader.TryGet(contentLink, out node))
            {
                return View("Index", null, "https://app.powerbi.com/view?r=eyJrIjoiMWQ0NzI0NjItMTIzOC00ZTJmLTkyMTctNTRjYjZjYTIwY2M5IiwidCI6IjNlYzAwZDc5LTAyMWEtNDJkNC1hYWM4LWRjYjM1OTczZGZmMiIsImMiOjh9");
            }

            EntryContentBase entry;
            if (!_contentLoader.TryGet(contentLink, out entry))
            {
                return View("index", null, url);
            }

            if (entry.Code.Equals("Puma-Black-Suede-Athletic-Sneakers_1"))
            {
                return View("Index", null, "https://app.powerbi.com/view?r=eyJrIjoiYTI1OWNmYzctNzM5OC00OGZlLWI5ZDYtOGI1NWUyNDkyZGE4IiwidCI6IjNlYzAwZDc5LTAyMWEtNDJkNC1hYWM4LWRjYjM1OTczZGZmMiIsImMiOjh9");
            }

            if (entry.Code.Equals("gray-jacket2"))
            {
                return View("Index", null, "https://app.powerbi.com/view?r=eyJrIjoiMmQyNmNhNTItNDg1Ny00YmE0LWJhMGUtNzUzYTIxNGNiZjE3IiwidCI6IjNlYzAwZDc5LTAyMWEtNDJkNC1hYWM4LWRjYjM1OTczZGZmMiIsImMiOjh9");
            }

            return View("Index", null, entry.Code.Equals("Beefy-T-Short-Sleeve-Tee_2") ? "https://app.powerbi.com/view?r=eyJrIjoiNDMyODYyNDktMDAxZS00YjkwLWJmNGMtZGZhNWVkYzYyZTVkIiwidCI6IjNlYzAwZDc5LTAyMWEtNDJkNC1hYWM4LWRjYjM1OTczZGZmMiIsImMiOjh9" : url);
        }
    }
}