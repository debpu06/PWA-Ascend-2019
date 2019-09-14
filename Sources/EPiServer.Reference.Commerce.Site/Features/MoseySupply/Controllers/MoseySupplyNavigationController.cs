using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Navigation.ViewModels;
using EPiServer.SpecializedProperties;
using EPiServer.Web.Mvc.Html;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;

namespace EPiServer.Reference.Commerce.Site.Features.MoseySupply.Controllers
{
    public class MoseySupplyNavigationController : Controller
    {
        private readonly IContentLoader _contentLoader;
        private readonly ICartService _cartService;
        private readonly UrlHelper _urlHelper;
        private readonly LocalizationService _localizationService;
        readonly CartViewModelFactory _cartViewModelFactory;
        private readonly CustomerContextFacade _customerContext;
        private readonly ICustomerService _customerService;

        public MoseySupplyNavigationController(
            IContentLoader contentLoader,
            ICartService cartService,
            UrlHelper urlHelper,
            LocalizationService localizationService,
            CartViewModelFactory cartViewModelFactory,
            ICustomerService customerService,
            CustomerContextFacade customerContext
            )
        {
            _contentLoader = contentLoader;
            _cartService = cartService;
            _urlHelper = urlHelper;
            _localizationService = localizationService;
            _cartViewModelFactory = cartViewModelFactory;
            _customerContext = customerContext;
            _customerService = customerService;
        }

        public ActionResult Index(IContent currentContent)
        {
            var cart = _cartService.LoadCart(_cartService.DefaultCartName, true)?.Cart;
            var wishlist = _cartService.LoadCart(_cartService.DefaultWishListName, true)?.Cart;
            var organizationId = _customerService.GetCurrentContact().Organization?.OrganizationId.ToString();
            var organization = _customerService.GetCurrentContact().Organization?.Name ?? "";
            var sharedcart = _cartService.LoadCart(_cartService.DefaultSharedCardName, organizationId, true)?.Cart;
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);

            var viewModel = new NavigationViewModel
            {
                StartPage = startPage,
                CurrentContentLink = currentContent?.ContentLink,
                UserLinks = new LinkItemCollection(),
                MiniCart = _cartViewModelFactory.CreateMiniCartViewModel(cart),
                //WishListMiniCart = _cartViewModelFactory.CreateWishListMiniCartViewModel(wishlist),
                //SharedMiniCart = _cartViewModelFactory.CreateSharedMiniCartViewModel(sharedcart),
                ShowCommerceControls = startPage.ShowCommerceHeaderComponents,
                ShowSharedCart = !string.IsNullOrEmpty(organizationId),
                RestrictedMenu = !string.IsNullOrEmpty(organization) && organization.Equals("Smith Construction") ? new LinkItemCollection
                {
                    new LinkItem
                    {
                        Text = "Home Improvment",
                        Href = "/en/quicksilverb2b/home-improvement/"
                    }
                } : startPage.B2BHeaderMainNavigation,
                Name = Request.IsAuthenticated ? _customerContext.CurrentContact.CurrentContact.FirstName : "",
            };

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var rightMenuItems = startPage.HeaderMetaNavigation;
                if (rightMenuItems != null)
                {
                    viewModel.UserLinks.AddRange(rightMenuItems);
                }

                viewModel.UserLinks.Add(new LinkItem
                {
                    Href = _urlHelper.Action("SignOut", "Login"),
                    Text = _localizationService.GetString("/Header/Account/SignOut")
                });
            }
            else
            {
                viewModel.UserLinks.Add(new LinkItem
                {
                    Href = _urlHelper.Action("Index", "Login", new { returnUrl = currentContent != null ? _urlHelper.ContentUrl(currentContent.ContentLink) : "/" }),
                    Text = _localizationService.GetString("/Header/Account/SignIn")
                });
            }

            return PartialView(viewModel);
        }
    }
}