using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels
{
    /// <summary>
    /// Represent for all credit cards of user or an organization
    /// </summary>
    public class CreditCardCollectionViewModel : ContentViewModel<CreditCardPage>
    {
        public IEnumerable<CreditCardModel> CreditCards { get; set; }
        public ContactViewModel CurrentContact { get; set; }
        public bool IsB2B { get; set; }
    }
}