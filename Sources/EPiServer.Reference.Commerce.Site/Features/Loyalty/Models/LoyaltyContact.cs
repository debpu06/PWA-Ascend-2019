using System;
using EPiServer.Reference.Commerce.B2B.Enums;
using EPiServer.Reference.Commerce.B2B.Extensions;
using EPiServer.Reference.Commerce.B2B.Models.Entities;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Customers;

namespace EPiServer.Reference.Commerce.Site.Features.Loyalty.Models
{
    public class LoyaltyContact
    {
        public LoyaltyContact(CustomerContact contact)
        {
            Contact = contact;
        }

        public CustomerContact Contact { get; }

        public int Points
        {
            get { return Contact.GetIntegerValue("Points"); }
            set { Contact["Points"] = value; }
        }
        public int NumberOfOrders
        {
            get { return Contact.GetIntegerValue("NumberOfOrders"); }
            set { Contact["NumberOfOrders"] = value; }
        }
        public int NumberOfReviews
        {
            get { return Contact.GetIntegerValue("NumberOfReviews"); }
            set { Contact["NumberOfReviews"] = value; }
        }
        public string Tier
        {
            get { return Contact.GetStringValue("Tier"); }
            set { Contact["Tier"] = value; }
        }

        public CustomerTiers CustomerTier
        {
            get
            {
                var parsed = Enum.TryParse(Tier, out CustomerTiers retVal);
                return parsed ? retVal : CustomerTiers.Classic;
            }
        }

        public void SaveChanges()
        {
            Contact.SaveChanges();
        }
    }
}