using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Reference.Commerce.Shared.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Pages
{
    [ContentType(DisplayName = "Order and confirmation mail page", GUID = "535070c8-e08b-45ff-9703-c7d990174017", Description = "", AvailableInEditMode = false)]
    public class OrderConfirmationMailPage : MailBasePage
    {
    }
}