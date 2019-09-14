using EPiServer.Commerce.Order;
using EPiServer.Logging;
using EPiServer.Reference.Commerce.B2B;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.OrderDetails.Pages;
using EPiServer.Reference.Commerce.Site.Features.OrderDetails.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Cart.Extensions;
using EPiServer.Web.Mvc.Html;

namespace EPiServer.Reference.Commerce.Site.Features.OrderDetails.Controllers
{
    public class OrderDetailsController : PageController<OrderDetailsPage>
    {
        private readonly IAddressBookService _addressBookService;
        private readonly IOrdersService _ordersService;
        private readonly ICustomerService _customerService;
        private readonly IOrderRepository _orderRepository;
        private readonly IContentLoader _contentLoader;
        private readonly ICartService _cartService;
        private readonly IPurchaseOrderFactory _purchaseOrderFactory;

        public OrderDetailsController(IAddressBookService addressBookService, IOrdersService ordersService, ICustomerService customerService, IOrderRepository orderRepository, IContentLoader contentLoader, ICartService cartService, IPurchaseOrderFactory purchaseOrderFactory)
        {
            _addressBookService = addressBookService;
            _ordersService = ordersService;
            _customerService = customerService;
            _orderRepository = orderRepository;
            _contentLoader = contentLoader;
            _cartService = cartService;
            _purchaseOrderFactory = purchaseOrderFactory;
        }

        [HttpGet]
        public ActionResult Index(OrderDetailsPage currentPage, int orderGroupId = 0)
        {
            return View(GetModel(orderGroupId, currentPage));
        }

        [HttpPost]
        public ActionResult ApproveOrder(int orderGroupId = 0)
        {
            if (orderGroupId == 0)
                return Json(new { result = true});
            var success =_ordersService.ApproveOrder(orderGroupId);

            return success ? Json(new { result = true }) : Json(new {result = "Failed to process your payment."});
        }

        [HttpPost]
        public ActionResult Reorder(int orderGroupId = 0)
        {
            var purchaseOrder = OrderContext.Current.Get<PurchaseOrder>(orderGroupId);
            var form = purchaseOrder.GetFirstForm();
            var list = form.GetAllLineItems();

            string warningMessage = string.Empty;

            ICart Cart = _cartService.LoadOrCreateCart(_cartService.DefaultCartName);

            foreach (var item in list)
            {
                Cart.AddLineItem(item);

            }

            var order = _orderRepository.Save(Cart);
            return Redirect(Url.ContentUrl(_contentLoader.Get<BaseStartPage>(ContentReference.StartPage).CheckoutPage));
        }

        [HttpPost]
        public ActionResult CreateReturn(int orderGroupId, int shipmentId, int lineItemId, decimal returnQuantity, string reason)
        {
            ReturnFormStatus formStatus = _ordersService.CreateReturn(orderGroupId, shipmentId, lineItemId, returnQuantity, reason);
            return Json(new {
                result = true,
                ReturnFormStatus = formStatus.ToString()
            });
        }

        [HttpPost]
        public ActionResult ChangePrice(int orderGroupId, int shipmentId, int lineItemId, decimal placedPrice, OrderDetailsPage currentPage)
        {
            var issues = _ordersService.ChangeLineItemPrice(orderGroupId, shipmentId, lineItemId, placedPrice);
            var model = GetModel(orderGroupId, currentPage);
            model.ErrorMessage = GetValidationMessages(issues);
            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult ChangeQuantity(int orderGroupId, int shipmentId, int lineItemId, decimal quantity, OrderDetailsPage currentPage)
        {
            var issues = _ordersService.ChangeLineItemQuantity(orderGroupId, shipmentId, lineItemId, quantity);
            var model = GetModel(orderGroupId, currentPage);
            model.ErrorMessage = GetValidationMessages(issues);
            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult AddNote(int orderGroupId, string note, OrderDetailsPage currentPage)
        {
            var order = _orderRepository.Load<IPurchaseOrder>(orderGroupId);
            _ordersService.AddNote(order, "Customer Manual Note", note);
            _orderRepository.Save(order);
            var model = GetModel(orderGroupId, currentPage);
            return PartialView("Index", model);
        }

        private static string GetValidationMessages(Dictionary<ILineItem, List<ValidationIssue>> validationIssues)
        {
            var messages = new List<string>();
            foreach (var validationIssue in validationIssues)
            {
                var warning = new StringBuilder();
                warning.Append($"Line Item with code {validationIssue.Key.Code} ");
                validationIssue.Value.Aggregate(warning, (current, issue) => current.Append(issue).Append(", "));
                messages.Add(warning.ToString().TrimEnd(',', ' '));
            }

            return string.Join(".", messages);
        }

        private OrderDetailsViewModel GetModel(int orderGroupId,  OrderDetailsPage currentPage)
        {
            var orderViewModel = new OrderDetailsViewModel
            {
                CurrentContent = currentPage,
                CurrentCustomer = _customerService.GetCurrentContact()
            };

            var purchaseOrder = OrderContext.Current.Get<PurchaseOrder>(orderGroupId);
            if (purchaseOrder == null)
            {
                return orderViewModel;
            }

            // Assume there is only one form per purchase.
            var form = purchaseOrder.GetFirstForm();

            var billingAddress = form.Payments.FirstOrDefault() != null
                ? form.Payments.First().BillingAddress
                : new OrderAddress();

            orderViewModel.PurchaseOrder = purchaseOrder;

            orderViewModel.Items = form.Shipments.SelectMany(shipment => shipment.LineItems.Select(lineitem => new OrderDetailsItemViewModel
            {
                LineItem = lineitem,
                Shipment = shipment,
                PurchaseOrder = orderViewModel.PurchaseOrder as PurchaseOrder,
            }
            ));

            orderViewModel.BillingAddress = _addressBookService.ConvertToModel(billingAddress);
            orderViewModel.ShippingAddresses = new List<AddressModel>();

            foreach (var orderAddress in form.Shipments.Select(s => s.ShippingAddress))
            {
                var shippingAddress = _addressBookService.ConvertToModel(orderAddress);
                orderViewModel.ShippingAddresses.Add(shippingAddress);
                orderViewModel.OrderGroupId = purchaseOrder.OrderGroupId;
            }
            if (purchaseOrder[Constants.Quote.QuoteExpireDate] != null &&
                !string.IsNullOrEmpty(purchaseOrder[Constants.Quote.QuoteExpireDate].ToString()))
            {
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


            if (!string.IsNullOrEmpty(purchaseOrder["QuoteStatus"]?.ToString()) &&
                (purchaseOrder.Status == OrderStatus.InProgress.ToString() ||
                 purchaseOrder.Status == OrderStatus.OnHold.ToString()))
            {
                orderViewModel.QuoteStatus = purchaseOrder["QuoteStatus"].ToString();
            }

            orderViewModel.BudgetPayment = _ordersService.GetOrderBudgetPayment(purchaseOrder);
            return orderViewModel;
        }
    }
}