using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels
{
    public class WishListMiniCartViewModel : CartViewModelBase<WishListPage>
    {
        public WishListMiniCartViewModel(WishListPage wishListPage) : base(wishListPage)
        {
            
        }
        public ContentReference WishListPage { get; set; }

        public string Label { get; set; }
    }
}