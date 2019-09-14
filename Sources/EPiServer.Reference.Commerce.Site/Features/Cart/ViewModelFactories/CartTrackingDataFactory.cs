using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Commerce.Order;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.ServiceLocation;
using EPiServer.Tracking.Commerce;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.ViewModelFactories
{

    /// <summary>
    /// The service to create tracking data for cart
    /// </summary>

    public class CartTrackingDataFactory : TrackingDataFactory
    {

        private readonly ICurrentMarket _currentMarket;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;

        public CartTrackingDataFactory(ILineItemCalculator lineItemCalculator, 
            IContentLoader contentLoader,
            IOrderGroupCalculator orderGroupCalculator, 
            LanguageResolver languageResolver, 
            IOrderRepository orderRepository,
            ReferenceConverter referenceConverter, 
            IRelationRepository relationRepository, 
            IRecommendationContext recommendationContext,
            ICurrentMarket currentMarket,
            IRequestTrackingDataService requestTrackingDataService,
            ICartService cartService) 
            : base(lineItemCalculator, contentLoader, orderGroupCalculator, languageResolver, orderRepository, referenceConverter, relationRepository, recommendationContext, currentMarket, requestTrackingDataService)
        {
            _currentMarket = currentMarket;
            _orderRepository = orderRepository;
            _cartService = cartService;
        }

        protected override IOrderGroup GetCurrentCart()
        {
            return _orderRepository.LoadCart<ICart>(GetContactId(), _cartService.DefaultCartName, _currentMarket);
        }

        protected override IOrderGroup GetCurrentWishlist()
        {
            return _orderRepository.LoadCart<ICart>(GetContactId(), _cartService.DefaultWishListName, _currentMarket);
        }
    }

}