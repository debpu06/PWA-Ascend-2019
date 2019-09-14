using EPiServer.Reference.Commerce.B2B.Extensions;
using Mediachase.Commerce.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models
{
    public class ElevatedRoleContact
    {
        public ElevatedRoleContact(CustomerContact contact)
        {
            Contact = contact;
        }

        public CustomerContact Contact { get; }

        public string ElevatedRole
        {
            get { return Contact.GetStringValue("ElevatedRole"); }
            set { Contact["ElevatedRole"] = value; }
        }

        public ElevatedRoles CustomerElevatedRole
        {
            get
            {
                var parsed = Enum.TryParse(ElevatedRole, out ElevatedRoles retVal);
                return parsed ? retVal : ElevatedRoles.Nonuser;
            }
        }

        public void SaveChanges()
        {
            Contact.SaveChanges();
        }
    }
}