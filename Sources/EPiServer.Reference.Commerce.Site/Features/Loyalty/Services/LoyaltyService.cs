using EPiServer.Reference.Commerce.Site.Features.Loyalty.Models;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Customers;

namespace EPiServer.Reference.Commerce.Site.Features.Loyalty.Services
{
    [ServiceConfiguration(typeof(ILoyaltyService))]
    public class LoyaltyService : ILoyaltyService
    {
        public void AddNumberOfOrders()
        {
            var currentContact = CustomerContext.Current.CurrentContact;
            if (currentContact != null)
            {
                var contact = new LoyaltyContact(currentContact);
                contact.NumberOfOrders += 1;
                contact.Points += 10;
                contact.Tier = SetTier(contact.Points);
                contact.SaveChanges();
            }
        }

        public void AddNumberOfReviews()
        {
            var currentContact = CustomerContext.Current.CurrentContact;
            if (currentContact != null)
            {
                var contact = new LoyaltyContact(currentContact);
                contact.NumberOfReviews += 1;
                contact.Points += 1;
                contact.Tier = SetTier(contact.Points);
                contact.SaveChanges();
            }
        }

        private string SetTier(int points)
        {
            if (points <= 100)
            {
                return "Classic";
            }
            else if (points <= 200)
            {
                return "Bronze";
            }
            else if (points <= 500)
            {
                return "Silver";
            }
            else if (points <= 1000)
            {
                return "Gold";
            }
            else if (points <= 2000)
            {
                return "Platium";
            }
            else
            {
                return "Diamond";
            }
        }
    }
}