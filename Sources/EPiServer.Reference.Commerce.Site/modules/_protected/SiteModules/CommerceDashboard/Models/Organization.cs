using System;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models
{
    /// <summary>
    /// Organization model.
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// Organization ID (GUID).
        /// </summary>
        public Guid PrimaryKeyId { get; set; }

        /// <summary>
        /// Array of addresses.
        /// </summary>
        public IEnumerable<Address> Addresses { get; set; }

        /// <summary>
        /// Array of child organizations.
        /// </summary>
        public IEnumerable<Organization> ChildOrganizations { get; set; }
        
        /// <summary>
        /// Array of contacts.
        /// </summary>
        public IEnumerable<Contact> Contacts { get; set; }
        
        /// <summary>
        /// Organization type.
        /// </summary>
        public string OrganizationType { get; set; }
        
        /// <summary>
        /// Organization customer group.
        /// </summary>
        public string OrgCustomerGroup { get; set; } 
        
        // TODO Add property for CreditCards 
    }
}