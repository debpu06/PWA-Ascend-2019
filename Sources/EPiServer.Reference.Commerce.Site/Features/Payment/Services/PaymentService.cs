using EPiServer.Reference.Commerce.B2B;
using EPiServer.Reference.Commerce.B2B.Enums;
using EPiServer.Reference.Commerce.B2B.Extensions;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Payment.ViewModels;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Orders.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Payment.Services
{
    [ServiceConfiguration(typeof(IPaymentService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class PaymentService : IPaymentService
    {
        private readonly ICustomerService _customerService;
        private readonly ICartService _cartService;
        private readonly HttpContextBase _httpContext;

        public PaymentService(ICustomerService customerService,
            ICartService cartService,
            HttpContextBase httpContext)
        {
            _customerService = customerService;
            _cartService = cartService;
            _httpContext = httpContext;
        }

        public IEnumerable<PaymentMethodViewModel> GetPaymentMethodsByMarketIdAndLanguageCode(string marketId, string languageCode)
        {
            var methods = PaymentManager.GetPaymentMethodsByMarket(marketId)
                .PaymentMethod
                .Where(x => x.IsActive && languageCode.Equals(x.LanguageId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Ordering)
                .Select(x => new PaymentMethodViewModel
                {
                    PaymentMethodId = x.PaymentMethodId,
                    SystemKeyword = x.SystemKeyword,
                    FriendlyName = x.Name,
                    Description = x.Description,
                    IsDefault = x.IsDefault
                });

            if (_httpContext == null || !_httpContext.Request.IsAuthenticated)
            {
                return methods;
            }

            var currentContact = _customerService.GetCurrentContact();
            if (string.IsNullOrEmpty(currentContact.UserRole))
            {
                return methods;
            }
            var cart = _cartService.LoadCart(_cartService.DefaultCartName, true)?.Cart;
            if (cart != null && cart.IsQuoteCart() && currentContact.Role == B2BUserRoles.Approver)
            {
                return methods.Where(payment => payment.SystemKeyword.Equals(Constants.Order.BudgetPayment));
            }
            return currentContact.Role == B2BUserRoles.Purchaser ? methods : methods.Where(payment => !payment.SystemKeyword.Equals(Constants.Order.BudgetPayment));
        }
    }
}