using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.Pages;
using Mediachase.Commerce.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.SubscriptionHistory.ViewModels
{
    /// <summary>
    /// Model for list all payment plans
    /// </summary>
    public class PaymentPlanHistoryViewModel : ContentViewModel<PaymentPlanHistoryPage>
    {
        public List<PaymentPlan> PaymentPlans { get; set; }
        public string PaymentPlanDetailsPageUrl { get; set; }
    }
}