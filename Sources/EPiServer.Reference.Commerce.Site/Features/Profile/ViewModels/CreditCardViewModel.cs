using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels
{
    /// <summary>
    /// Represent for data of credit card on the view
    /// </summary>
    public class CreditCardViewModel: ContentViewModel<CreditCardPage>
    {
        public CreditCardModel CreditCard { get; set; }
        public bool IsB2B { get; set; }
        public List<OrganizationModel> Organizations { get; set; }
        public string ErrorMessage { get; set; }
        public List<OrganizationModel> GetAllOrganizationAndSub(OrganizationModel organizationInfo)
        {
            var result = new List<OrganizationModel>();
            if(organizationInfo != null)
            {
                GetAllOganizationAndSub(organizationInfo, result, 0);
            }
            return result;
        }
        private void GetAllOganizationAndSub(OrganizationModel organization, List<OrganizationModel> list, int level)
        {
            if (organization != null)
            {
                while (level > 0)
                {
                    organization.Name = ".." + organization.Name;
                    level--;
                }
                list.Add(organization);
                if (organization.SubOrganizations.Count > 0)
                {
                    foreach (var subOrg in organization.SubOrganizations)
                    {
                        GetAllOganizationAndSub(subOrg, list, level + 1);
                    }
                }
            }
        }

    }
}