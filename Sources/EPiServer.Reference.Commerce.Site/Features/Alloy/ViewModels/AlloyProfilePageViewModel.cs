using EPiServer.Personalization;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels
{
    public class AlloyProfilePageViewModel : ContentViewModel<AlloyProfilePage>
    {
        public AlloyProfilePageViewModel()
        {
            Profile = EPiServerProfile.Current;
        }

        public AlloyProfilePageViewModel(AlloyProfilePage currentPage)
            : base(currentPage)
        {
            Profile = EPiServerProfile.Current;
        }

        public EPiServerProfile Profile { get; set; }

        //public Boolean Impersonating { get; set; }
    }
}