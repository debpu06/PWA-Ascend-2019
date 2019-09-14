using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Login.Services;
using EPiServer.Reference.Commerce.Site.Features.OrderHistory.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Controllers;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Controllers
{
    [Authorize]
    public class ProfilePageController : IdentityControllerBase<ProfilePage>
    {
        private readonly CustomerContextFacade _customerContext;
        private readonly IAddressBookService _addressBookService;
        private readonly IOrderRepository _orderRepository;
        private readonly IContentLoader _contentLoader;
        private readonly ICartService _cartService;

        public ProfilePageController(CustomerContextFacade customerContextFacade,
            IAddressBookService addressBookService,
            IOrderRepository orderRepository,
            ApplicationSignInManager<SiteUser> signinManager,
            ApplicationUserManager<SiteUser> userManager,
            ICartService cartService,
            UserService userService, IContentLoader contentLoader) : base(signinManager, userManager, userService)
        {
            _customerContext = customerContextFacade;
            _addressBookService = addressBookService;
            _orderRepository = orderRepository;
            _contentLoader = contentLoader;
            _cartService = cartService;
        }

        public ActionResult Index(ProfilePage currentPage)
        {
            var viewModel = new ProfilePageViewModel(currentPage)
            {
                Orders = GetOrderHistoryViewModels(),
                Addresses = GetAddressViewModels(),
                SiteUser = UserService.GetUser(User.Identity.Name),
                CustomerContact = _customerContext.CurrentContact.CurrentContact
            };

            viewModel.OrderDetailsPageUrl =
           UrlResolver.Current.GetUrl(_contentLoader.Get<BaseStartPage>(ContentReference.StartPage).OrderDetailsPage);


            return View(viewModel);
        }

        private IList<AddressModel> GetAddressViewModels()
        {
            return _addressBookService.List();
        }

        private List<OrderViewModel> GetOrderHistoryViewModels()
        {
            var purchaseOrders = _orderRepository.Load<IPurchaseOrder>(_customerContext.CurrentContactId, _cartService.DefaultCartName)
                .OrderByDescending(x => x.Created).ToList();

            if (purchaseOrders.Count > 3)
            {
                purchaseOrders = purchaseOrders.Take(3).ToList();
            }
                

            var viewModel = new List<OrderViewModel>();

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

                foreach (var orderAddress in purchaseOrder.Forms.SelectMany(x => x.Shipments).Select(s => s.ShippingAddress))
                {
                    var shippingAddress = _addressBookService.ConvertToModel(orderAddress);
                    orderViewModel.ShippingAddresses.Add(shippingAddress);
                }

                viewModel.Add(orderViewModel);
            }

            return viewModel;
        }
    }
}