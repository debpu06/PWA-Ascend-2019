using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EPiServer.Reference.Commerce.B2B.Models.Entities;
using EPiServer.Reference.Commerce.Shared.Attributes;

namespace EPiServer.Reference.Commerce.B2B.Models.ViewModels
{
    public class SubOrganizationModel : OrganizationModel
    {
        public SubOrganizationModel(B2BOrganization organization) : base(organization)
        {
            Name = organization.Name;
            Locations = organization.Addresses != null && organization.Addresses.Any()
                ? organization.Addresses.Select(address => new B2BAddressViewModel(address)).ToList()
                : new List<B2BAddressViewModel>();
        }

        public SubOrganizationModel()
        {
            Locations = new List<B2BAddressViewModel>();
        }

        [LocalizedDisplay("/B2B/Organization/SubOrganizationName")]
        [LocalizedRequired("/B2B/Organization/SubOrganizationNameRequired")]
        public new string Name { get; set; }

        public List<B2BAddressViewModel> Locations { get; set; }
        public IEnumerable<B2BCountryViewModel> CountryOptions { get; set; }
    }
}
