using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.OrderHistory.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.OrderHistory.ViewModels
{
    public class OrderHistoryViewModel : ContentViewModel<OrderHistoryPage>
    {
        public List<OrderViewModel> Orders { get; set; }
        public string OrderDetailsPageUrl { get; set; }
        public ContactViewModel CurrentCustomer { get; set; }

        public int CycleMode { get; set; }
        public int CycleLength { get; set; }

        public List<SelectListItem> Modes => new List<SelectListItem>
        {
            new SelectListItem { Text = "Every x Days", Value="1"},
            new SelectListItem { Text = "Every x Weeks", Value="2"},
            new SelectListItem { Text = "Every X Months", Value="3"},
            new SelectListItem { Text = "Every X Years", Value="4"}
        };
    }
}