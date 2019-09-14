using EPiServer.Reference.Commerce.Site.Features.Profile.Models;
using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels
{
    public class GiftCardViewModel : ContentViewModel<GiftCardPage>
    {
        public List<GiftCard> GiftCardList { get; set; }
    }
}