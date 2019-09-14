using System.Collections.Generic;
using EPiServer.Reference.Commerce.Shared.Identity;
using EPiServer.Reference.Commerce.Site.Features.OrderHistory.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using Mediachase.Commerce.Customers;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels
{
    public class ProfilePageViewModel : ContentViewModel<ProfilePage>
    {
        public ProfilePageViewModel(ProfilePage profilePage) : base(profilePage)
        {
            
        }
        public List<OrderViewModel> Orders { get; set; }
        public IEnumerable<AddressModel> Addresses { get; set; }
        public SiteUser SiteUser { get; set; }
        public CustomerContact CustomerContact { get; set; }
        public string OrderDetailsPageUrl { get; set; }
    }
}