using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Orders.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Orders.ViewModels
{
    public class OrdersPageViewModel : ContentViewModel<OrdersPage>
    {
        public List<OrderOrganizationViewModel> OrdersOrganization { get; set; }
        public string OrderDetailsPageUrl { get; set; }
    }
}