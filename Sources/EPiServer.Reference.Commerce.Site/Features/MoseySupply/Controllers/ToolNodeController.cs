using System.Threading.Tasks;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Catalog;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;

namespace EPiServer.Reference.Commerce.Site.Features.MoseySupply.Controllers
{
    public class ToolNodeController : ContentController<ToolNode>
    {
        private readonly SearchViewModelFactory _viewModelFactory;
        private readonly IRecommendationService _recommendationService;
        private readonly ReferenceConverter _referenceConverter;

        public ToolNodeController(
            SearchViewModelFactory viewModelFactory,
            IRecommendationService recommendationService,
            ReferenceConverter referenceConverter)
        {
            _viewModelFactory = viewModelFactory;
            _recommendationService = recommendationService;
            _referenceConverter = referenceConverter;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public async Task<ViewResult> Index(ToolNode currentContent, FilterOptionViewModel viewModel)
        {
            var model = _viewModelFactory.Create(currentContent, viewModel, HttpContext.Request.QueryString["facets"]);

            if (HttpContext.Request.HttpMethod == "GET")
            {
                var response = await _recommendationService.TrackCategory(HttpContext, currentContent);
                model.Recommendations = response.GetCategoryRecommendations(_referenceConverter);
            }

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult Facet(ToolNode currentContent, FilterOptionViewModel viewModel)
        {
            return PartialView("_Facet", viewModel);
        }

        [ChildActionOnly]
        public ActionResult RecentlyBrowsed(ToolNode currentContent)
        {
            return PartialView("_RecentlyBrowsed", ContentExtensions.GetBrowseHistory());
        }
    }
}