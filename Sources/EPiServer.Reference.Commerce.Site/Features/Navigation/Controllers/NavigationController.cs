using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.Bookmark;
using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Navigation.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Navigation.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Security;
using System;
using System.Linq;
using System.Web.Mvc;
using Constants = EPiServer.Reference.Commerce.B2B.Constants;

namespace EPiServer.Reference.Commerce.Site.Features.Navigation.Controllers
{
    public class NavigationController : Controller
    {
        private readonly IContentLoader _contentLoader;
        private readonly ICartService _cartService;
        private readonly UrlHelper _urlHelper;
        private readonly LocalizationService _localizationService;
        readonly CartViewModelFactory _cartViewModelFactory;
        private readonly CustomerContextFacade _customerContext;
        private readonly CookieService _cookieService = new CookieService();
        private readonly IOrganizationService _organizationService;
        private readonly ICustomerService _customerService;
        private readonly IPageRouteHelper _pageRouteHelper;
        private readonly UrlResolver _urlResolver;
        private readonly IBookmarksService _bookmarksService;

        public NavigationController(
            IContentLoader contentLoader,
            ICartService cartService,
            UrlHelper urlHelper,
            LocalizationService localizationService,
            CartViewModelFactory cartViewModelFactory,
            CustomerContextFacade customerContext,
            IOrganizationService organizationService,
            ICustomerService customerService,
            IPageRouteHelper pageRouteHelper,
            UrlResolver urlResolver,
            IBookmarksService bookmarksService)
        {
            _contentLoader = contentLoader;
            _cartService = cartService;
            _urlHelper = urlHelper;
            _localizationService = localizationService;
            _cartViewModelFactory = cartViewModelFactory;
            _customerContext = customerContext;
            _organizationService = organizationService;
            _customerService = customerService;
            _pageRouteHelper = pageRouteHelper;
            _urlResolver = urlResolver;
            _bookmarksService = bookmarksService;
        }

        public ActionResult Header(IContent currentContent)
        {
            var cart = _cartService.LoadCart(_cartService.DefaultCartName, true)?.Cart;
            var wishlist = _cartService.LoadCart(_cartService.DefaultWishListName, true)?.Cart;
            var organizationId = _customerService.GetCurrentContact().Organization?.OrganizationId.ToString();
            var organization = _customerService.GetCurrentContact().Organization?.Name ?? "";
            var sharedcart = _cartService.LoadCart(_cartService.DefaultSharedCardName, organizationId, true)?.Cart;
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            var isBookmarked = false;
            if (Request.IsAuthenticated)
            {
                var bookmarks = _bookmarksService.Get();
                if (bookmarks != null && bookmarks.Any() && bookmarks.FirstOrDefault().ContentGuid != null && currentContent != null)
                {
                    isBookmarked = bookmarks.FirstOrDefault(x => x.ContentGuid == currentContent.ContentGuid) != null;
                }
            }
            var viewModel = new NavigationViewModel
            {
                StartPage = startPage,
                CurrentContentLink = currentContent?.ContentLink,
                UserLinks = new LinkItemCollection(),
                MiniCart = _cartViewModelFactory.CreateMiniCartViewModel(cart),
                WishListMiniCart = _cartViewModelFactory.CreateMiniWishListViewModel(wishlist),
                SharedMiniCart = _cartViewModelFactory.CreateMiniCartViewModel(sharedcart, true),
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
                IsBookmarked = isBookmarked
            };

            var menuItems = (startPage.ShowCommerceHeaderComponents ? startPage.MyAccountMenu : startPage.BasicAccountMenu) ?? new LinkItemCollection();

            //Get css information for each item on menuItems
            foreach (var linkItem in menuItems)
            {
                string linkUrl;

                if (!UrlResolver.Current.TryToPermanent(linkItem.Href, out linkUrl))
                {
                    continue;
                }

                if (String.IsNullOrEmpty(linkUrl))
                {
                    continue;
                }

                var urlBuilder = new UrlBuilder(linkUrl);

                var page = _urlResolver.Route(urlBuilder) as Commerce.Shared.SitePageData;

                if (page != null && !string.IsNullOrEmpty(page.Css))
                {
                    if (!linkItem.Attributes.ContainsKey("css"))
                    {
                        linkItem.Attributes.Add("css", page.Css);
                    }
                    else if (!linkItem.Attributes["css"].Contains(page.Css))
                    {
                        linkItem.Attributes["css"] += $" {page.Css}";
                    }
                    linkItem.Title = linkItem.Text;
                }
            }

            viewModel.UserLinks.AddRange(menuItems);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var signoutText = _localizationService.GetString("/Header/Account/SignOut");
                var link = new LinkItem
                {
                    Href = _urlHelper.Action("SignOut", "Login"),
                    Text = signoutText,
                    Title = signoutText
                };
                link.Attributes.Add("css", "fa-sign-out");
                viewModel.UserLinks.Add(link);
            }

            return PartialView("~/Views/Shared/Header/_Header.cshtml", viewModel);
        }

