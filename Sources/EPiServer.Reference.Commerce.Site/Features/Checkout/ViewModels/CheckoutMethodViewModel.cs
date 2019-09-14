using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Login.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels
{
    public class CheckoutMethodViewModel : ContentViewModel<CheckoutPage>
    {
        public LoginViewModel LoginViewModel { get; set; }
        public RegisterAccountViewModel RegisterAccountViewModel { get; set; }

        public CheckoutMethodViewModel()
        {
            LoginViewModel = new LoginViewModel();
            RegisterAccountViewModel = new RegisterAccountViewModel
            {
                Address = new AddressModel()
            };
        }
        public CheckoutMethodViewModel(CheckoutPage currentPage, string returnUrl) : this()
        {
            LoginViewModel.ReturnUrl = returnUrl;
            CurrentContent = currentPage;
        }
    }
}