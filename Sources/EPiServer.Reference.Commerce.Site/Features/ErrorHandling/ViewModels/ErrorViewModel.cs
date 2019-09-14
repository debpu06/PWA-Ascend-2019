using EPiServer.Reference.Commerce.Site.Features.ErrorHandling.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.ErrorHandling.ViewModels
{
    public class ErrorViewModel : ContentViewModel<ErrorPage> 
    {
        public ErrorViewModel(ErrorPage errorPage) : base(errorPage)
        {
            
        }
    }
}