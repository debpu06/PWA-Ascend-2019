using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Payment.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using System;
using System.Collections;
using EPiServer.Reference.Commerce.Site.Features.Payment.PaymentMethods;
using EPiServer.Reference.Commerce.Shared.Attributes;
using EPiServer.ServiceLocation;
using EPiServer.Framework.Localization;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels
{
    [Bind(Exclude = "Payment")]
    public class CheckoutViewModel : ContentViewModel<CheckoutPage>
    {
        private Injected<LocalizationService> _localizationService;
        public const string MultiShipmentCheckoutViewName = "MultiShipmentCheckout";

        public const string SingleShipmentCheckoutViewName = "SingleShipmentCheckout";

        public CheckoutViewModel()
        {
            Payments = new List<PaymentOptionBase>();
        }

        public CheckoutViewModel(CheckoutPage checkoutPage) : base(checkoutPage)
        {
            Payments = new List<PaymentOptionBase>();
        }
        

        /// <summary>
        /// Gets or sets a collection of all coupon codes that have been applied.
        /// </summary>
        public IEnumerable<string> AppliedCouponCodes { get; set; }

        /// <summary>
        /// Gets or sets all available payment methods that the customer can choose from.
        /// </summary>
        public IEnumerable<PaymentMethodViewModel> PaymentMethodViewModels { get; set; }

        public string ReferrerUrl { get; set; }

        /// <summary>
        /// Gets or sets all existing shipments related to the current order.
        /// </summary>
        public IList<ShipmentViewModel> Shipments { get; set; }

        /// <summary>
        /// Gets or sets a list of all existing addresses for the current customer and that can be used for billing and shipment.
        /// </summary>
        public IList<AddressModel> AvailableAddresses { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public AddressModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the payment method associated to the current purchase.
        /// </summary>
        public IList<PaymentOptionBase> Payments { get; set; }

        public IPaymentMethod Payment { get; set; }

        /// <summary>
        /// Gets or sets whether the shipping address should be the same as the billing address.
        /// </summary>
        public bool UseBillingAddressForShipment { get; set; }

        /// <summary>
        /// Gets or sets the view message.
        /// </summary>
        public string Message { get; set; }

        public int AddressType { get; set; }

        public Currency Currency { get; set; }

        public string SelectedPayment { get; set; }

        public OrderSummaryViewModel OrderSummary { get; set; }

        /// <summary>
        /// Gets the name of the checkout view required depending on the number of distinct shipping addresses.
        /// </summary>
        public string ViewName => Shipments.Count > 1 ? MultiShipmentCheckoutViewName : SingleShipmentCheckoutViewName;

        public ContactViewModel CurrentCustomer { get; set; }
        public string QuoteStatus { get; set; } = "";
        public bool IsOnHoldBudget { get; set; }

        [LocalizedDisplay("/Shared/CreateSubscription")]
        public bool IsUsePaymentPlan { get; set; }

        public PaymentPlanSetting PaymentPlanSetting { get; set; }

        public List<SelectListItem> SubscriptionOption
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem {Text = "3 months", Value = "3"},
                    new SelectListItem {Text = "6 months", Value = "6"},
                    new SelectListItem {Text = "12 months", Value = "12"}
                };
            }
        }

        public List<SelectListItem> Modes => new List<SelectListItem>
        {
            new SelectListItem { Text = _localizationService.Service.GetString("/Shared/None"), Value="0"},
            new SelectListItem { Text = _localizationService.Service.GetString("/Shared/Days"), Value="1"},
            new SelectListItem { Text = _localizationService.Service.GetString("/Shared/Weeks"), Value="2"},
            new SelectListItem { Text = _localizationService.Service.GetString("/Shared/Months"), Value="3"},
            new SelectListItem { Text = _localizationService.Service.GetString("/Shared/Years"), Value="4"}
        };
    }


    public class PaymentPlanSetting
    {
        [LocalizedDisplay("/Shared/CycleMode")]
        public PaymentPlanCycle CycleMode
        {
            get; set;
        }

        [LocalizedDisplay("/Shared/CycleLength")]
        public int CycleLength { get; set; }

        public int MaxCyclesCount { get; set; }
        public int CompletedCyclesCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LastTransactionDate { get; set; }
        public bool IsActive { get; set; }
    }

}