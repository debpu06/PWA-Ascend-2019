using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Tracking.Models;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.ViewModels
{
    public class TrackingBuilderPageViewModel : ContentViewModel<TrackingBuilderPage>
    {
        public TrackingBuilderPageViewModel(TrackingBuilderPage trackingPage) : base(trackingPage)
        {
        }

        public virtual ContentArea TrackingItems { get; set; }

        public virtual string CurrentUserEmail { get; set; }

        public virtual string ContentGuid { get; set; }
    }
}