using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Blocks;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Models;
using EPiServer.Reference.Commerce.Site.Features.Destinations.Pages;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Media.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Destinations.Controllers
{
    public class TagPageController : PageController<TagPage>
    {
        private readonly IContentLoader _contentLoader;

        public TagPageController(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public ActionResult Index(TagPage currentPage)
        {
            var model = new TagsViewModel(currentPage)
                {
                    Continent = ControllerContext.RequestContext.GetCustomRouteData<string>("Continent")
                };

            var addcat = ControllerContext.RequestContext.GetCustomRouteData<string>("Category");
            if (addcat != null) 
                model.AdditionalCategories = addcat.Split(',');

            var carousel = new CarouselViewModel
                {
                    Items = new List<CarouselItemBlock>()
                };
            
            
            if (currentPage.Images != null)
            {
                foreach (var img in currentPage.Images.FilteredItems.Select(ci => ci.ContentLink))
                {
                    var t = _contentLoader.Get<ImageMediaData>(img).Title;
                    carousel.Items.Add(new CarouselItemBlock { Image = img, Title = t});
                }
            }
            var q = SearchClient.Instance.Search<DestinationPage>()
                .Filter(f => f.TagString().Match(currentPage.Name));

            if (model.AdditionalCategories != null)
            {
                q = model.AdditionalCategories.Aggregate(q, (current, c) => current.Filter(f => f.TagString().Match(c)));
            }
            if (model.Continent != null)
            {
                q = q.Filter(dp => dp.Continent.MatchCaseInsensitive(model.Continent));
            }
            var res = q.StaticallyCacheFor(new System.TimeSpan(0, 1, 0)).GetContentResult();

            model.Destinations = res.ToList();

            //Add theme images from results
            foreach (var d in model.Destinations)
            {
                carousel.Items.Add(new CarouselItemBlock
                {
                    Image = d.Image,
                    Title = d.Name,
                });
            }

            model.Carousel = carousel;
            return View(model);
        }
    }
}