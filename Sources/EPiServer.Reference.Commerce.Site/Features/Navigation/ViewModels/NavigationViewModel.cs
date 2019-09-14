using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.SpecializedProperties;

namespace EPiServer.Reference.Commerce.Site.Features.Navigation.ViewModels
{
    public class NavigationViewModel
    {
        public ContentReference CurrentContentLink { get; set; }
        public BaseStartPage StartPage { get; set; }
        public LinkItemCollection UserLinks { get; set; }
        public MiniCartViewModel MiniCart { get; set; }
        public MiniWishlistViewModel WishListMiniCart { get; set; }
        public MiniCartViewModel SharedMiniCart { get; set; }
        public string Name { get; set; }
        public bool ShowCommerceControls { get; set; }
        public bool ShowSharedCart { get; set; }
        public PageData StorePage { get; set; }
        public LinkItemCollection RestrictedMenu { get; set; }
        public bool HasOrganization { get; set; }
        public bool IsBookmarked { get; set; }

    }

    /// <summary>
    /// Model for Mobile Navigation
    /// </summary>
    public class MobileNavigationViewModel
    {
        public MegaMenuModel MenuModel { get; set; }

        public LinkItemCollection Pages { get; set; }

        public BaseStartPage StartPage { get; set; }
    }
}