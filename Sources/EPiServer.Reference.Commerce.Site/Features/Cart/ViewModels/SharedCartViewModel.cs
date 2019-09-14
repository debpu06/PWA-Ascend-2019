using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels
{
    public class SharedCartViewModel : CartViewModelBase<SharedCartPage> 
    {
        public SharedCartViewModel(SharedCartPage sharedCartPage) : base(sharedCartPage)
        {
        }
    }
}