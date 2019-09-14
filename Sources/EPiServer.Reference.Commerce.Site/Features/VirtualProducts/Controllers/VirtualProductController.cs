using EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Social.Services;
using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Tracking.Commerce.Data;
using Mediachase.Commerce.Catalog;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Product.Controllers;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.VirtualProducts.ViewModels;
using System.Net;
using EPiServer.Web.Routing;
using EPiServer.Web;
using System;

namespace EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Controllers
{
    public class VirtualProductController : BaseProductController<VirtualProduct>
    {
        private readonly bool _isInEditMode;
        private readonly CatalogEntryViewModelFactory _viewModelFactory;
        private readonly IReviewService _reviewService;
        private readonly IRecommendationService _recommendationService;
        private readonly ReferenceConverter _referenceConverter;
        private readonly UrlResolver _urlResolver;

        public VirtualProductController(IsInEditModeAccessor isInEditModeAccessor,
            CatalogEntryViewModelFactory viewModelFactory,
            IReviewService reviewService,
            IRecommendationService recommendationService,
            ReferenceConverter referenceConverter,
            IReviewActivityService reviewActivityService,
            UrlResolver urlResolver) : base(reviewActivityService, reviewService)
        {
            _isInEditMode = isInEditModeAccessor();
            _viewModelFactory = viewModelFactory;
            _reviewService = reviewService;
            _recommendationService = recommendationService;
            _referenceConverter = referenceConverter;
            _urlResolver = urlResolver;
        }

        [HttpGet]
        public async Task<ActionResult> Index(VirtualProduct currentContent, string variationCode = "", bool useQuickview = false, bool skipTracking = false)
        {
            var viewModel = _viewModelFactory.Create<VirtualProduct, VirtualVariant, VirtualProductViewModel>(currentContent, variationCode);

            if (_isInEditMode && viewModel.Variant == null)
            {
                var emptyViewName = "ProductWithoutEntries";
                return Request.IsAjaxRequest() ? PartialView(emptyViewName, viewModel) : (ActionResult)View(emptyViewName, viewModel);
            }

            if (viewModel.Variant == null)
            {
                return HttpNotFound();
            }

            if (viewModel.Variant is FileVariant)
            {
                viewModel.FileUrlIsWorking = ValadationLink(_urlResolver.GetUrl(((FileVariant)viewModel.Variant).File,
                                                                            null,
                                                                            new VirtualPathArguments
                                                                            {
                                                                                ContextMode = ContextMode.Default,
                                                                                ForceCanonical = true
                                                                            }));
                //if (!viewModel.FileUrlIsWorking) viewModel.IsAvailable = false;
            }

            if (viewModel.Variant is ElevatedRoleVariant)
            { 
                if(!User.Identity.IsAuthenticated) viewModel.IsAvailable = false;
            }

            if (useQuickview)
            {
                await _recommendationService.TrackProduct(HttpContext, currentContent.Code, true);
                return PartialView("_Quickview", viewModel);
            }

            var trackingResponse = new TrackingResponseData();
            if (!skipTracking)
            {
                trackingResponse = await _recommendationService.TrackProduct(HttpContext, currentContent.Code, false);
            }

            viewModel.Reviews = GetReviews(currentContent.Code);
            viewModel.AlternativeProducts = trackingResponse.GetAlternativeProductsRecommendations(_referenceConverter);
            viewModel.CrossSellProducts = trackingResponse.GetCrossSellProductsRecommendations(_referenceConverter);
            currentContent.AddBrowseHistory();


            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SelectVariant(VirtualProduct currentContent, string color, string size, bool useQuickview = false)
        {
            var variant = _viewModelFactory.SelectVariant(currentContent, color, size);
            if (variant != null)
            {
                return RedirectToAction("Index", new { variationCode = variant.Code, useQuickview, skipTracking = true });
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

        private bool ValadationLink(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.AllowAutoRedirect = true;
            request.Method = "HEAD";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch(Exception ex)
            {
                var mess = ex.Message;
                return false;
            }
        }
    }
}