using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Find.Helpers.Text;
using EPiServer.Reference.Commerce.B2B;
using EPiServer.Reference.Commerce.B2B.Enums;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Cart.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels;
using EPiServer.Security;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Security;
using System.Text;
using EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models;
using EPiServer.Reference.Commerce.Shared.Services;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Controllers
{
    public class CartController : PageController<CartPage>
    {
        private readonly ICartService _cartService;
        private CartWithValidationIssues _cart;
        private CartWithValidationIssues _wishlist;
        private CartWithValidationIssues _sharedcart;
        private readonly IOrderRepository _orderRepository;
        private readonly IRecommendationService _recommendationService;
        readonly CartViewModelFactory _cartViewModelFactory;
        private readonly IContentLoader _contentLoader;
        private readonly IContentRouteHelper _contentRouteHelper;
        private readonly ReferenceConverter _referenceConverter;
        private readonly ICartServiceB2B _cartServiceB2B;
        private readonly IQuickOrderService _quickOrderService;
        private readonly ICustomerService _customerService;
        private readonly ShipmentViewModelFactory _shipmentViewModelFactory;
        private readonly CheckoutService _checkoutService;
        readonly IOrderGroupCalculator _orderGroupCalculator;


        private const string b2bMinicart = "~/Views/Shared/_MiniCartDetails.cshtml";
        private const string b2cMinicart = "~/Views/Shared/Header/_HeaderCart.cshtml";

        public CartController(
            ICartService cartService,
            IOrderRepository orderRepository,
            IRecommendationService recommendationService,
            CartViewModelFactory cartViewModelFactory,
            IContentLoader contentLoader,
            IContentRouteHelper contentRouteHelper,
            ReferenceConverter referenceConverter,
            ICartServiceB2B cartServiceB2B,
            IQuickOrderService quickOrderService,
            ICustomerService customerService,
            ShipmentViewModelFactory shipmentViewModelFactory,
            CheckoutService checkoutService,
            IOrderGroupCalculator orderGroupCalculator)

        {
            _cartService = cartService;
            _orderRepository = orderRepository;
            _recommendationService = recommendationService;
            _cartViewModelFactory = cartViewModelFactory;
            _contentLoader = contentLoader;
            _contentRouteHelper = contentRouteHelper;
            _referenceConverter = referenceConverter;
            _cartServiceB2B = cartServiceB2B;
            _quickOrderService = quickOrderService;
            _customerService = customerService;
            _shipmentViewModelFactory = shipmentViewModelFactory;
            _checkoutService = checkoutService;
            _orderGroupCalculator = orderGroupCalculator;

        }

        private CartWithValidationIssues CartWithValidationIssues => _cart ?? (_cart = _cartService.LoadCart(_cartService.DefaultCartName, true));

        private CartWithValidationIssues WishListWithValidationIssues => _wishlist ?? (_wishlist = _cartService.LoadCart(_cartService.DefaultWishListName, true));

        private CartWithValidationIssues SharedCardWithValidationIssues => _sharedcart ?? (_sharedcart = _cartService.LoadCart(_cartService.DefaultSharedCardName, true));

        private CartWithValidationIssues SharedCart => _sharedcart ?? (_sharedcart = _cartService.LoadCart(_cartService.DefaultSharedCardName, OrganizationId, true));

        private string OrganizationId => _customerService.GetCurrentContact().Organization?.OrganizationId.ToString();

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult MiniCartDetails()
        {
            var viewModel = _cartViewModelFactory.CreateMiniCartViewModel(CartWithValidationIssues.Cart);
            var refStartPage = SiteDefinition.Current.StartPage;
            var startPage = _contentLoader.Get<BaseStartPage>(refStartPage);
            return PartialView(b2cMinicart, viewModel);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public async Task<ActionResult> Index(CartPage currentPage)
        {
            var messages = string.Empty;
            if (CartWithValidationIssues.Cart != null && CartWithValidationIssues.ValidationIssues.Any())
            {
                foreach (var item in CartWithValidationIssues.Cart.GetAllLineItems())
                {
                    messages = GetValidationMessages(item, CartWithValidationIssues.ValidationIssues);
                }
            }

            var viewModel = _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage);
            viewModel.Message = messages;
            var trackingResponse = await _recommendationService.TrackCart(HttpContext, CartWithValidationIssues.Cart);
            viewModel.Recommendations = trackingResponse.GetCartRecommendations(_referenceConverter);
            return View("LargeCart", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> AddToCart(string code, int qty = 1, string store = "delivery", string selectedStore = "")
        {
            var warningMessage = string.Empty;

            ModelState.Clear();

            if (CartWithValidationIssues.Cart == null)
            {
                _cart = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultCartName),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            var result = _cartService.AddToCart(CartWithValidationIssues.Cart, code, qty, store, selectedStore);
            if (result.EntriesAddedToCart)
            {
                _orderRepository.Save(CartWithValidationIssues.Cart);
                await _recommendationService.TrackCart(HttpContext, CartWithValidationIssues.Cart);
                return MiniCartDetails();
            }
            
            return new HttpStatusCodeResult(500, result.GetComposedValidationMessage());
        }

        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> AddAllToCart()
        {
            ModelState.Clear();

            var allLineItem = SharedCart.Cart.GetAllLineItems();
            bool entriesAddedToCart = true;
            string validationMessage = "";

            foreach (var lineitem in allLineItem)
            {
                var result = _cartService.AddToCart(CartWithValidationIssues.Cart, lineitem.Code, lineitem.Quantity, "delivery", "");
                entriesAddedToCart &= result.EntriesAddedToCart;
                validationMessage += result.GetComposedValidationMessage();
            }

            if (entriesAddedToCart)
            {
                _orderRepository.Save(CartWithValidationIssues.Cart);
                await _recommendationService.TrackCart(HttpContext, CartWithValidationIssues.Cart);
                return MiniCartDetails();
            }

            return new HttpStatusCodeResult(500, validationMessage);
        }

        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> Subscription(string code, int qty = 1, string store = "delivery", string selectedStore = "")
        {
            var warningMessage = string.Empty;

            ModelState.Clear();

            if (CartWithValidationIssues.Cart == null)
            {
                _cart = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultCartName),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            
            var result = _cartService.AddToCart(CartWithValidationIssues.Cart, code, qty, store, selectedStore);
            if (result.EntriesAddedToCart)
            {
                var item = CartWithValidationIssues.Cart.GetAllLineItems().FirstOrDefault(x => x.Code.Equals(code));
                var subscriptionPrice = PriceCalculationService.GetSubscriptionPrice(code, CartWithValidationIssues.Cart.MarketId, CartWithValidationIssues.Cart.Currency);
                if (subscriptionPrice != null)
                {
                    item.Properties["SubscriptionPrice"] = subscriptionPrice.UnitPrice.Amount;
                    item.PlacedPrice = subscriptionPrice.UnitPrice.Amount;
                }
                
                _orderRepository.Save(CartWithValidationIssues.Cart);
                await _recommendationService.TrackCart(HttpContext, CartWithValidationIssues.Cart);
                return MiniCartDetails();
            }

            return new HttpStatusCodeResult(500, result.GetComposedValidationMessage());
        }

        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> BuyNow(string code, int qty = 1, string store = "delivery", string selectedStore = "")
        {
            var warningMessage = string.Empty;

            ModelState.Clear();

            if (CartWithValidationIssues.Cart == null)
            {
                _cart = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultCartName),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            var result = _cartService.AddToCart(CartWithValidationIssues.Cart, code, qty, store, selectedStore);
            if (!result.EntriesAddedToCart)
            {
                return new HttpStatusCodeResult(500, result.GetComposedValidationMessage());
            }
            var contact = PrincipalInfo.CurrentPrincipal.GetCustomerContact();
            if (contact == null)
            {
                return MiniCartDetails();
            }

            var creditCard = contact.ContactCreditCards.FirstOrDefault();
            if (creditCard == null)
            {
                return MiniCartDetails();
            }

            var shipment = CartWithValidationIssues.Cart.GetFirstShipment();
            if (shipment == null)
            {
                return MiniCartDetails();
            }

            var shippingAddress = (contact.PreferredShippingAddress ?? contact.ContactAddresses.FirstOrDefault())?.ConvertToOrderAddress(CartWithValidationIssues.Cart);
            if (shippingAddress == null)
            {
                return MiniCartDetails();
            }

            shipment.ShippingAddress = shippingAddress;

            var shippingMethodViewModels = _shipmentViewModelFactory.CreateShipmentsViewModel(CartWithValidationIssues.Cart).SelectMany(x => x.ShippingMethods);
            var shippingMethodViewModel = shippingMethodViewModels.Where(x => x.Price != 0)
                .OrderBy(x => x.Price)
                .FirstOrDefault();

            //If product is virtual set shipping method is Free
            if (shipment.LineItems.FirstOrDefault().GetEntryContentBase() is VirtualVariant)
            {
                shippingMethodViewModel = shippingMethodViewModels.Where(x => x.Price == 0).FirstOrDefault();
            }

            if (shippingMethodViewModel == null)
            {
                return MiniCartDetails();
            }

            shipment.ShippingMethodId = shippingMethodViewModel.Id;

            var paymentAddress = (contact.PreferredBillingAddress ?? contact.ContactAddresses.FirstOrDefault())?.ConvertToOrderAddress(CartWithValidationIssues.Cart);
            if (paymentAddress == null)
            {
                return MiniCartDetails();
            }

            var totals = _orderGroupCalculator.GetOrderGroupTotals(CartWithValidationIssues.Cart);

            var payment = CartWithValidationIssues.Cart.CreateCardPayment();
            payment.BillingAddress = paymentAddress;
            payment.CardType = "Credit card";
            payment.PaymentMethodId = new Guid("B1DA37A6-CF19-40D5-915B-B863D74D8799");
            payment.PaymentMethodName = "GenericCreditCard";
            payment.Amount = CartWithValidationIssues.Cart.GetTotal().Amount;
            payment.CreditCardNumber = creditCard.CreditCardNumber;
            payment.CreditCardSecurityCode = creditCard.SecurityCode;
            payment.ExpirationMonth = creditCard.ExpirationMonth ?? 1;
            payment.ExpirationYear = creditCard.ExpirationYear ?? DateTime.Now.Year;
            payment.Status = PaymentStatus.Pending.ToString();
            payment.CustomerName = contact.FullName;
            payment.TransactionType = TransactionType.Authorization.ToString();
            CartWithValidationIssues.Cart.GetFirstForm().Payments.Add(payment);

            var issues = _cartService.ValidateCart(CartWithValidationIssues.Cart);
            if (issues.Keys.Any(x => issues.HasItemBeenRemoved(x)))
            {
                return MiniCartDetails();
            }
            var order = _checkoutService.PlaceOrder(CartWithValidationIssues.Cart, new ModelStateDictionary(), new CheckoutViewModel());

            await _checkoutService.CreateOrUpdateBoughtProductsProfileStore(CartWithValidationIssues.Cart);
            await _checkoutService.CreateBoughtProductsSegments(CartWithValidationIssues.Cart);
            await _recommendationService.TrackOrder(HttpContext, order);
            return Json(new { contactId = contact.PrimaryKeyId?.ToString(), orderNumber = order.OrderLink.OrderGroupId }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowDBWrite]
        public ActionResult AddToWishlist(string code)
        {
            ModelState.Clear();
            var currentPage = _contentRouteHelper.Content as CartPage;
            if (WishListWithValidationIssues.Cart == null)
            {
                _wishlist = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultWishListName),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            var items = new Dictionary<int, string>();
            foreach (var shipment in CartWithValidationIssues.Cart.Forms.SelectMany(x => x.Shipments))
            {
                foreach (var lineItem in shipment.LineItems)
                {
                    if (!lineItem.Code.Equals(code))
                    {
                        continue;
                    }
                    items.Add(shipment.ShipmentId, code);
                }
            }
            foreach (var key in items.Keys)
            {
                _cartService.ChangeCartItem(CartWithValidationIssues.Cart, key, items[key], 0, "", "");
            }
            _orderRepository.Save(CartWithValidationIssues.Cart);

            if (WishListWithValidationIssues.Cart.GetAllLineItems().Any(item => item.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                return View("LargeCart", _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage));
            }

            var result = _cartService.AddToCart(WishListWithValidationIssues.Cart, code, 1, "delivery", "");
            if (!result.EntriesAddedToCart)
            {
                return new HttpStatusCodeResult(500, result.GetComposedValidationMessage());
            }
            
            _orderRepository.Save(WishListWithValidationIssues.Cart);
            
            var viewModel = _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage); ;
            return View("LargeCart", viewModel);
            
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult AddToSharedCart(string code)
        {
            ModelState.Clear();
            var currentPage = _contentRouteHelper.Content as CartPage;
            if (SharedCardWithValidationIssues.Cart == null)
            {
                _sharedcart = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultSharedCardName, _customerService.GetCurrentContact().Organization?.OrganizationId.ToString()),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            var items = new Dictionary<int, string>();
            foreach (var shipment in CartWithValidationIssues.Cart.Forms.SelectMany(x => x.Shipments))
            {
                foreach (var lineItem in shipment.LineItems)
                {
                    if (!lineItem.Code.Equals(code))
                    {
                        continue;
                    }
                    items.Add(shipment.ShipmentId, code);
                }
            }
            foreach (var key in items.Keys)
            {
                _cartService.ChangeCartItem(CartWithValidationIssues.Cart, key, items[key], 0, "", "");
            }
            _orderRepository.Save(CartWithValidationIssues.Cart);

            if (SharedCardWithValidationIssues.Cart.GetAllLineItems().Any(item => item.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                return View("LargeCart", _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage));
            }

            var result = _cartService.AddToCart(SharedCardWithValidationIssues.Cart, code, 1, "delivery", "");
            if (!result.EntriesAddedToCart)
            {
                return new HttpStatusCodeResult(500, result.GetComposedValidationMessage());
            }

            _orderRepository.Save(SharedCardWithValidationIssues.Cart);

            var viewModel = _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage); ;
            return View("LargeCart", viewModel);

        }

        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> Reorder(string orderId)
        {
            int orderIntId;

            if (!int.TryParse(orderId, out orderIntId))
            {
                return new HttpStatusCodeResult(500, "Error reordering order");
            }
            var order = _orderRepository.Load<IPurchaseOrder>(orderIntId);

            if (order == null)
            {
                return new HttpStatusCodeResult(500, "Error reordering order");
            }
            ModelState.Clear();

            if (CartWithValidationIssues.Cart == null)
            {
                _cart = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultCartName),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            var lineitems = order.Forms.First().GetAllLineItems();
            foreach (var item in lineitems)
            {
                var result = _cartService.AddToCart(CartWithValidationIssues.Cart, item.Code, item.Quantity, "delivery", "");
                if (result.EntriesAddedToCart)
                {
                    await _recommendationService.TrackCart(HttpContext, CartWithValidationIssues.Cart);
                }
                else
                {
                    return new HttpStatusCodeResult(500, result.GetComposedValidationMessage());
                }
            }

            _orderRepository.Save(CartWithValidationIssues.Cart);
            return Redirect(Url.ContentUrl(_contentLoader.Get<BaseStartPage>(ContentReference.StartPage).CheckoutPage));
        }

        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> ChangeCartItem(int shipmentId, string code, decimal quantity, string size, string newSize)
        {
            ModelState.Clear();

            var validationIssues = _cartService.ChangeCartItem(CartWithValidationIssues.Cart, shipmentId, code, quantity, size, newSize);
            _orderRepository.Save(CartWithValidationIssues.Cart);
            var model = _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, null);
            if (validationIssues.Any())
            {
                foreach (var item in validationIssues.Keys)
                {
                    model.Message += GetValidationMessages(item, validationIssues);
                }
            }
            var trackingResponse = await _recommendationService.TrackCart(HttpContext, CartWithValidationIssues.Cart);
            //model.Recommendations = trackingResponse.GetCartRecommendations(_referenceConverter);
            return model.StartPage.PageTypeName == "MoseySupplyStartPage" 
                    ? MiniCartDetails() 
                    : PartialView("LargeCart", model);
        }

        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> RemoveCartItem(int shipmentId, string code, decimal quantity, string size, string newSize)
        {
            ModelState.Clear();

            _cartService.ChangeCartItem(CartWithValidationIssues.Cart, shipmentId, code, quantity, size, newSize);
            _orderRepository.Save(CartWithValidationIssues.Cart);
            await _recommendationService.TrackCart(HttpContext, CartWithValidationIssues.Cart);
            return MiniCartDetails();
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult AddCouponCode(CartPage currentPage, string couponCode)
        {
            if (_cartService.AddCouponCode(CartWithValidationIssues.Cart, couponCode))
            {
                _orderRepository.Save(CartWithValidationIssues.Cart);
            }
            var viewModel = _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage); ;
            return View("LargeCart", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult RemoveCouponCode(CartPage currentPage, string couponCode)
        {
            _cartService.RemoveCouponCode(CartWithValidationIssues.Cart, couponCode);
            _orderRepository.Save(CartWithValidationIssues.Cart);
            var viewModel = _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage); ;
            return View("LargeCart", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult EstimateShipping(CartPage currentPage, LargeCartViewModel largeCartViewModel)
        {
            var orderAddress = CartWithValidationIssues.Cart.GetFirstShipment().ShippingAddress;
            if (orderAddress == null)
            {
                orderAddress = CartWithValidationIssues.Cart.CreateOrderAddress(Guid.NewGuid().ToString());
                CartWithValidationIssues.Cart.GetFirstShipment().ShippingAddress = orderAddress;
            }

            orderAddress.CountryName = largeCartViewModel.AddressModel.CountryName;
            orderAddress.CountryCode = largeCartViewModel.AddressModel.CountryCode;
            orderAddress.RegionName = largeCartViewModel.AddressModel.CountryRegion.Region;
            orderAddress.PostalCode = largeCartViewModel.AddressModel.PostalCode;

            _orderRepository.Save(CartWithValidationIssues.Cart);
            var viewModel = _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage);
            return View("LargeCart", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult ClearCart(CartPage currentPage)
        {
            if (CartWithValidationIssues.Cart != null)
            {
                _orderRepository.Delete(CartWithValidationIssues.Cart.OrderLink);
                _cart = null;
            }
            var viewModel = _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage); ;
            return View("LargeCart", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> RemoveItem(CartPage currentPage, int shipmentId, string code)
        {
            var message = string.Empty;
            var issues = _cartService.ChangeCartItem(CartWithValidationIssues.Cart, shipmentId, code, 0, "", "");
            _orderRepository.Save(CartWithValidationIssues.Cart);
            await _recommendationService.TrackCart(HttpContext, CartWithValidationIssues.Cart);
            var viewModel = _cartViewModelFactory.CreateLargeCartViewModel(CartWithValidationIssues.Cart, currentPage);
            if (issues.Any())
            {
                foreach (var item in issues.Keys)
                {
                    viewModel.Message += GetValidationMessages(item, issues);
                }
            }
            return View("LargeCart", viewModel);
        }
        [HttpPost]
        [AllowDBWrite]
        public ActionResult RequestQuote()
        {
            bool succesRequest;

            if (CartWithValidationIssues.Cart == null)
            {
                _cart = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultCartName),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
                succesRequest = _cartServiceB2B.PlaceCartForQuote(_cart.Cart);
            }
            else
            {
                succesRequest = _cartServiceB2B.PlaceCartForQuote(CartWithValidationIssues.Cart);
            }
            _cartServiceB2B.DeleteCart(_cart.Cart);
            _cart = new CartWithValidationIssues
            {
                Cart = _cartServiceB2B.CreateNewCart(),
                ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
            };

            return Json(new { result = succesRequest });
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult RequestQuoteById(int orderId)
        {
            var currentCustomer = _customerService.GetCurrentContact();
            if (currentCustomer.Role != B2BUserRoles.Purchaser)
                return Json(new { result = false });

            var placedOrderId = _cartServiceB2B.PlaceCartForQuoteById(orderId, currentCustomer.ContactId);

            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);

            return RedirectToAction("Index", "OrderDetails",
                new { currentPage = startPage.OrderDetailsPage, orderGroupId = placedOrderId });
        }

        [HttpPost]
        [AllowDBWrite]
        public JsonResult ClearQuotedCart()
        {
            _cartServiceB2B.DeleteCart(CartWithValidationIssues.Cart);
            _cart = new CartWithValidationIssues
            {
                Cart = _cartServiceB2B.CreateNewCart(),
                ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
            };

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowDBWrite]
        public JsonResult AddVariantsToCart(List<string> variants)
        {
            var returnedMessages = new List<string>();

            ModelState.Clear();

            if (CartWithValidationIssues.Cart == null)
            {
                _cart = new CartWithValidationIssues
                {
                    Cart = _cartService.LoadOrCreateCart(_cartService.DefaultCartName),
                    ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
                };
            }

            foreach (var product in variants)
            {
                var sku = product.Split(';')[0];
                var quantity = Convert.ToInt32(product.Split(';')[1]);

                var variationReference = _referenceConverter.GetContentLink(sku);

                var responseMessage = _quickOrderService.ValidateProduct(variationReference, Convert.ToDecimal(quantity), sku);
                if (responseMessage.IsNullOrEmpty())
                {
                    var result = _cartService.AddToCart(CartWithValidationIssues.Cart, sku, quantity, "delivery", "");
                    if (result.EntriesAddedToCart)
                    {
                        _cartService.ChangeCartItem(CartWithValidationIssues.Cart, 0, sku, quantity, "", "");
                        _orderRepository.Save(CartWithValidationIssues.Cart);
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

        private static string GetValidationMessages(ILineItem lineItem, Dictionary<ILineItem, List<ValidationIssue>> validationIssues)
        {
            var message = string.Empty;
            foreach (var validationIssue in validationIssues)
            {
                var warning = new StringBuilder();
                warning.Append(string.Format("Line Item with code {0} ", lineItem.Code));
                validationIssue.Value.Aggregate(warning, (current, issue) => current.Append(issue).Append(", "));

                message += (warning.ToString().TrimEnd(',', ' '));
            }
            return message;
        }
    }
}