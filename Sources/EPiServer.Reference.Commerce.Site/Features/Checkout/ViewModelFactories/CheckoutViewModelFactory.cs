using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Payment.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Payment.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Reference.Commerce.B2B;
using EPiServer.Reference.Commerce.Site.Features.Payment.PaymentMethods;
using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModelFactories
{
    [ServiceConfiguration(typeof(CheckoutViewModelFactory), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CheckoutViewModelFactory
    {
        readonly LocalizationService _localizationService;
        readonly ServiceAccessor<PaymentMethodViewModelFactory> _paymentMethodViewModelFactory;
        readonly IAddressBookService _addressBookService;
        readonly IContentLoader _contentLoader;
        readonly UrlResolver _urlResolver;
        readonly ServiceAccessor<HttpContextBase> _httpContextAccessor;
        readonly ShipmentViewModelFactory _shipmentViewModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IOrganizationService _organizationService;
        private readonly IBudgetService _budgetService;
        private readonly CustomerContextFacade _customerContext;

        public CheckoutViewModelFactory(
            LocalizationService localizationService,
            ServiceAccessor<PaymentMethodViewModelFactory> paymentMethodViewModelFactory,
            IAddressBookService addressBookService,
            IContentLoader contentLoader,
            UrlResolver urlResolver,
            ServiceAccessor<HttpContextBase> httpContextAccessor,
            ShipmentViewModelFactory shipmentViewModelFactory,
            ICustomerService customerService,
            IOrganizationService organizationService,
            IBudgetService budgetService,
            CustomerContextFacade customerContext)
        {
            _localizationService = localizationService;
            _paymentMethodViewModelFactory = paymentMethodViewModelFactory;
            _addressBookService = addressBookService;
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _httpContextAccessor = httpContextAccessor;
            _shipmentViewModelFactory = shipmentViewModelFactory;
            _customerService = customerService;
            _organizationService = organizationService;
            _budgetService = budgetService;
            _customerContext = customerContext;
        }

        public virtual CheckoutViewModel CreateCheckoutViewModel(ICart cart, CheckoutPage currentPage, IPaymentMethod paymentOption = null)
        {
            if (cart == null)
            {
                return CreateEmptyCheckoutViewModel(currentPage);
            }

            var currentShippingAddressId = cart.GetFirstShipment()?.ShippingAddress?.Id;
            var currentBillingAdressId = cart.GetFirstForm().Payments.FirstOrDefault()?.BillingAddress?.Id;

            var shipments = _shipmentViewModelFactory.CreateShipmentsViewModel(cart).ToList();
            var useBillingAddressForShipment = shipments.Count == 1 && currentBillingAdressId == currentShippingAddressId && _addressBookService.UseBillingAddressForShipment();

            var viewModel = new CheckoutViewModel(currentPage)
            {
                Shipments = shipments,
                BillingAddress = CreateBillingAddressModel(),
                UseBillingAddressForShipment = useBillingAddressForShipment,
                AppliedCouponCodes = cart.GetFirstForm().CouponCodes.Distinct(),
                AvailableAddresses = new List<AddressModel>(),
                ReferrerUrl = GetReferrerUrl(),
                Currency = cart.Currency,
                CurrentCustomer = _customerService.GetCurrentContact(),
                IsOnHoldBudget = CheckForOnHoldBudgets(),
                Payment = paymentOption,
                PaymentPlanSetting = new PaymentPlanSetting()
                {
                    CycleMode = Mediachase.Commerce.Orders.PaymentPlanCycle.None,
                    IsActive = false,
                    StartDate = DateTime.UtcNow
                },
            };

            UpdatePayments(viewModel, cart);

            var availableAddresses = GetAvailableAddresses();

            if (availableAddresses.Any())
            {
                viewModel.AvailableAddresses.Add(new AddressModel { Name = _localizationService.GetString("/Checkout/MultiShipment/SelectAddress") });
                
                foreach (var address in availableAddresses)
                {
                    viewModel.AvailableAddresses.Add(address);
                }
            }
            else
            {
                viewModel.AvailableAddresses.Add(new AddressModel { Name = _localizationService.GetString("/Checkout/MultiShipment/NoAddressFound") });
            }

            SetDefaultShipmentAddress(viewModel, currentShippingAddressId);

            _addressBookService.LoadAddress(viewModel.BillingAddress);
            PopulateCountryAndRegions(viewModel);

            return viewModel;
        }

        private void SetDefaultShipmentAddress(CheckoutViewModel viewModel, string shippingAddressId)
        {
            if (viewModel.AddressType == 1 && viewModel.Shipments.Count == 1)
            {
                viewModel.Shipments[0].Address = viewModel.AvailableAddresses.SingleOrDefault(x => x.AddressId == shippingAddressId) ??
                                                 viewModel.AvailableAddresses.SingleOrDefault(x => x.ShippingDefault) ?? 
                                                 viewModel.BillingAddress;
            }
        }

        private IList<AddressModel> GetAvailableAddresses()
        {
            var addresses = _addressBookService.List();
            foreach (var address in addresses.Where(x => string.IsNullOrEmpty(x.Name)))
            {
                address.Name = _localizationService.GetString("/Shared/Address/DefaultAddressName");
            }

            return addresses;
        }

        private CheckoutViewModel CreateEmptyCheckoutViewModel(CheckoutPage currentPage)
        {
            return new CheckoutViewModel(currentPage)
            {
                Shipments = new List<ShipmentViewModel>(),
                AppliedCouponCodes = new List<string>(),
                AvailableAddresses = new List<AddressModel>(),
                PaymentMethodViewModels = Enumerable.Empty<PaymentMethodViewModel>(),
                UseBillingAddressForShipment = true
            };
        }

        private void PopulateCountryAndRegions(CheckoutViewModel viewModel)
        {
            foreach (var shipment in viewModel.Shipments)
            {
                _addressBookService.LoadCountriesAndRegionsForAddress(shipment.Address);
            }
        }

        private void UpdatePayments(CheckoutViewModel viewModel, ICart cart)
        {
            viewModel.PaymentMethodViewModels = _paymentMethodViewModelFactory().GetPaymentMethodViewModels();
            var methodViewModels = viewModel.PaymentMethodViewModels.ToList();
            if (!methodViewModels.Any())
            {
                return;
            }

            var defaultPaymentMethod = methodViewModels.FirstOrDefault(p => p.IsDefault) ?? methodViewModels.First();
            var selectedPaymentMethod = viewModel.Payment == null ?
                defaultPaymentMethod :
                methodViewModels.Single(p => p.SystemKeyword == viewModel.Payment.SystemKeyword);

            viewModel.Payment = selectedPaymentMethod.PaymentOption;
            viewModel.Payments = methodViewModels.Where(x => cart.GetFirstForm().Payments.Any(p => p.PaymentMethodId == x.PaymentMethodId))
                .Select(x => x.PaymentOption)
                .OfType<PaymentOptionBase>()
                .ToList();

            foreach (var viewModelPayment in viewModel.Payments)
            {
                viewModelPayment.Amount =
                    new Money(
                        cart.GetFirstForm().Payments
                            .FirstOrDefault(p => p.PaymentMethodId == viewModelPayment.PaymentMethodId)?.Amount ?? 0,
                        cart.Currency);
            }
            if (!cart.GetFirstForm().
                Payments.Any())
            {
                return;
            }
            var method = methodViewModels.FirstOrDefault(
                x => x.PaymentMethodId == cart.GetFirstForm().
                         Payments.FirstOrDefault().
                         PaymentMethodId);
            if (method == null)
            {
                return;
            }
            viewModel.SelectedPayment = method.Description;
            var payment = cart.GetFirstForm().
                Payments.FirstOrDefault();
            var creditCardPayment = payment as ICreditCardPayment;
            if (creditCardPayment != null)
            {
                viewModel.SelectedPayment +=
                    $" - ({creditCardPayment.CreditCardNumber.Substring(creditCardPayment.CreditCardNumber.Length - 4)})";
            }
        }

        private AddressModel CreateBillingAddressModel()
        {
            var preferredBillingAddress = _addressBookService.GetPreferredBillingAddress();

            return new AddressModel
            {
                AddressId = preferredBillingAddress?.Name,
                Name = preferredBillingAddress != null ? preferredBillingAddress.Name : Guid.NewGuid().ToString(),
            };
        }

        private string GetReferrerUrl()
        {
            var httpContext = _httpContextAccessor();
            if (httpContext.Request.UrlReferrer != null &&
                httpContext.Request.UrlReferrer.Host.Equals(httpContext.Request.Url.Host, StringComparison.OrdinalIgnoreCase))
            {
                return httpContext.Request.UrlReferrer.ToString();
            }
            return _urlResolver.GetUrl(ContentReference.StartPage);
        }

        private bool CheckForOnHoldBudgets()
        {
            var currentCustomer = _customerContext.GetContactById(_customerContext.CurrentContactId);
            if (currentCustomer?.ContactOrganization != null)
            {
                var subOrganizationId = new Guid(currentCustomer.ContactOrganization.PrimaryKeyId.Value.ToString());

                var purchaserBudget = _budgetService.GetCustomerCurrentBudget(subOrganizationId, _customerContext.CurrentContactId);
                if (purchaserBudget != null)
                    if (purchaserBudget.Status.Equals(Constants.BudgetStatus.OnHold)) return true;

                var suborganizationCurrentBudget = _budgetService.GetCurrentOrganizationBudget(subOrganizationId);
                if (suborganizationCurrentBudget != null)
                    if (suborganizationCurrentBudget.Status.Equals(Constants.BudgetStatus.OnHold)) return true;

                var organizationCurrentBudget = _budgetService.GetCurrentOrganizationBudget(_organizationService.GetSubOrganizationById(subOrganizationId.ToString()).ParentOrganizationId);
                if (organizationCurrentBudget != null)
                    if (organizationCurrentBudget.Status.Equals(Constants.BudgetStatus.OnHold)) return true;
            }

            return false;
        }
    }
}