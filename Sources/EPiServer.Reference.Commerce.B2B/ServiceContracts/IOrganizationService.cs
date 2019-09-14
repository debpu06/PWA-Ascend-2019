using System;
using System.Collections.Generic;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;

namespace EPiServer.Reference.Commerce.B2B.ServiceContracts
{
    public interface IOrganizationService
    {
        OrganizationModel GetCurrentUserOrganization();
        OrganizationModel GetOrganizationModel(Guid id);
        List<OrganizationModel> GetOrganizationModels();
        void CreateOrganization(OrganizationModel organizationInfo);
        void UpdateOrganization(OrganizationModel organizationInfo);
        void CreateSubOrganization(SubOrganizationModel newSubOrganization);
        SubOrganizationModel GetSubOrganizationById(string subOrganizationId);
        void UpdateSubOrganization(SubOrganizationModel subOrganizationModel);
        string GetUserCurrentOrganizationLocation();
    }
}
