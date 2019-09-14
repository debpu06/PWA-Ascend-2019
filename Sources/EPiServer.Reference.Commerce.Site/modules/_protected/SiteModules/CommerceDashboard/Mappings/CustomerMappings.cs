using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Organization = Mediachase.Commerce.Customers.Organization;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Mappings
{
    internal static class CustomerMappings
    {
        public static Contact ConvertToContact(this CustomerContact customerContact)
        {
            if (customerContact == null)
            {
                return null;
            }

            return new Contact
            {
                PrimaryKeyId = customerContact.PrimaryKeyId,
                Addresses = customerContact.ContactAddresses.Select(a => a.ConvertToAddress()).ToArray(),
                FirstName = customerContact.FirstName,
                LastName = customerContact.LastName,
                Email = customerContact.Email,
                RegistrationSource = customerContact.RegistrationSource,
                Created = customerContact.Created,
                Modified = customerContact.Modified
            };
        }

        public static Models.Organization ConvertToOrganization(this Organization organization)
        {
            if (organization == null)
            {
                return null;
            }

            var organizationModel = new Models.Organization
            {
                OrganizationType = organization.OrganizationType,
                OrgCustomerGroup = organization.OrgCustomerGroup,
                Contacts = organization.Contacts.Select(c => c.ConvertToContact()),
                Addresses = organization.Addresses.Select(a => a.ConvertToAddress()),
                ChildOrganizations = organization.ChildOrganizations.Select(o => o.ConvertToOrganization())
            };

            if (organization.PrimaryKeyId.HasValue)
            {
                organizationModel.PrimaryKeyId = organization.PrimaryKeyId.Value;
            }

            return organizationModel;
        }


        public static void CreateContact(CustomerContact customerContact, Guid userId, Contact contact)
        {
            customerContact.PrimaryKeyId = new PrimaryKeyId(userId);
            customerContact.FirstName = contact.FirstName;
            customerContact.LastName = contact.LastName;
            customerContact.Email = contact.Email;
            customerContact.UserId = "String:" + contact.Email; // The UserId needs to be set in the format "String:{email}". Else a duplicate CustomerContact will be created later on.
            customerContact.RegistrationSource = contact.RegistrationSource;

            if (contact.Addresses != null)
            {
                foreach (var address in contact.Addresses)
                {
                    customerContact.AddContactAddress(address.ConvertToCustomerAddress(CustomerAddress.CreateInstance()));
                }
            }

            // The contact, or more likely its related addresses, must be saved to the database before we can set the preferred
            // shipping and billing addresses. Using an address id before its saved will throw an exception because its value
            // will still be null.
            customerContact.SaveChanges();

            // Once the contact has been saved we can look for any existing addresses.
            CustomerAddress defaultAddress = customerContact.ContactAddresses.FirstOrDefault();
            if (defaultAddress != null)
            {
                // If an addresses was found, it will be used as default for shipping and billing.
                customerContact.PreferredShippingAddress = defaultAddress;
                customerContact.PreferredBillingAddress = defaultAddress;

                // Save the address preferences also.
                customerContact.SaveChanges();
            }
        }

        public static CustomerAddress CreateOrUpdateCustomerAddress(CustomerContact contact, Address address)
        {
            var customerAddress = GetAddress(contact, address.AddressId);
            var isNew = customerAddress == null;
            IEnumerable<PrimaryKeyId> existingId = contact.ContactAddresses.Select(a => a.AddressId).ToList();
            if (isNew)
            {
                customerAddress = CustomerAddress.CreateInstance();
            }

            customerAddress = address.ConvertToCustomerAddress(customerAddress);

            if (isNew)
            {
                contact.AddContactAddress(customerAddress);
            }
            else
            {
                contact.UpdateContactAddress(customerAddress);
            }

            contact.SaveChanges();
            if (isNew)
            {
                customerAddress.AddressId = contact.ContactAddresses
                    .Where(a => !existingId.Contains(a.AddressId))
                    .Select(a => a.AddressId)
                    .Single();
                address.AddressId = customerAddress.AddressId;
            }

            return customerAddress;
        }

        public static CustomerAddress ConvertToCustomerAddress(this Address address, CustomerAddress customerAddress)
        {
            customerAddress.Name = address.Name;
            customerAddress.City = address.City;
            customerAddress.CountryCode = address.CountryCode;
            customerAddress.CountryName = GetAllCountries().Where(x => x.Code == address.CountryCode).Select(x => x.Name).FirstOrDefault();
            customerAddress.FirstName = address.FirstName;
            customerAddress.LastName = address.LastName;
            customerAddress.Line1 = address.Line1;
            customerAddress.Line2 = address.Line2;
            customerAddress.DaytimePhoneNumber = address.DaytimePhoneNumber;
            customerAddress.EveningPhoneNumber = address.EveningPhoneNumber;
            customerAddress.PostalCode = address.PostalCode;
            customerAddress.RegionName = address.RegionName;
            customerAddress.RegionCode = address.RegionCode;
            // Commerce Manager expects State to be set for addresses in order management. Set it to be same as
            // RegionName to avoid issues.
            customerAddress.State = address.RegionName;
            customerAddress.Email = address.Email;
            customerAddress.AddressType = CustomerAddressTypeEnum.Public | (address.ShippingDefault ? CustomerAddressTypeEnum.Shipping : 0) | (address.BillingDefault ? CustomerAddressTypeEnum.Billing : 0);

            return customerAddress;
        }

        public static Address ConvertToAddress(this CustomerAddress address)
        {
            if (address == null)
            {
                return null;
            }

            return new Address
            {
                Name = address.Name,
                City = address.City,
                CountryCode = address.CountryCode,
                CountryName = address.CountryName,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Line1 = address.Line1,
                Line2 = address.Line2,
                DaytimePhoneNumber = address.DaytimePhoneNumber,
                EveningPhoneNumber = address.EveningPhoneNumber,
                PostalCode = address.PostalCode,
                RegionName = address.RegionName,
                RegionCode = address.RegionCode,
                Email = address.Email,
                AddressId = address.AddressId,
                ShippingDefault = address.AddressType.HasFlag(CustomerAddressTypeEnum.Shipping),
                BillingDefault = address.AddressType.HasFlag(CustomerAddressTypeEnum.Billing),
                Modified = address.Modified
            };
        }

        private static List<CountryDto.CountryRow> GetAllCountries()
        {
            return CountryManager.GetCountries().Country.ToList();
        }

        private static CustomerAddress GetAddress(CustomerContact contact, Guid? addressId)
        {
            return addressId.HasValue ?
                contact.ContactAddresses.FirstOrDefault(x => x.AddressId == addressId.GetValueOrDefault()) :
                null;
        }
    }
}