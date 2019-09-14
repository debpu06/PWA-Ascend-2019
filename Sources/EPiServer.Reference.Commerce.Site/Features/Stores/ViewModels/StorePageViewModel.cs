using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Stores.Pages;

namespace EPiServer.Reference.Commerce.Site.Features.Stores.ViewModels
{
    public class StorePageViewModel : ContentViewModel<StorePage>
    {
        public StoreViewModel StoreViewModel { get; set; }
    }
}