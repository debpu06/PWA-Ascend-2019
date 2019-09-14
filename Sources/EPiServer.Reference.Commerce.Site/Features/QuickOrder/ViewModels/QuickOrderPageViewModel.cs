using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.QuickOrder.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.QuickOrder.ViewModels
{
    public class QuickOrderPageViewModel : ContentViewModel<QuickOrderPage>
    {
        public List<ProductViewModel> ProductsList { get; set; }
        public List<string> ReturnedMessages  { get; set; }
    }
}