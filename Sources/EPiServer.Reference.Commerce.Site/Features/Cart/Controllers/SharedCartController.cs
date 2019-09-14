using EPiServer;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Reference.Commerce.B2B;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Tracking.Commerce;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Controllers


{
    [Authorize]
    public class SharedCartController : PageController<SharedCartPage>
    {
        private readonly IContentLoader _contentLoader;
        private readonly ICartService _cartService;
        private CartWithValidationIssues _SharedCart;
        private readonly IOrderRepository _orderRepository;
        private readonly IRecommendationService _recommendationService;
        readonly CartViewModelFactory _cartViewModelFactory;
        private readonly IQuickOrderService _quickOrderService;
        private readonly ReferenceConverter _referenceConverter;
        private readonly ICustomerService _customerService;
        private readonly ICartServiceB2B _cartServiceB2B;

        public SharedCartController(
            IContentLoader contentLoader,
            ICartService cartService,
            IOrderRepository orderRepository,
            IRecommendationService recommendationService,
            CartViewModelFactory cartViewModelFactory,
            IQuickOrderService quickOrderService,
            ReferenceConverter referenceConverter,
            ICustomerService customerService,
            ICartServiceB2B cartServiceB2B)
        {
            _contentLoader = contentLoader;
            _cartService = cartService;
            _orderRepository = orderRepository;
            _recommendationService = recommendationService;
            _cartViewModelFactory = cartViewModelFactory;
            _quickOrderService = quickOrderService;
            _referenceConverter = referenceConverter;
            _customerService = customerService;
            _cartServiceB2B = cartServiceB2B;
        }

        [HttpGet]
        [CommerceTracking(TrackingType.Other)]
        public ActionResult Index(SharedCartPage currentPage)
        {
            var viewModel = _cartViewModelFactory.CreateSharedCartViewModel(SharedCart.Cart, currentPage);
            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult SharedCartMiniCartDetails()
        {
            var viewModel = _cartViewModelFactory.CreateMiniCartViewModel(SharedCart.Cart, true);
            var refStartPage = SiteDefinition.Current.StartPage;
            var startPage = _contentLoader.Get<BaseStartPage>(refStartPage);
            return PartialView("~/Views/Shared/Header/_HeaderCart.cshtml", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult AddToCart(string code, int qty = 1)
        {
            ModelState.Clear();

            if (SharedCart.Cart == null)
            {
                _SharedCart = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultSharedCardName, OrganizationId),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            var result = _cartService.AddToCart(SharedCart.Cart, code, qty, "delivery", "");
            if (result.EntriesAddedToCart)
            {
                _orderRepository.Save(SharedCart.Cart);
                if (Request.IsAjaxRequest())
                {
                    return SharedCartMiniCartDetails();
                }
                return RedirectToAction("Index");
            }

            return new HttpStatusCodeResult(500, result.GetComposedValidationMessage());

        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult ChangeCartItem(string code, decimal quantity, string size, string newSize)
        {
            ModelState.Clear();

            _cartService.ChangeCartItem(SharedCart.Cart, 0, code, quantity, size, newSize);
            _orderRepository.Save(SharedCart.Cart);
            if (Request.IsAjaxRequest())
            {
                return SharedCartMiniCartDetails();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult DeleteSharedCart()
        {
            if (SharedCart.Cart != null)
            {
                _orderRepository.Delete(SharedCart.Cart.OrderLink);
            }
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);

            return RedirectToAction("Index", new { Node = startPage.SharedCartPage });
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult RemoveCartItem(string code, string userId)
        {
            ModelState.Clear();
            var userWishCart = _cartService.LoadSharedCardByCustomerId(new Guid(userId));
            if (userWishCart.GetAllLineItems().Count() == 1)
            {
                _orderRepository.Delete(userWishCart.OrderLink);
            }
            else
            {
                _cartService.ChangeQuantity(userWishCart, 0, code, 0);
                _orderRepository.Save(userWishCart);
            }

            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            var urlResolver = ServiceLocation.ServiceLocator.Current.GetInstance<Web.Routing.UrlResolver>();
            var pageUrl = urlResolver.GetUrl(startPage.OrderPadsPage);

            return Redirect(pageUrl);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult RequestSharedCartQuote()
        {
            var currentCustomer = _customerService.GetCurrentContact();
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);

            var sharedCart = _cartService.LoadSharedCardByCustomerId(new Guid(OrganizationId));
            var savedCart = _cartService.LoadOrCreateCart(_cartService.DefaultSharedCardName, currentCustomer.ContactId.ToString());

            //clone all items in shared cart to savedCart 
            var allLineItem = sharedCart.GetAllLineItems();
            foreach (var lineitem in allLineItem)
            {
                _cartService.AddToCart(savedCart, lineitem.Code, lineitem.Quantity, "delivery", "");
            }

            //Used saved cart to place
            if (savedCart != null)
            {
                // Set price on line item.
                foreach (var lineItem in savedCart.GetAllLineItems())
                {
                    lineItem.PlacedPrice = _cartService.GetDiscountedPrice(savedCart, lineItem).Value.Amount;
                }

                _cartServiceB2B.PlaceCartForQuote(savedCart);
                _cartServiceB2B.DeleteCart(savedCart);
                _cartServiceB2B.DeleteCart(sharedCart);
                _cartService.LoadOrCreateCart(_cartService.DefaultSharedCardName, OrganizationId);

                return RedirectToAction("Index", "SharedCart");
            }

            return RedirectToAction("Index", new { Node = startPage.OrderHistoryPage });
        }

        private CartWithValidationIssues SharedCart => _SharedCart ?? (_SharedCart = _cartService.LoadCart(_cartService.DefaultSharedCardName, OrganizationId, true));
        private string OrganizationId => _customerService.GetCurrentContact().Organization?.OrganizationId.ToString();
    }
}