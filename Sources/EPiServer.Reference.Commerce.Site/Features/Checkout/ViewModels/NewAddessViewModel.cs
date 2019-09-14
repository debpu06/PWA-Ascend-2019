using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels
{
    public class NewAddessViewModel : ContentViewModel<CheckoutPage>
    {
        public AddressModel Address { get; set; }
    }
}