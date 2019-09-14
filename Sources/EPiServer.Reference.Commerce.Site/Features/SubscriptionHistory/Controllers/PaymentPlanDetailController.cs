using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.OrderHistory.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.Pages;
using EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.ViewModels;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Orders;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.Controllers
{
    public class PaymentPlanDetailController : PageController<PaymentPlanDetailPage>
    {

        private readonly AddressBookService _addressBookService;
        private readonly IOrderRepository _orderRepository;
        private readonly IContentLoader _contentLoader;
        private readonly ICartService _cartService;
        private readonly CustomerContextFacade _customerContext;

        public PaymentPlanDetailController(AddressBookService addressBookService,
            IOrderRepository orderRepository,
            IContentLoader contentLoader,
            CustomerContextFacade customerContext,
            ICartService cartService
            )
        {
            _addressBookService = addressBookService;
            _orderRepository = orderRepository;
            _contentLoader = contentLoader;
            _customerContext = customerContext;
            _cartService = cartService;
        }

        public ActionResult Index(PaymentPlanDetailPage currentPage, int paymentPlanId = 0)
        {
            /* Implementation of action. You can create your own view model class that you pass to the view or
             * you can pass the page type for simpler templates */

            var paymentDetail = OrderContext.Current.Get<PaymentPlan>(paymentPlanId);

            var viewModel = new PaymentPlanDetailViewModel()
            {
                CurrentContent = currentPage,
                PaymentPlan = paymentDetail
            };


            //Get order that created by 
            var purchaseOrders = OrderContext.Current.LoadByCustomerId<PurchaseOrder>(_customerContext.CurrentContactId)
                                 .OrderByDescending(x => x.Created)
                                 .Where(x => x.ParentOrderGroupId.Equals(paymentPlanId))
                                 .ToList();

            var orders = new OrderHistoryViewModel
            {
                Orders = new List<OrderViewModel>()
            };

            foreach (var purchaseOrder in purchaseOrders)
            {

                // Assume there is only one form per purchase.
                var form = purchaseOrder.GetFirstForm();
                var billingAddress = new AddressModel();
                var payment = form.Payments.FirstOrDefault();
                if (payment != null)
                {
                    billingAddress = _addressBookService.ConvertToModel(payment.BillingAddress);
                }
                var orderViewModel = new OrderViewModel
                {
                    PurchaseOrder = purchaseOrder,
                    Items = form.GetAllLineItems().Select(lineItem => new OrderHistoryItemViewModel
                    {
                        LineItem = lineItem,
                    }).GroupBy(x => x.LineItem.Code).Select(group => group.First()),
                    BillingAddress = billingAddress,
                    ShippingAddresses = new List<AddressModel>()
                };

                foreach (var orderAddress in purchaseOrder.OrderForms.Cast<IOrderForm>().SelectMany(x => x.Shipments).Select(s => s.ShippingAddress))
                {
                    var shippingAddress = _addressBookService.ConvertToModel(orderAddress);
                    orderViewModel.ShippingAddresses.Add(shippingAddress);
                }

                orders.Orders.Add(orderViewModel);
            }
            orders.OrderDetailsPageUrl =
             UrlResolver.Current.GetUrl(_contentLoader.Get<BaseStartPage>(ContentReference.StartPage).OrderDetailsPage);

            viewModel.Orders = orders;

            return View(viewModel);
        }

        
    }
}