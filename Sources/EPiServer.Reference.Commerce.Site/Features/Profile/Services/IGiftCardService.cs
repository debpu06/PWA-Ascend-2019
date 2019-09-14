using EPiServer.Reference.Commerce.Site.Features.Profile.Models;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Services
{
    public interface IGiftCardService
    {
        List<GiftCard> GetAllGiftCards();
        List<GiftCard> GetCustomerGiftCards(string contactId);
        string CreateGiftCard(GiftCard giftCard);
        string UpdateGiftCard(GiftCard giftCard);
        string DeleteGiftCard(string giftCardId);
    }
}
