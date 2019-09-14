using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Cart.Pages;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels
{
    public class SharedMiniCartViewModel : CartViewModelBase<SharedCartPage>
    {
        public SharedMiniCartViewModel(SharedCartPage sharedCartPage) : base(sharedCartPage)
        {
            
        }
        public ContentReference SharedCartPage { get; set; }
    }
}