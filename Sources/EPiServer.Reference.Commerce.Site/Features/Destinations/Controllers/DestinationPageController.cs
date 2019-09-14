using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Models;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Controllers
{
    public class DestinationPageController : PageController<DestinationPage>
    {
        private IContentRepository _contentRepository;
        public DestinationPageController(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public ActionResult Index(DestinationPage currentPage)
        {
            var model = new DestinationViewModel(currentPage);

            if (!ContentReference.IsNullOrEmpty(currentPage.Image))
            {
                model.Image = _contentRepository.Get<ImageData>(currentPage.Image);
            }

            model.DestinationNavigation.ContinentDestinations = SearchClient.Instance
                .Search<DestinationPage>()
                .Filter(x => x.Continent.Match(currentPage.Continent))
                .OrderBy(x => x.PageName)
                .FilterForVisitor()
                .Take(100)
                .StaticallyCacheFor(new System.TimeSpan(0,10,0))
                .GetContentResult();

            model.DestinationNavigation.CloseBy = SearchClient.Instance
                .Search<DestinationPage>()
                .Filter(x => x.Continent.Match(currentPage.Continent)
                             & !x.PageLink.Match(currentPage.PageLink))
                .FilterForVisitor()
                .OrderBy(x => x.Coordinates)
                .DistanceFrom(currentPage.Coordinates)
                .Take(5)
                .StaticallyCacheFor(new System.TimeSpan(0, 10, 0))
                .GetContentResult();

            //model.SimilarDestinations = GetRelatedDestinations(currentPage);

            var editingHints = ViewData.GetEditHints<DestinationViewModel, DestinationPage>();
            editingHints.AddFullRefreshFor(p => p.Image);
            editingHints.AddFullRefreshFor(p => p.Tags);

            return View(model);
        }

        private IEnumerable<DestinationPage> GetRelatedDestinations(DestinationPage currentPage)
        {
            IQueriedSearch<DestinationPage> query = SearchClient.Instance
                .Search<DestinationPage>()
                .MoreLike(SearchTextFly(currentPage))
                .BoostMatching(x =>
                               x.Country.Match(currentPage.Country ?? ""), 2)
                .BoostMatching(x =>
                        x.Continent.Match(currentPage.Continent ?? ""), 1.5)
                .BoostMatching(x =>
                               x.Coordinates
                                   .WithinDistanceFrom(currentPage.Coordinates ?? new GeoLocation(0, 0),
                                                       1000.Kilometers()), 2.5);

            query = currentPage.Category.Aggregate(query,
                                                   (current, category) =>
                                                   current.BoostMatching(x => x.InCategory(category), 1.5));

            return query
                .Filter(x => !x.PageLink.Match(currentPage.PageLink))
                .FilterForVisitor()
                .Take(3)
                .GetPagesResult();
        }

        public virtual string SearchTextFly(DestinationPage currentPage)
        {
            var searchText = "";
            
            return searchText;
        }
    }
}