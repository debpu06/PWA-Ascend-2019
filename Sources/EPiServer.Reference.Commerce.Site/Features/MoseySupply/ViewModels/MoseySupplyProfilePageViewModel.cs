using EPiServer.Core;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.MoseySupply.ViewModels
{
    public class MoseySupplyProfilePageViewModel : ContentViewModel<MoseySupplyProfilePage>
    {
        public ContactViewModel Contact { get; set; }
        public ContentReference OrganizationPage { get; set; }
        public BudgetViewModel CurrentPersonalBudget { get; set; }
    }
}