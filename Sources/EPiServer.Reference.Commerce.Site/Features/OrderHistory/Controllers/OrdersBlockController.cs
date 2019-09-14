using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.OrderHistory.Pages;
using EPiServer.Reference.Commerce.Site.Features.OrderHistory.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.B2B;

namespace EPiServer.Reference.Commerce.Site.Features.OrderHistory.Controllers
{
    [Authorize]
    public class OrdersBlockController : BlockController<OrderHistoryBlock>
    {
        private readonly CustomerContextFacade _customerContext;
        private readonly IAddressBookService _addressBookService;
        private readonly IOrderRepository _orderRepository;
        private readonly IContentLoader _contentLoader;
        private readonly ICustomerService _customerService;

        public OrdersBlockController(CustomerContextFacade customerContextFacade, IAddressBookService addressBookService, IOrderRepository orderRepository, IContentLoader contentLoader, ICustomerService customerService)
        {
            _customerContext = customerContextFacade;
            _addressBookService = addressBookService;
            _orderRepository = orderRepository;
            _contentLoader = contentLoader;
            _customerService = customerService;
        }

        public override ActionResult Index(OrderHistoryBlock currentBlock)
        {
            var purchaseOrders = OrderContext.Current.LoadByCustomerId<PurchaseOrder>(_customerContext.CurrentContactId)
                                             .OrderByDescending(x => x.Created)
                                             .ToList();

            var viewModel = new OrderHistoryViewModel
            {

                Orders = new List<OrderViewModel>(),
                CurrentCustomer = _customerService.GetCurrentContact()
            };

            foreach (var purchaseOrder in purchaseOrders)
            {
                //Assume there is only one form per purchase.
                   var form = purchaseOrder.GetFirstForm();

                var billingAddress = form.Payments.FirstOrDefault() != null ? form.Payments.First().BillingAddress : new OrderAddress();
                var orderViewModel = new OrderViewModel
                {
                    PurchaseOrder = purchaseOrder,
                    Items = form.GetAllLineItems().Select(lineItem => new OrderHistoryItemViewModel
                    {
                        LineItem = lineItem,
                    }).GroupBy(x => x.LineItem.Code).Select(group => group.First()),
                    BillingAddress = _addressBookService.ConvertToModel(billingAddress),
                    ShippingAddresses = new List<AddressModel>()
                };

                foreach (var orderAddress in form.Shipments.Select(s => s.ShippingAddress))
                {
                    var shippingAddress = _addressBookService.ConvertToModel(orderAddress);
                    orderViewModel.ShippingAddresses.Add(shippingAddress);
                    orderViewModel.OrderGroupId = purchaseOrder.OrderGroupId;
                }

                if (!string.IsNullOrEmpty(purchaseOrder[Constants.Quote.QuoteStatus]?.ToString()) &&
                    (purchaseOrder.Status == OrderStatus.InProgress.ToString() || purchaseOrder.Status == OrderStatus.OnHold.ToString()))
                {
                    orderViewModel.QuoteStatus = purchaseOrder[Constants.Quote.QuoteStatus].ToString();
                    DateTime quoteExpireDate;
                    DateTime.TryParse(purchaseOrder[Constants.Quote.QuoteExpireDate].ToString(), out quoteExpireDate);
                    if (DateTime.Compare(DateTime.Now, quoteExpireDate) > 0)
                    {
                        orderViewModel.QuoteStatus = Constants.Quote.QuoteExpired;
                        try
                        {
                            // Update order quote status to expired
                            purchaseOrder[Constants.Quote.QuoteStatus] = Constants.Quote.QuoteExpired;
                            _orderRepository.Save(purchaseOrder);
                        }
                        catch (Exception ex)
                        {
                            LogManager.GetLogger(GetType()).Error("Failed to update order status to Quote Expired.", ex.StackTrace);
                        }

                    }
                }

                viewModel.Orders.Add(orderViewModel);
            }

            viewModel.OrderDetailsPageUrl =
                UrlResolver.Current.GetUrl(_contentLoader.Get<BaseStartPage>(ContentReference.StartPage).OrderDetailsPage);

            return PartialView("~/Views/OrdersBlock/Index.cshtml",viewModel);
        }
    }
}