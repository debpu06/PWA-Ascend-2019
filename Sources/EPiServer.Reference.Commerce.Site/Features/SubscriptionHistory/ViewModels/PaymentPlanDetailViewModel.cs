using EPiServer.Reference.Commerce.Site.Features.OrderHistory.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.Pages;
using Mediachase.Commerce.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.ViewModels
{
    public class PaymentPlanDetailViewModel : ContentViewModel<PaymentPlanDetailPage>
    {
        public OrderHistoryViewModel Orders { get; set; }

        public PaymentPlan PaymentPlan { get; set; }
    }
}