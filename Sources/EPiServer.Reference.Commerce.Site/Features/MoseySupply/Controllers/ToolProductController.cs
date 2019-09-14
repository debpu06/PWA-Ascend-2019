using System.Threading.Tasks;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Catalog;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models;
using EPiServer.Tracking.Commerce.Data;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    public class ToolProductController : BaseProductController<ToolProduct>
    {
        private readonly bool _isInEditMode;
        private readonly CatalogEntryViewModelFactory _viewModelFactory;
        private readonly IReviewService _reviewService;
        private readonly IRecommendationService _recommendationService;
        private readonly ReferenceConverter _referenceConverter;

        public ToolProductController(IsInEditModeAccessor isInEditModeAccessor,
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
        public async Task<ActionResult> Index(ToolProduct currentContent,
            string variationCode = "",
            bool useQuickview = false,
            bool skipTracking = false)
        {
            var viewModel = _viewModelFactory.Create<ToolProduct, ToolVariant, ToolProductViewModel>(currentContent, variationCode);

            if (_isInEditMode && viewModel.Variant == null)
            {
                var emptyViewName = "ProductWithoutEntries";
                return Request.IsAjaxRequest() ?
                    PartialView(emptyViewName, viewModel) :
                    (ActionResult)View(emptyViewName, viewModel);
            }

            if (viewModel.Variant == null)
            {
                return HttpNotFound();
            }

            if (useQuickview)
            {
                await _recommendationService.TrackProduct(HttpContext,
                    currentContent.Code,
                    true);
                return PartialView("_Quickview", viewModel);
            }

            var trackingResponse = new TrackingResponseData();
            if (!skipTracking)
            {
                trackingResponse = await _recommendationService.TrackProduct(HttpContext,
                    currentContent.Code,
                    false);
            }

            viewModel.Reviews = GetReviews(currentContent.Code);
            viewModel.AlternativeProducts = trackingResponse.GetAlternativeProductsRecommendations(_referenceConverter);
            viewModel.CrossSellProducts = trackingResponse.GetCrossSellProductsRecommendations(_referenceConverter);
            currentContent.AddBrowseHistory();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SelectVariant(ToolProduct currentContent,
            string color,
            string size,
            bool useQuickview = false)
        {
            var variant = _viewModelFactory.SelectVariant(currentContent, color, size);
            if (variant != null)
            {
                return RedirectToAction("Index",
                    new { variationCode = variant.Code, useQuickview, skipTracking = true });
            }

            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult AddAReview(ReviewSubmissionViewModel reviewForm)
        {
            // Invoke the ReviewService to add the submission
            _reviewService.Add(reviewForm);
            AddActivity(reviewForm.ProductCode, reviewForm.Rating, reviewForm.Nickname);

            return RedirectToAction("Index");
        }
    }
}