        public ActionResult MyAccountHeaderMenu(IContent currentContent)
        {
            var cart = _cartService.LoadCart(_cartService.DefaultCartName, true)?.Cart;
            var wishlist = _cartService.LoadCart(_cartService.DefaultWishListName, true)?.Cart;
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            var currentOrganizationId = _customerService.GetCurrentContact().Organization?.OrganizationId.ToString();
            var sharedcart = _cartService.LoadCart(_cartService.DefaultSharedCardName, currentOrganizationId, true)?.Cart;

            var viewModel = new NavigationViewModel
            {
                StartPage = startPage,
                CurrentContentLink = currentContent?.ContentLink,
                UserLinks = new LinkItemCollection(),
                MiniCart = _cartViewModelFactory.CreateMiniCartViewModel(cart),
                WishListMiniCart = _cartViewModelFactory.CreateMiniWishListViewModel(wishlist),
                SharedMiniCart = _cartViewModelFactory.CreateMiniCartViewModel(sharedcart, true),
                ShowCommerceControls = startPage.ShowCommerceHeaderComponents,
                ShowSharedCart = !string.IsNullOrEmpty(currentOrganizationId)
            };

            var menuItems = (startPage.ShowCommerceHeaderComponents ? startPage.MyAccountMenu : startPage.BasicAccountMenu) ?? new LinkItemCollection();

            //Get css information for each item on menuItems
            foreach (var linkItem in menuItems)
            {
                string linkUrl;

                if (!UrlResolver.Current.TryToPermanent(linkItem.Href, out linkUrl))
                    continue;

                if (String.IsNullOrEmpty(linkUrl))
                    continue;

                var urlBuilder = new UrlBuilder(linkUrl);
                var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();

                var page = urlResolver.Route(urlBuilder) as EPiServer.Reference.Commerce.Shared.SitePageData;

                if (page != null && !string.IsNullOrEmpty(page.Css))
                {
                    if (!linkItem.Attributes.ContainsKey("css"))
                    {
                        linkItem.Attributes.Add("css", page.Css);
                    }
                    else if (!linkItem.Attributes["css"].Contains(page.Css))
                    {
                        linkItem.Attributes["css"] += $" {page.Css}";
                    }
                    linkItem.Title = linkItem.Text;
                }
            }

            viewModel.UserLinks.AddRange(menuItems);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var signoutText = _localizationService.GetString("/Header/Account/SignOut");
                var link = new LinkItem
                {
                    Href = _urlHelper.Action("SignOut", "Login"),
                    Text = signoutText,
                    Title = signoutText
                };
                link.Attributes.Add("css", "fa-sign-out");
                viewModel.UserLinks.Add(link);
            }

            return PartialView("~/Views/Shared/Header/_MyAccountHeaderMenu.cshtml", viewModel);
        }

        /// <summary>
        /// Render the mobile navigation
        /// </summary>
        /// <param name="currentContent"></param>
        /// <returns></returns>
        public ActionResult MobileNavigation(IContent currentContent)
        {
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            var viewModel = new MobileNavigationViewModel
            {
                StartPage = startPage,
                //Pages = startPage.MobileNavigationPages,
                MenuModel = startPage.MobileNavigationPages.GetMegaMenuModel()
            };


            return PartialView("~/Views/Shared/Header/_NavigationMobile.cshtml", viewModel);
        }

        public ActionResult MyAccountMenu(MyAccountPageType id)
        {
            var startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            if (startPage == null)
            {
                return new EmptyResult();
            }

            var selectedSubNav = _cookieService.Get(Constants.Fields.SelectedNavSuborganization);
            var organization = _organizationService.GetCurrentUserOrganization();
            var canSeeOrganizationNav = _customerService.CanSeeOrganizationNav();

            var model = new MyAccountNavigationViewModel
            {
                Organization = canSeeOrganizationNav ? organization : null,
                CurrentOrganization = !string.IsNullOrEmpty(selectedSubNav) ?
                    _organizationService.GetSubOrganizationById(selectedSubNav) :
                    _organizationService.GetCurrentUserOrganization(),
                CurrentPageType = id,
                OrganizationPage = startPage.OrganizationMainPage,
                SubOrganizationPage = startPage.SubOrganizationPage,
                MenuItemCollection = new LinkItemCollection()
            };

            var menuItems = startPage.ShowCommerceHeaderComponents ? startPage.MyAccountMenu : startPage.BasicAccountMenu;
            if (menuItems == null)
            {
                return PartialView("_ProfileSidebar", model);
            }
            var wishlist = _contentLoader.Get<WishListPage>(startPage.WishListPage);
            menuItems = menuItems.CreateWritableClone();

            if (model.Organization != null)
            {
                if (wishlist != null)
                {
                    var url = wishlist.LinkURL.Contains("?") ? wishlist.LinkURL.Split('?').First() : wishlist.LinkURL;
                    var item = menuItems.FirstOrDefault(x => x.Href.Substring(1).Equals(url));
                    if (item != null)
                    {
                        menuItems.Remove(item);
                    }
                }
                menuItems.Add(new LinkItem
                {
                    Href = _urlResolver.GetUrl(startPage.QuickOrderPage),
                    Text = _localizationService.GetString("/Dashboard/Labels/QuickOrder")
                });
            }
            else if (organization != null)
            {
                if (wishlist != null)
                {
                    var url = wishlist.LinkURL.Contains("?") ? wishlist.LinkURL.Split('?').First() : wishlist.LinkURL;
                    var item = menuItems.FirstOrDefault(x => x.Href.Substring(1).Equals(url));
                    if (item != null)
                    {
                        item.Text = _localizationService.GetString("/Dashboard/Labels/OrderPad");
                    }
                }
            }

            model.MenuItemCollection.AddRange(menuItems);

            if (id == MyAccountPageType.Organization)
            {
                return PartialView("_ProfileSidebar", model);
            }

            var currentContent = _pageRouteHelper.Page;
            foreach (var menuItem in menuItems)
            {
                var content = UrlResolver.Current.Route(new UrlBuilder(menuItem.Href));
                if (content == null)
                {
                    continue;
                }
                if (currentContent.ContentLink == content.ContentLink)
                {
                    model.CurrentPageText = menuItem.Text;
                }
            }

            return PartialView("_ProfileSidebar", model);
        }


    }
}