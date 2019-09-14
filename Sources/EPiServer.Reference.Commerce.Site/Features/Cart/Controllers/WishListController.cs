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
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Controllers
{
    [Authorize]
    public class WishListController : PageController<WishListPage>
    {
        private readonly IContentLoader _contentLoader;
        private readonly ICartService _cartService;
        private CartWithValidationIssues _wishlist;
        private readonly IOrderRepository _orderRepository;
        private readonly IRecommendationService _recommendationService;
        readonly CartViewModelFactory _cartViewModelFactory;
        private readonly IQuickOrderService _quickOrderService;
        private readonly ReferenceConverter _referenceConverter;
        private readonly ICustomerService _customerService;
        private readonly ICartServiceB2B _cartServiceB2B;

        public WishListController(
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
        [CommerceTracking(TrackingType.Wishlist)]
        public ActionResult Index(WishListPage currentPage)
        {
            var viewModel = _cartViewModelFactory.CreateWishListViewModel(WishList.Cart, currentPage);
            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult WishListMiniCartDetails()
        {
            var viewModel = _cartViewModelFactory.CreateMiniWishListViewModel(WishList.Cart);
            return PartialView("_WishListMiniCartDetails", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult AddToCart(string code)
        {
            ModelState.Clear();

            if (WishList.Cart == null)
            {
                _wishlist = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultWishListName),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            if (WishList.Cart.GetAllLineItems().Any(item => item.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                return WishListMiniCartDetails();
            }

            var result = _cartService.AddToCart(WishList.Cart, code, 1, "delivery", "");
            if (result.EntriesAddedToCart)
            {
                _orderRepository.Save(WishList.Cart);
                _recommendationService.TrackWishlist(HttpContext);
                if(Request.IsAjaxRequest())
                {
                    return WishListMiniCartDetails();
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

            _cartService.ChangeCartItem(WishList.Cart, 0, code, quantity, size, newSize);
            _orderRepository.Save(WishList.Cart);
            _recommendationService.TrackWishlist(HttpContext);
            if (Request.IsAjaxRequest())
            {
                return WishListMiniCartDetails();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult DeleteWishList()
        {
            if (WishList.Cart != null)
            {
                _orderRepository.Delete(WishList.Cart.OrderLink);
            }
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);

            return RedirectToAction("Index", new { Node = startPage.WishListPage });
        }

        [HttpPost]
        [AllowDBWrite]
        public JsonResult AddVariantsToOrderPad(List<string> variants)
        {
            var returnedMessages = new List<string>();

            ModelState.Clear();

            if (WishList.Cart == null)
            {
                _wishlist = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultWishListName),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            foreach (var product in variants)
            {
                var sku = product.Split(';')[0];
                var quantity = Convert.ToInt32(product.Split(';')[1]);

                var variationReference = _referenceConverter.GetContentLink(sku);

                var responseMessage = _quickOrderService.ValidateProduct(variationReference, Convert.ToDecimal(quantity), sku);
                if (string.IsNullOrEmpty(responseMessage))
                {
                    if (_cartService.AddToCart(WishList.Cart, sku, 1, "delivery", "").EntriesAddedToCart)
                    {
                        _orderRepository.Save(WishList.Cart);
                    }
                }
                else
                {
                    returnedMessages.Add(responseMessage);
                }
            }
            Session[Constants.ErrorMesages] = returnedMessages;

            return Json(returnedMessages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult RemoveCartItem(string code, string userId)
        {
            ModelState.Clear();
            var userWishCart = _cartService.LoadWishListCardByCustomerId(new Guid(userId));
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
        public ActionResult RequestWishListQuote()
        {
            var currentCustomer = _customerService.GetCurrentContact();
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);

            var wishListCart = _cartService.LoadWishListCardByCustomerId(currentCustomer.ContactId);
            if (wishListCart != null)
            {
                // Set price on line item.
                foreach (var lineItem in wishListCart.GetAllLineItems())
                {
                    lineItem.PlacedPrice = _cartService.GetDiscountedPrice(wishListCart, lineItem).Value.Amount;
                }

                _cartServiceB2B.PlaceCartForQuote(wishListCart);
                _cartServiceB2B.DeleteCart(wishListCart);
                _cartService.LoadOrCreateCart(_cartService.DefaultWishListName);

                return RedirectToAction("Index", "WishList");
            }

            return RedirectToAction("Index", new { Node = startPage.OrderHistoryPage });
        }

        private CartWithValidationIssues WishList => _wishlist ?? (_wishlist = _cartService.LoadCart(_cartService.DefaultWishListName, true));
    }
}