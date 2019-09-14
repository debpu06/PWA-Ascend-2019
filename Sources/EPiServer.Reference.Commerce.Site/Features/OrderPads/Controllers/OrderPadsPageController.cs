using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.B2B;
using EPiServer.Reference.Commerce.B2B.Filters;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.OrderPads.Pages;
using EPiServer.Reference.Commerce.Site.Features.OrderPads.ViewModels;
using EPiServer.Web.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;

namespace EPiServer.Reference.Commerce.Site.Features.OrderPads.Controllers
{
    [Authorize]
    public class OrderPadsPageController : PageController<OrderPadsPage>
    {
        private readonly ICustomerService _customerService;
        private readonly ICartService _cartService;
        private readonly IOrganizationService _organizationService;
        private readonly CookieService _cookieService = new CookieService();

        public OrderPadsPageController(ICartService cartService, ICustomerService customerService, IOrganizationService organizationService)
        {
            _customerService = customerService;
            _cartService = cartService;
            _organizationService = organizationService;
        }

        [NavigationAuthorize("Admin,Approver")]
        public ActionResult Index(OrderPadsPage currentPage)
        {
            var currentOrganization = !string.IsNullOrEmpty(_cookieService.Get(Constants.Fields.SelectedSuborganization))
               ? _organizationService.GetSubOrganizationById(_cookieService.Get(Constants.Fields.SelectedSuborganization))
               : _organizationService.GetCurrentUserOrganization();

            var viewModel = new OrderPadsPageViewModel
            {
                CurrentContent = currentPage,
                QuoteStatus = "",
                CurrentCustomer = _customerService.GetCurrentContact(),
                OrganizationOrderPadList = new List<OrganizationOrderPadViewModel>()
            };

            if (currentOrganization != null)
            {
                if (string.IsNullOrEmpty(_cookieService.Get(Constants.Fields.SelectedSuborganization)))
                {
                    // Has suborganizatons. (is Organization)
                    foreach (var suborganization in currentOrganization.SubOrganizations)
                    {
                        viewModel.OrganizationOrderPadList.Add(AddSuborganizationToOrderPadList(suborganization.OrganizationId.ToString(), suborganization.Name));
                    }
                }
                else
                {
                    // Has only users. (is Suborganization)
                    viewModel.OrganizationOrderPadList.Add(AddSuborganizationToOrderPadList(currentOrganization.OrganizationId.ToString(), currentOrganization.Name));
                }
            }

            return View(viewModel);
        }

        private OrganizationOrderPadViewModel AddSuborganizationToOrderPadList(string suborganizationGuid, string suborganizationName)
        {
            var orderPadOrganization = new OrganizationOrderPadViewModel
            {
                OrganizationName = suborganizationName,
                OrganizationId = suborganizationGuid,
                UsersOrderPad = new List<UsersOrderPadViewModel>()
            };

            var organizationUsersList = _customerService.GetContactsByOrganizationId(suborganizationGuid);
            foreach (var user in organizationUsersList)
            {
                var userOrderPad = new UsersOrderPadViewModel
                {
                    UserName = user.FullName,
                    UserId = user.ContactId.ToString()
                };
                userOrderPad.WishCartList = _cartService.LoadWishListCardByCustomerId(user.ContactId);
                if (userOrderPad.WishCartList != null)
                {
                    foreach (var lineItem in userOrderPad.WishCartList.GetAllLineItems())
                    {
                        lineItem.PlacedPrice = _cartService.GetDiscountedPrice(userOrderPad.WishCartList, lineItem).Value.Amount;
                    }
                }
                orderPadOrganization.UsersOrderPad.Add(userOrderPad);
            }

            return orderPadOrganization;
        }

    }
}