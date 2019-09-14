using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.B2B;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Login.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.Services;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Payment.PaymentMethods;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Controllers
{
    public class CheckoutController : PageController<CheckoutPage>
    {
        private readonly ControllerExceptionHandler _controllerExceptionHandler;
        private readonly CheckoutViewModelFactory _checkoutViewModelFactory;
        private readonly OrderSummaryViewModelFactory _orderSummaryViewModelFactory;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IRecommendationService _recommendationService;
        private CartWithValidationIssues _cart;
        private readonly CheckoutService _checkoutService;
        private readonly UrlHelper _urlHelper;
        private readonly ApplicationSignInManager<SiteUser> _applicationSignInManager;
        private readonly LocalizationService _localizationService;
        private readonly IAddressBookService _addressBookService;
        private readonly MultiShipmentViewModelFactory _multiShipmentViewModelFactory;
        private readonly IOrderGroupFactory _orderGroupFactory;
        private readonly ICartServiceB2B _cartServiceB2B;
        private readonly IContentLoader _contentLoader;
        private readonly UrlResolver _urlResolver;
        private readonly CustomerContextFacade _customerContext;
        private readonly IOrganizationService _organizationService;

        public CheckoutController(
            ControllerExceptionHandler controllerExceptionHandler,
            IOrderRepository orderRepository,
            CheckoutViewModelFactory checkoutViewModelFactory,
            ICartService cartService,
            OrderSummaryViewModelFactory orderSummaryViewModelFactory,
            IRecommendationService recommendationService,
            CheckoutService checkoutService,
            UrlHelper urlHelper,
            ApplicationSignInManager<SiteUser> applicationSignInManager,
            LocalizationService localizationService,
            IAddressBookService addressBookService,
            MultiShipmentViewModelFactory multiShipmentViewModelFactory,
            IOrderGroupFactory orderGroupFactory,
            ICartServiceB2B cartServiceB2B,
            IContentLoader contentLoader,
            UrlResolver urlResolver,
            CustomerContextFacade customerContext,
            IOrganizationService organizationService)
        {
            _controllerExceptionHandler = controllerExceptionHandler;
            _orderRepository = orderRepository;
            _checkoutViewModelFactory = checkoutViewModelFactory;
            _cartService = cartService;
            _orderSummaryViewModelFactory = orderSummaryViewModelFactory;
            _recommendationService = recommendationService;
            _checkoutService = checkoutService;
            _urlHelper = urlHelper;
            _applicationSignInManager = applicationSignInManager;
            _localizationService = localizationService;
            _addressBookService = addressBookService;
            _multiShipmentViewModelFactory = multiShipmentViewModelFactory;
            _orderGroupFactory = orderGroupFactory;
            _cartServiceB2B = cartServiceB2B;
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _customerContext = customerContext;
            _organizationService = organizationService;
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult Index(CheckoutPage currentPage)
        {
            if (CartIsNullOrEmpty())
            {
                return View("EmptyCart", new CheckoutMethodViewModel());
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("CheckoutMethod", new { node = currentPage.ContentLink });
            }

            if (CartWithValidationIssues.Cart.GetFirstForm().Shipments.Count(x => x.ShippingMethodId != _cartService.InStorePickupInfoModel.MethodId) == 1)
            {
                return RedirectToAction("SingleAddress", new { node = currentPage.ContentLink });
            }

            if (CartWithValidationIssues.Cart.GetFirstForm().Shipments.All(x => x.ShippingMethodId == _cartService.InStorePickupInfoModel.MethodId))
            {
                return RedirectToAction("BillingInformation", new { node = currentPage.ContentLink });
            }

            return RedirectToAction("MutipleAddresses", new { node = currentPage.ContentLink });
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult CheckoutMethod(CheckoutPage currentPage)
        {
            var viewModel = new CheckoutMethodViewModel(currentPage, _urlHelper.Action("Index", "Checkout"));
            return View("CheckoutMethod",viewModel);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult ShippingInformation(CheckoutPage currentPage)
        {
            var viewModel = CreateCheckoutViewModel(currentPage);
            return View("ShippingInformation", viewModel);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult BillingInformation(CheckoutPage currentPage, int? addressType)
        {
            var viewModel = CreateCheckoutViewModel(currentPage);
            viewModel.OrderSummary = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
            if (addressType == null && Request.IsAuthenticated)
            {
                viewModel.AddressType = 1;
            }
            else if (addressType == null)
            {
                viewModel.AddressType = 0;
            }
            else
            {
                viewModel.AddressType = addressType.Value;
            }
            return View("BillingInformation", viewModel);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult AddPayment(CheckoutPage currentPage)
        {
            var viewModel = CreateCheckoutViewModel(currentPage);
            viewModel.OrderSummary = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
            return PartialView("AddPayment", viewModel);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult PlaceOrder(CheckoutPage currentPage)
        {
            var viewModel = CreateCheckoutViewModel(currentPage);
            viewModel.OrderSummary = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
            return View("PlaceOrder", viewModel);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult PunchoutOrder(CheckoutPage currentPage)
        {
            var viewModel = CreateCheckoutViewModel(currentPage);
            viewModel.OrderSummary = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
            return View("PunchoutOrder", viewModel);
        }

        [HttpGet]
        [OutputCache(Duration = 0, NoStore = true)]
        public ActionResult SingleAddress(CheckoutPage currentPage, int? addressType)
        {
            var viewModel = CreateCheckoutViewModel(currentPage);
            if (addressType == null && Request.IsAuthenticated)
            {
                viewModel.AddressType = 1;
            }
            else if (addressType == null)
            {
                viewModel.AddressType = 0;
            }
            else
            {
                viewModel.AddressType = addressType.Value;
            }
            return Request.IsAjaxRequest() ? PartialView("SingleAddress", viewModel) : (ActionResult)View("SingleAddress", viewModel);
        }

        [HttpGet]
        public ActionResult MutipleAddresses(CheckoutPage currentPage)
        {
            var viewModel = _multiShipmentViewModelFactory.CreateMultiShipmentViewModel(CartWithValidationIssues.Cart, currentPage, User.Identity.IsAuthenticated);
            return View("MultipleAddresses", viewModel);
        }

        [HttpGet]
        public ActionResult SingleShipment(CheckoutPage currentPage)
        {
            if (!CartIsNullOrEmpty())
            {
                _cartService.MergeShipments(CartWithValidationIssues.Cart);
                _orderRepository.Save(CartWithValidationIssues.Cart);
            }

            return RedirectToAction("Index", new { node = currentPage.ContentLink });
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult Update(CheckoutPage currentPage, UpdateShippingMethodViewModel shipmentViewModel, IPaymentMethod paymentOption)
        {
            ModelState.Clear();

            _checkoutService.UpdateShippingMethods(CartWithValidationIssues.Cart, shipmentViewModel.Shipments);
            _checkoutService.ApplyDiscounts(CartWithValidationIssues.Cart);
            _orderRepository.Save(CartWithValidationIssues.Cart);

            var viewModel = CreateCheckoutViewModel(currentPage, paymentOption);

            return PartialView("Partial", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult ChangeAddress(UpdateAddressViewModel addressViewModel)
        {
            ModelState.Clear();
            var viewModel = CreateCheckoutViewModel(addressViewModel.CurrentPage);
            _checkoutService.CheckoutAddressHandling.ChangeAddress(viewModel, addressViewModel);

            _checkoutService.UpdateShippingAddresses(CartWithValidationIssues.Cart, viewModel);

            _orderRepository.Save(CartWithValidationIssues.Cart);

            var addressViewName = addressViewModel.ShippingAddressIndex > -1 ? "SingleShippingAddress" : "BillingAddress";

            return PartialView(addressViewName, viewModel);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult OrderSummary()
        {
            var viewModel = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
            return PartialView(viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult AddCouponCode(CheckoutPage currentPage, string couponCode)
        {
            if (_cartService.AddCouponCode(CartWithValidationIssues.Cart, couponCode))
            {
                _orderRepository.Save(CartWithValidationIssues.Cart);
            }
            var viewModel = CreateCheckoutViewModel(currentPage);
            viewModel.OrderSummary = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
            return View("PlaceOrder", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult RemoveCouponCode(CheckoutPage currentPage, string couponCode)
        {
            _cartService.RemoveCouponCode(CartWithValidationIssues.Cart, couponCode);
            _orderRepository.Save(CartWithValidationIssues.Cart);
            var viewModel = CreateCheckoutViewModel(currentPage);
            viewModel.OrderSummary = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
            return View("PlaceOrder", viewModel);
        }
        
        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> Purchase(CheckoutViewModel viewModel, IPaymentMethod paymentOption)
        {
            if (CartIsNullOrEmpty())
            {
                return Redirect(Url.ContentUrl(ContentReference.StartPage));
            }

            // Since the payment property is marked with an exclude binding attribute in the CheckoutViewModel
            // it needs to be manually re-added again.
            //viewModel.Payments = paymentOption;
            
            if (User.Identity.IsAuthenticated)
            {
                _checkoutService.CheckoutAddressHandling.UpdateAuthenticatedUserAddresses(viewModel);

                var validation = _checkoutService.AuthenticatedPurchaseValidation;

                if (!validation.ValidateModel(ModelState, viewModel) ||
                    !validation.ValidateOrderOperation(ModelState, _cartService.ValidateCart(CartWithValidationIssues.Cart)) ||
                    !validation.ValidateOrderOperation(ModelState, _cartService.RequestInventory(CartWithValidationIssues.Cart)))
                {
                    return View(viewModel);
                }
            }
            else
            {
                _checkoutService.CheckoutAddressHandling.UpdateAnonymousUserAddresses(viewModel);

                var validation = _checkoutService.AnonymousPurchaseValidation;
              
                if (!validation.ValidateModel(ModelState, viewModel) ||
                    !validation.ValidateOrderOperation(ModelState, _cartService.ValidateCart(CartWithValidationIssues.Cart)) ||
                    !validation.ValidateOrderOperation(ModelState, _cartService.RequestInventory(CartWithValidationIssues.Cart)))
                {
                    return View(viewModel);
                }
            }
            
            if (!paymentOption.ValidateData())
            {
                return View(viewModel);
            }

            _checkoutService.UpdateShippingAddresses(CartWithValidationIssues.Cart, viewModel);
            
            _checkoutService.CreateAndAddPaymentToCart(CartWithValidationIssues.Cart, viewModel);
            
            var purchaseOrder = _checkoutService.PlaceOrder(CartWithValidationIssues.Cart, ModelState, viewModel);
            if (purchaseOrder == null)
            {
                return View(viewModel);
            }

            if (Request.IsAuthenticated)
            {
                var contact = _customerContext.CurrentContact.CurrentContact;
                var organization = contact.ContactOrganization;
                if (organization != null)
                {
                    purchaseOrder.Properties[Constants.Customer.CustomerFullName] = contact.FullName;
                    purchaseOrder.Properties[Constants.Customer.CustomerEmailAddress] = contact.Email;
                    purchaseOrder.Properties[Constants.Customer.CurrentCustomerOrganization] = organization.Name;
                    _orderRepository.Save(purchaseOrder);
                }
            }

            var confirmationSentSuccessfully = _checkoutService.SendConfirmation(viewModel, purchaseOrder);
            await _checkoutService.CreateOrUpdateBoughtProductsProfileStore(CartWithValidationIssues.Cart);
            await _checkoutService.CreateBoughtProductsSegments(CartWithValidationIssues.Cart);
            await _recommendationService.TrackOrder(HttpContext, purchaseOrder);

            return Redirect(_checkoutService.BuildRedirectionUrl(viewModel, purchaseOrder, confirmationSentSuccessfully));
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult GuestOrRegister(string checkoutMethod)
        {
            if (CartIsNullOrEmpty())
            {
                return View("EmptyCart", new CheckoutMethodViewModel());
            }

            var content = Request.RequestContext.GetRoutedData<CheckoutPage>();
            if (checkoutMethod.Equals("register"))
            {
                return RedirectToAction("Index", "Login", new { returnUrl = content != null ? _urlHelper.ContentUrl(content.ContentLink) : "/" });
            }

            if (CartWithValidationIssues.Cart.GetFirstForm().Shipments.All(x => x.ShippingMethodId != _cartService.InStorePickupInfoModel.MethodId))
            {
                return RedirectToAction("SingleAddress", new { node = content.ContentLink });
            }

            return RedirectToAction(CartWithValidationIssues.Cart.GetFirstForm().Shipments.Count == 1 ? "ShippingInformation" : "MutipleAddresses", new { node = content.ContentLink });
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult Login(CheckoutMethodViewModel viewModel)
        {
            var result = _applicationSignInManager.PasswordSignInAsync(viewModel.LoginViewModel.Email, viewModel.LoginViewModel.Password, true, true).Result;
            switch (result)
            {
                case SignInStatus.Success:
                    break;
                default:
                    ModelState.AddModelError("Password", _localizationService.GetString("/Login/Form/Error/WrongPasswordOrEmail"));
                    return View("CheckoutMethod", viewModel);
            }

            return RedirectToAction("Index", "Checkout");
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult UpdateSingleShipmentAddress(CheckoutViewModel checkoutViewModel)
        {
            ModelState.Clear();
            var content = Request.RequestContext.GetRoutedData<CheckoutPage>();
            var viewModel = CreateCheckoutViewModel(content);
            if (checkoutViewModel.AddressType == 0)
            {
                viewModel.Shipments[0].Address = checkoutViewModel.Shipments[0].Address;
            }
            else
            {
                _addressBookService.LoadAddress(checkoutViewModel.Shipments[0].Address);
                viewModel.Shipments[0].Address = checkoutViewModel.Shipments[0].Address;
            }

            _checkoutService.UpdateShippingAddresses(CartWithValidationIssues.Cart, viewModel);
            _orderRepository.Save(CartWithValidationIssues.Cart);

            return RedirectToAction("ShippingInformation", "Checkout");
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult UpdateMultipleShipmentAddresses(MultiAddressViewModel viewModel)
        {
            for (var i = 0; i < viewModel.CartItems.Length; i++)
            {
                if (string.IsNullOrEmpty(viewModel.CartItems[i].AddressId))
                {
                    ModelState.AddModelError($"CartItems[{i}].AddressId", _localizationService.GetString("/Checkout/MultiShipment/Empty/AddressId"));
                }
            }

            var content = Request.RequestContext.GetRoutedData<CheckoutPage>();
            if (!ModelState.IsValid)
            {
                return View("MultipleAddresses", _multiShipmentViewModelFactory.CreateMultiShipmentViewModel(CartWithValidationIssues.Cart, content, User.Identity.IsAuthenticated));
            }

            _cartService.RecreateLineItemsBasedOnShipments(CartWithValidationIssues.Cart, viewModel.CartItems, GetAddresses(viewModel));

            _orderRepository.Save(CartWithValidationIssues.Cart);

            return RedirectToAction("ShippingInformation", "Checkout");
        }

        [HttpPost]
        [AllowDBWrite]
        public async Task<ActionResult> PlaceOrder(CheckoutPage currentPage, CheckoutViewModel checkoutViewModel)
        {
            var purchaseOrder = _checkoutService.PlaceOrder(CartWithValidationIssues.Cart, ModelState, checkoutViewModel);
            if (purchaseOrder == null)
            {
                var viewModel = CreateCheckoutViewModel(currentPage);
                viewModel.OrderSummary = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
                return View("PlaceOrder", viewModel);
            }
            if (Request.IsAuthenticated)
            {
                var contact = _customerContext.CurrentContact.CurrentContact;
                var organization = contact.ContactOrganization;
                if (organization != null)
                {
                    purchaseOrder.Properties[Constants.Customer.CustomerFullName] = contact.FullName;
                    purchaseOrder.Properties[Constants.Customer.CustomerEmailAddress] = contact.Email;
                    purchaseOrder.Properties[Constants.Customer.CurrentCustomerOrganization] = organization.Name;
                    _orderRepository.Save(purchaseOrder);
                }
            }
            checkoutViewModel.CurrentContent = currentPage;
            var confirmationSentSuccessfully = _checkoutService.SendConfirmation(checkoutViewModel, purchaseOrder);
            await _checkoutService.CreateOrUpdateBoughtProductsProfileStore(CartWithValidationIssues.Cart);
            await _checkoutService.CreateBoughtProductsSegments(CartWithValidationIssues.Cart);
            await _recommendationService.TrackOrder(HttpContext, purchaseOrder);

            return Redirect(_checkoutService.BuildRedirectionUrl(checkoutViewModel, purchaseOrder, confirmationSentSuccessfully));
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult UpdatePaymentOption(CheckoutPage currentPage, IPaymentMethod paymentOption)
        {
            ModelState.Clear();

            var viewModel = CreateCheckoutViewModel(currentPage, paymentOption);

            return PartialView("AddPayment", viewModel);
        }

        [HttpGet]
        public ActionResult AddNewAddress(CheckoutPage currentPage)
        {

            var viewModel = new NewAddessViewModel
            {
                Address = new AddressModel
                {
                    AddressId = Guid.NewGuid().ToString(),
                    Name = Guid.NewGuid().ToString(),
                    CountryCode = "USA"
                }
            };

            _addressBookService.LoadCountriesAndRegionsForAddress(viewModel.Address);
            return View("NewAddress", viewModel);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult AddNewAddress(NewAddessViewModel viewModel)
        {
            if (CartWithValidationIssues.Cart.GetFirstForm().Shipments.Count == 1 && CartWithValidationIssues.Cart.GetFirstShipment().ShippingAddress == null)
            {
                CartWithValidationIssues.Cart.GetFirstShipment().ShippingAddress = _addressBookService.ConvertToAddress(viewModel.Address, CartWithValidationIssues.Cart);
                _orderRepository.Save(CartWithValidationIssues.Cart);
                return RedirectToAction("MutipleAddresses", "Checkout");
            }

            var shipment = CartWithValidationIssues.Cart.CreateShipment(_orderGroupFactory);
            CartWithValidationIssues.Cart.GetFirstForm().Shipments.Add(shipment);
            shipment.ShippingAddress = _addressBookService.ConvertToAddress(viewModel.Address, CartWithValidationIssues.Cart);
            _orderRepository.Save(CartWithValidationIssues.Cart);
            return RedirectToAction("MutipleAddresses", "Checkout");
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult UpdateShippingMethods(CheckoutViewModel viewModel)
        {
            _checkoutService.UpdateShippingMethods(CartWithValidationIssues.Cart, viewModel.Shipments);
            _checkoutService.ApplyDiscounts(CartWithValidationIssues.Cart);
            _orderRepository.Save(CartWithValidationIssues.Cart);
            return RedirectToAction("BillingInformation", "Checkout");
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult UpdatePayment(CheckoutPage currentPage, CheckoutViewModel viewModel, IPaymentMethod paymentOption)
        {
            if (!paymentOption.ValidateData())
            {
                return View(viewModel);
            }

            viewModel.Payment = paymentOption;
            _checkoutService.CreateAndAddPaymentToCart(CartWithValidationIssues.Cart, viewModel);
            _orderRepository.Save(CartWithValidationIssues.Cart);

            var model = CreateCheckoutViewModel(currentPage);
            model.OrderSummary = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
            if (Request.IsAuthenticated)
            {
                model.AddressType = 1;
            }
            else
            {
                model.AddressType = 0;
            }
            return PartialView("BillingInformation", model);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult RemovePayment(CheckoutPage currentPage, CheckoutViewModel viewModel, IPaymentMethod paymentOption)
        {
            if (!paymentOption.ValidateData())
            {
                return View(viewModel);
            }

            viewModel.Payment = paymentOption;
            _checkoutService.RemovePaymentFromCart(CartWithValidationIssues.Cart, viewModel);
            _orderRepository.Save(CartWithValidationIssues.Cart);

            var model = CreateCheckoutViewModel(currentPage);
            model.OrderSummary = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(CartWithValidationIssues.Cart);
            if (Request.IsAuthenticated)
            {
                model.AddressType = 1;
            }
            else
            {
                model.AddressType = 0;
            }
            return PartialView("BillingInformation", model);
        }

        [HttpPost]
        [AllowDBWrite]
        public ActionResult UpdatePaymentAddress(CheckoutViewModel viewModel)
        {
            if (!CartWithValidationIssues.Cart.Forms.SelectMany(x => x.Payments).Any())
            {
                ModelState.AddModelError("SelectedPayment", _localizationService.GetString("/Shared/PaymentRequired"));
                return View("BillingInformation", viewModel);
            }
            if (viewModel.AddressType == 1)
            {
                _addressBookService.LoadAddress(viewModel.BillingAddress);
            }

            foreach (var payment in CartWithValidationIssues.Cart.GetFirstForm().Payments)
            {
                payment.BillingAddress = _addressBookService.ConvertToAddress(viewModel.BillingAddress, CartWithValidationIssues.Cart);
            }
            
            _orderRepository.Save(CartWithValidationIssues.Cart);
            return RedirectToAction("CreateSubscription", "Checkout");
        }

        public ActionResult LoadOrder(int orderLink)
        {
            bool success = false;
            var purchaseOrder = _orderRepository.Load<IPurchaseOrder>(orderLink);

            DateTime quoteExpireDate;
            DateTime.TryParse(purchaseOrder.Properties[Constants.Quote.QuoteExpireDate].ToString(), out quoteExpireDate);
            if (DateTime.Compare(DateTime.Now, quoteExpireDate) > 0) return Json(new { success });
            if (CartWithValidationIssues.Cart != null && CartWithValidationIssues.Cart.OrderLink != null)
                _orderRepository.Delete(CartWithValidationIssues.Cart.OrderLink);

            _cart = new CartWithValidationIssues
            {
                Cart = _cartServiceB2B.CreateNewCart(),
                ValidationIssues = new Dictionary<ILineItem, List<ValidationIssue>>()
            };
            var returnedCart = _cartServiceB2B.PlaceOrderToCart(purchaseOrder, _cart.Cart);

            returnedCart.Properties[Constants.Quote.ParentOrderGroupId] = purchaseOrder.OrderLink.OrderGroupId;
            _orderRepository.Save(returnedCart);

            
            var checkoutPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage).CheckoutPage;
            _cartService.ValidateCart(returnedCart);
            return Json(new { link = _urlResolver.GetUrl(checkoutPage)});
        }

        [HttpGet]
        public ActionResult CreateSubscription(CheckoutPage currentPage)
        {
            var viewModel = CreateCheckoutViewModel(currentPage);
            return View("Subscription", viewModel);
        }

        [HttpPost]
        public ActionResult AddSubscription(CheckoutViewModel checkoutViewModel)
        {
            _checkoutService.UpdatePaymentPlan(CartWithValidationIssues.Cart, checkoutViewModel);

            return RedirectToAction("PlaceOrder", "Checkout");
        }

        public ActionResult OnPurchaseException(ExceptionContext filterContext)
        {
            var currentPage = filterContext.RequestContext.GetRoutedData<CheckoutPage>();
            if (currentPage == null)
            {
                return new EmptyResult();
            }

            var viewModel = CreateCheckoutViewModel(currentPage);
            ModelState.AddModelError("Purchase", filterContext.Exception.Message);

            return View(viewModel.ViewName, viewModel);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            _controllerExceptionHandler.HandleRequestValidationException(filterContext, "purchase", OnPurchaseException);
        }

        private ViewResult View(CheckoutViewModel checkoutViewModel)
        {
            return View(checkoutViewModel.ViewName, CreateCheckoutViewModel(checkoutViewModel.CurrentContent, checkoutViewModel.Payments.FirstOrDefault()));
        }

        private CheckoutViewModel CreateCheckoutViewModel(CheckoutPage currentPage, IPaymentMethod paymentOption = null)
        {
            return _checkoutViewModelFactory.CreateCheckoutViewModel(CartWithValidationIssues.Cart, currentPage, paymentOption);
        }

        private CartWithValidationIssues CartWithValidationIssues => _cart ?? (_cart = _cartService.LoadCart(_cartService.DefaultCartName, true));

        private bool CartIsNullOrEmpty()
        {
            return CartWithValidationIssues.Cart == null || !CartWithValidationIssues.Cart.GetAllLineItems().Any();
        }

        private IList<AddressModel> GetAddresses(MultiAddressViewModel viewModel)
        {
            var addresses = new List<AddressModel>();
            var savedAddresses = _addressBookService.List();
            var orderAddresses = CartWithValidationIssues.Cart.GetFirstForm()
                .Shipments
                .Select(x => x.ShippingAddress)
                .Where(x => x != null);

            foreach (var addressId in viewModel.CartItems.Select(x => x.AddressId).Distinct())
            {
                var address = new AddressModel { AddressId = addressId };
                var savedAddress = savedAddresses.FirstOrDefault(x => x.AddressId.Equals(addressId));
                if (savedAddress != null)
                {
                    _addressBookService.LoadAddress(address);
                    addresses.Add(address);
                    continue;
                }

                var orderAddress = orderAddresses.FirstOrDefault(x => x.Id.Equals(addressId));
                if (orderAddress == null)
                {
                    continue;
                }
                addresses.Add(_addressBookService.ConvertToModel(orderAddress));
            }
            

            return addresses;
        }
    }
}
