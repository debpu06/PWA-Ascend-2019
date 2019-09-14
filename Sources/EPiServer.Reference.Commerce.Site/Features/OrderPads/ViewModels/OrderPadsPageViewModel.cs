using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.OrderPads.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.OrderPads.ViewModels
{
    public class OrderPadsPageViewModel : ContentViewModel<OrderPadsPage>
    {
        public string QuoteStatus { get; set; }
        public ContactViewModel CurrentCustomer { get; set; }
        public List<ICart> OrderPardCartsList { get; set; } 
        public List<OrganizationOrderPadViewModel> OrganizationOrderPadList {get; set;} 
        
    }
}