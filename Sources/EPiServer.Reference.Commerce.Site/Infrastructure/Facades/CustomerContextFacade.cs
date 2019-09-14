using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Customers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Facades
{
    [ServiceConfiguration(typeof(CustomerContextFacade), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CustomerContextFacade
    {
        public CustomerContextFacade()
        {
            CurrentContact = new CurrentContactFacade();
        }
        public virtual CurrentContactFacade CurrentContact { get; }

        public virtual Guid CurrentContactId => CustomerContext.Current.CurrentContactId;

        public virtual CustomerContact GetContactById(Guid contactId)
        {
            return CustomerContext.Current.GetContactById(contactId);
        }

        public virtual IList<CustomerContact> GetContacts()
        {
            return CustomerContext.Current.GetContacts(0, 1000).ToList();
        }

        public virtual Organization GetOrganization(Guid id)
        {
            return CustomerContext.Current.GetOrganizationById(new PrimaryKeyId(id));
        }

        public virtual IList<Organization> GetOrganizations()
        {
            return CustomerContext.Current.GetOrganizations().ToList();
        }

        public virtual IList<CustomerContact> GetOrganizationCustomers(Guid id)
        {
            var org = CustomerContext.Current.GetOrganizationById(new PrimaryKeyId(id));
            return org?.Contacts.ToList() ?? new List<CustomerContact>();
        }

        public virtual IList<CreditCard> GetCreditCardsForOrganization(Organization organization)
        {
            return CustomerContext.Current.GetOrganizationCreditCards(organization).ToList();
        }
    }
}