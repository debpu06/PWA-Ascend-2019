using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels
{
    public class WishListViewModel : CartViewModelBase<WishListPage> 
    {
        public WishListViewModel(WishListPage wishListPage) : base(wishListPage)
        {
        }
    }
}