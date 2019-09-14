using System.Web.Mvc;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Personalization;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Models;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Web.Mvc;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.IdentityTracker;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Controllers
{
    public class DestinationsPageController : PageController<DestinationsPage>
    {
        // c is selected continents
        // a is selected activities
        // d is selected distances
        // t is selected temperature

        private readonly IContentLoader _contentLoader;

        public DestinationsPageController(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public ActionResult Index(DestinationsPage currentPage)
        {
            var query = SearchClient.Instance.Search<DestinationPage>();
            
            if (currentPage.FilterArea != null)
            {
                foreach (var filterBlock in currentPage.FilterArea.FilteredItems)
                {
                    var b = _contentLoader.Get<BlockData>(filterBlock.ContentLink) as IFilterBlock;
                    if (b != null)
                    {
                        query = b.AddFilter(query);
                    }
                }

                query = query.PublishedInCurrentLanguage().FilterOnReadAccess();

                foreach (var filterBlock in currentPage.FilterArea.FilteredItems)
                {
                    var b = _contentLoader.Get<BlockData>(filterBlock.ContentLink) as IFilterBlock;
                    if (b != null)
                    {
                        query = b.ApplyFilter(query, Request.QueryString);
                    }
                }
            }

            var destinations = query.OrderBy(x => x.PageName)
                                    .Take(500)
                                    .StaticallyCacheFor(new System.TimeSpan(0, 1, 0)).GetContentResult();

            var model = new DestinationsViewModel(currentPage)
                {
                    Destinations = destinations,
                    MapCenter = GetMapCenter(),
                    UserLocation = GeoPosition.GetUsersLocation(),
                    QueryString = Request.QueryString
                };
            return View(model);
        }

        private static GeoCoordinate GetMapCenter()
        {
            var userLocation = GeoPosition.GetUsersPosition();
            if (userLocation != null)
            {
                return new GeoCoordinate(30, userLocation.Longitude);
            }
            return new GeoCoordinate(30, 0);
        }
    }
}