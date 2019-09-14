using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Shared;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using Mediachase.Commerce.Orders;
using EPiServer.Web.Routing;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models;
using Mediachase.Commerce.Customers;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Controllers
{
    public abstract class OrderConfirmationControllerBase<T> : PageController<T> where T : SitePageData
    {
        protected readonly ConfirmationService _confirmationService;
        private readonly AddressBookService _addressBookService;
        protected readonly CustomerContextFacade _customerContext;
        private readonly IOrderGroupCalculator _orderGroupCalculator;
        private readonly UrlResolver _urlResolver;

        protected OrderConfirmationControllerBase(
            ConfirmationService confirmationService,
            AddressBookService addressBookService,
            CustomerContextFacade customerContextFacade,
            IOrderGroupCalculator orderGroupTotalsCalculator,
            UrlResolver urlResolver)
        {
            _confirmationService = confirmationService;
            _addressBookService = addressBookService;
            _customerContext = customerContextFacade;
            _orderGroupCalculator = orderGroupTotalsCalculator;
            _urlResolver = urlResolver;
        }

        protected OrderConfirmationViewModel<T> CreateViewModel(T currentPage, IPurchaseOrder order)
        {
            var hasOrder = order != null;

            if (!hasOrder)
            {
                return new OrderConfirmationViewModel<T>(currentPage);
            }

            var lineItems = order.GetFirstForm().Shipments.SelectMany(x => x.LineItems);
            var totals = _orderGroupCalculator.GetOrderGroupTotals(order);

            var viewModel = new OrderConfirmationViewModel<T>(currentPage)
            {
                Currency = order.Currency,
                CurrentContent = currentPage,
                HasOrder = hasOrder,
                OrderId = order.OrderNumber,
                Created = order.Created,
                Items = lineItems,
                BillingAddress = new AddressModel(),
                ShippingAddresses = new List<AddressModel>(),
                ContactId = _customerContext.CurrentContactId,
                Payments = order.GetFirstForm().Payments.Where(c => c.TransactionType == TransactionType.Authorization.ToString() || c.TransactionType == TransactionType.Sale.ToString()),
                OrderGroupId = order.OrderLink.OrderGroupId,
                OrderLevelDiscountTotal = order.GetOrderDiscountTotal(),
                ShippingSubTotal = order.GetShippingSubTotal(),
                ShippingDiscountTotal = order.GetShippingDiscountTotal(),
                ShippingTotal = totals.ShippingTotal,
                HandlingTotal = totals.HandlingTotal,
                TaxTotal = totals.TaxTotal,
                CartTotal = totals.Total,
                SubTotal = order.GetSubTotal(),
                FileUrls = new List<Dictionary<string, string>>(),
                Keys = new List<Dictionary<string, string>>()
            };

            foreach (var lineItem in lineItems)
            {
                if (lineItem.GetEntryContentBase() is FileVariant)
                {
                    var url = _urlResolver.GetUrl(((FileVariant)lineItem.GetEntryContentBase()).File);
                    viewModel.FileUrls.Add(new Dictionary<string, string>() { { lineItem.DisplayName, url } });
                }
                if (lineItem.GetEntryContentBase() is KeyVariant)
                {
                    var key = ((KeyVariant)lineItem.GetEntryContentBase()).Key;
                    viewModel.Keys.Add(new Dictionary<string, string>() { { lineItem.DisplayName, key } });
                }
                if (lineItem.GetEntryContentBase() is ElevatedRoleVariant)
                {
                    var role = ((ElevatedRoleVariant)lineItem.GetEntryContentBase()).Role;
                    viewModel.ElevatedRole = role;
                    var currentContact = CustomerContext.Current.CurrentContact;
                    if (currentContact != null)
                    {
                        var contact = new ElevatedRoleContact(currentContact);
                        contact.ElevatedRole = ElevatedRoles.Reader.ToString();
                        contact.SaveChanges();
                    }
                }
            }

            var billingAddress = order.GetFirstForm().Payments.First().BillingAddress;

            // Map the billing address using the billing id of the order form.
            _addressBookService.MapToModel(billingAddress, viewModel.BillingAddress);

            // Map the remaining addresses as shipping addresses.
            foreach (var orderAddress in order.Forms.SelectMany(x => x.Shipments).Select(s => s.ShippingAddress))
            {
                var shippingAddress = new AddressModel();
                _addressBookService.MapToModel(orderAddress, shippingAddress);
                viewModel.ShippingAddresses.Add(shippingAddress);
            }

            return viewModel;
        }
    }
}
