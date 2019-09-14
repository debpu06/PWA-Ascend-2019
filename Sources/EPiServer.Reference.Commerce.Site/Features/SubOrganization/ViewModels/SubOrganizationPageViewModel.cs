using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.SubOrganization.Pages;

namespace EPiServer.Reference.Commerce.Site.Features.SubOrganization.ViewModels
{
    public class SubOrganizationPageViewModel : ContentViewModel<SubOrganizationPage>
    {
        public SubOrganizationModel SubOrganizationModel { get; set; }
        public bool IsAdmin { get; set; }
    }
}