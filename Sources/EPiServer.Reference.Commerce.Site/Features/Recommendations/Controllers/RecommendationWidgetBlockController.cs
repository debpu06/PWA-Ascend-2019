using EPiServer.Editor;
using EPiServer.Framework.Web.Resources;
using EPiServer.Personalization.Commerce.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Tracking.Commerce.Data;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Catalog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;

namespace EPiServer.Reference.Commerce.Site.Features.Recommendations.Controllers
{
    public class RecommendationWidgetBlockController : BlockController<RecommendationWidgetBlock>
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ReferenceConverter _referenceConverter;
        private readonly IContentRouteHelper _contentRouteHelper;
        private readonly IRequiredClientResourceList _requiredClientResource;
        private readonly ICartService _cartService;

        public RecommendationWidgetBlockController(IRecommendationService recommendationService,
            ReferenceConverter referenceConverter,
            IContentRouteHelper contentRouteHelper, IRequiredClientResourceList requiredClientResource, ICartService cartService)
        {
            _recommendationService = recommendationService;
            _referenceConverter = referenceConverter;
            _contentRouteHelper = contentRouteHelper;
            _requiredClientResource = requiredClientResource;
            _cartService = cartService;
        }

        public override ActionResult Index(RecommendationWidgetBlock currentContent)
        {
            _requiredClientResource.RequireScriptInline($"Recommendations.getRecommendations('{currentContent.WidgetType}',{currentContent.NumberOfRecommendations},'{currentContent.Name}','{currentContent.Value}')").AtFooter();
            return PartialView(new BlockViewModel<RecommendationWidgetBlock>(currentContent));
        }

        public async Task<ActionResult> GetRecommendations(string widgetType, string name, string value, int numberOfRecs = 4)
        {
            if (string.IsNullOrEmpty(widgetType) || PageEditing.PageIsInEditMode)
            {
                return new EmptyResult();
            }

            var parentContent = _contentRouteHelper.Content;
            List<Recommendation> recommendations = null;
            TrackingResponseData response = null;

            switch (widgetType)
            {
                case "Home":
                    response = await _recommendationService.TrackHome(ControllerContext.HttpContext);
                    recommendations = response.GetRecommendations(_referenceConverter, RecommendationsExtensions.Home)
                        .ToList();
                    break;
                case "Basket":
                    response = await _recommendationService.TrackCart(ControllerContext.HttpContext, _cartService.LoadCart(_cartService.DefaultCartName, false).Cart);
                    recommendations = response.GetRecommendations(_referenceConverter, RecommendationsExtensions.Basket)
                        .ToList();
                    break;
                case "Checkout":
                    response = await _recommendationService.TrackCheckout(ControllerContext.HttpContext);
                    recommendations = response.GetRecommendations(_referenceConverter, "Checkout")
                        .ToList();
                    break;
                case "Wishlist":
                    response = await _recommendationService.TrackWishlist(ControllerContext.HttpContext);
                    recommendations = response.GetRecommendations(_referenceConverter, "Wishlist")
                        .ToList();
                    break;
                default:
                    response = await _recommendationService.TrackAttribute(ControllerContext.HttpContext, name, value);
                    recommendations = response.GetRecommendations(_referenceConverter, "attributeWidget")
                        .ToList();
                    break;
            }

            if (recommendations == null)
            {
                return new EmptyResult();
            }

            recommendations = recommendations.Take(numberOfRecs).ToList();
            return PartialView("Recommendations", _recommendationService.GetRecommendedProductTileViewModels(recommendations));
        }
    }
}
