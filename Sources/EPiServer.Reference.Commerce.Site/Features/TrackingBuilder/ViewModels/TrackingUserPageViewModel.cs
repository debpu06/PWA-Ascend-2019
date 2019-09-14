using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.TrackingBuilder.ViewModels
{
    public class TrackingUserPageViewModel : ContentViewModel<TrackingUserPage>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string TrackingPageUrl { get; set; }
        public bool IsUserAdmin { get; set; }
    }
}