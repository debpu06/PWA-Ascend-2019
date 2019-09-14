using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Personalization.Commerce.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web.Mvc;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Tracking.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Product.Controllers
{
    public class BundleController : ContentController<BundleContent>
    {
        private readonly bool _isInEditMode;
        private readonly CatalogEntryViewModelFactory _viewModelFactory;

        public BundleController(IsInEditModeAccessor isInEditModeAccessor, CatalogEntryViewModelFactory viewModelFactory)
        {
            _isInEditMode = isInEditModeAccessor();
            _viewModelFactory = viewModelFactory;
        }

        [HttpGet]
        [CommerceTracking(TrackingType.Product)]
        public ActionResult Index(FashionBundle currentContent, bool useQuickview = false)
        {
            var viewModel = _viewModelFactory.CreateBundle<FashionBundle, FashionVariant, FashionBundleViewModel>(currentContent);

            if (_isInEditMode && !viewModel.Entries.Any())
            {
                var emptyViewName = "BundleWithoutEntries";
                return Request.IsAjaxRequest() ? PartialView(emptyViewName, viewModel) : (ActionResult)View(emptyViewName, viewModel);
            }

            if (useQuickview)
            {
                return PartialView("_Quickview", viewModel);
            }

            viewModel.AlternativeProducts = this.GetAlternativeProductsRecommendations();
            viewModel.CrossSellProducts = this.GetCrossSellProductsRecommendations();
            currentContent.AddBrowseHistory();
            return Request.IsAjaxRequest() ? PartialView(viewModel) : (ActionResult)View(viewModel);
        }
    }
}