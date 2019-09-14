using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using Mediachase.Commerce.Catalog;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    public class DefaultVariationController : BaseProductController<VariantBase>
    {
        private readonly bool _isInEditMode;
        private readonly CatalogEntryViewModelFactory _viewModelFactory;
        private readonly IReviewService _reviewService;
        private readonly IRecommendationService _recommendationService;
        private readonly ReferenceConverter _referenceConverter;

        public DefaultVariationController(IsInEditModeAccessor isInEditModeAccessor,
            CatalogEntryViewModelFactory viewModelFactory,
            IReviewService reviewService,
            IRecommendationService recommendationService,
            ReferenceConverter referenceConverter,
            IReviewActivityService reviewActivityService) : base(reviewActivityService, reviewService)
        {
            _isInEditMode = isInEditModeAccessor();
            _viewModelFactory = viewModelFactory;
            _reviewService = reviewService;
            _recommendationService = recommendationService;
            _referenceConverter = referenceConverter;
        }

        [HttpGet]
        public async Task<ActionResult> Index(VariantBase currentContent)
        {
            var trackingResponse = await _recommendationService.TrackProduct(HttpContext, currentContent.Code, false);
            var viewModel = _viewModelFactory.CreateVariant<VariantBase, VariantBaseViewModel>(currentContent);
            viewModel.Reviews = GetReviews(currentContent.Code);
            viewModel.AlternativeProducts = trackingResponse.GetAlternativeProductsRecommendations(_referenceConverter);
            viewModel.CrossSellProducts = trackingResponse.GetCrossSellProductsRecommendations(_referenceConverter);
            currentContent.AddBrowseHistory();
            return Request.IsAjaxRequest() ? PartialView(viewModel) : (ActionResult)View(viewModel);
        }
    }
}