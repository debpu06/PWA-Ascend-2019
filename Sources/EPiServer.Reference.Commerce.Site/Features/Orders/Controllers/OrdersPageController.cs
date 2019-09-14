using EPiServer.Core;
using EPiServer.Reference.Commerce.B2B.Filters;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.Orders.Pages;
using EPiServer.Reference.Commerce.Site.Features.Orders.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Orders.Controllers
{
    [Authorize]
    public class OrdersPageController : PageController<OrdersPage>
    {
        private readonly ICustomerService _customerService;
        private readonly IOrdersService _ordersService;
        private readonly IContentLoader _contentLoader;

        public OrdersPageController(ICustomerService customerService, IOrdersService ordersService, IContentLoader contentLoader)
        {
            _customerService = customerService;
            _ordersService = ordersService;
            _contentLoader = contentLoader;
        }

        [NavigationAuthorize("Admin,Approver")]
        public ActionResult Index(OrdersPage currentPage)
        {
            var organizationUsersList = _customerService.GetContactsForCurrentOrganization();
            var viewModel = new OrdersPageViewModel
            {
                CurrentContent = currentPage
            };

            var ordersOrganization = new List<OrderOrganizationViewModel>();
            foreach (var user in organizationUsersList)
            {
                ordersOrganization.AddRange(_ordersService.GetUserOrders(user.ContactId));
            }
            viewModel.OrdersOrganization = ordersOrganization;

            viewModel.OrderDetailsPageUrl =
                UrlResolver.Current.GetUrl(_contentLoader.Get<BaseStartPage>(ContentReference.StartPage).OrderDetailsPage);
            return View(viewModel);
        }

        [NavigationAuthorize("Admin,Approver")]
        public ActionResult QuickOrder(OrdersPage currentPage)
        {
            var viewModel = new OrdersPageViewModel { CurrentContent = currentPage};
            return View(viewModel);
        }
    }
}