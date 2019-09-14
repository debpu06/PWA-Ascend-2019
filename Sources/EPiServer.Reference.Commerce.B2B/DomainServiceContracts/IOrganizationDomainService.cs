using System.Collections.Generic;
using EPiServer.Reference.Commerce.B2B.Models.Entities;

namespace EPiServer.Reference.Commerce.B2B.DomainServiceContracts
{
    public interface IOrganizationDomainService
    {
        B2BOrganization GetCurrentUserOrganizationEntity();
        B2BOrganization GetOrganizationEntityById(string organizationId);
        List<B2BOrganization> GetOrganizations();
        B2BOrganization GetNewOrganization();
    }
}
