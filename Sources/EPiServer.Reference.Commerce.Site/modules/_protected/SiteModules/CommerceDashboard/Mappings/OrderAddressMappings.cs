using Mediachase.Commerce.Orders;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Mappings
{
    internal static class OrderAddressMappings
    {
        public static OrderAddress ConvertToOrderAddress(
            this Models.OrderAddress orderAddressDto, OrderAddress orderAddress)
        {
            orderAddress.Name = orderAddressDto.Name;
            orderAddress.City = orderAddressDto.City;
            orderAddress.CountryCode = orderAddressDto.CountryCode;
            orderAddress.CountryName = orderAddressDto.CountryName;
            orderAddress.DaytimePhoneNumber = orderAddressDto.DaytimePhoneNumber;
            orderAddress.Email = orderAddressDto.Email;
            orderAddress.EveningPhoneNumber = orderAddressDto.EveningPhoneNumber;
            orderAddress.FaxNumber = orderAddressDto.FaxNumber;
            orderAddress.FirstName = orderAddressDto.FirstName;
            orderAddress.LastName = orderAddressDto.LastName;
            orderAddress.Line1 = orderAddressDto.Line1;
            orderAddress.Line2 = orderAddressDto.Line2;
            orderAddress.PostalCode = orderAddressDto.PostalCode;
            orderAddress.RegionName = orderAddressDto.RegionName;
            orderAddress.RegionCode = orderAddressDto.RegionCode;
            orderAddress.State = orderAddressDto.State;
            orderAddress.Organization = orderAddressDto.Organization;

            return orderAddress;
        }

        public static Models.OrderAddress ConvertToOrderAddress(this OrderAddress orderAddress)
        {
            return new Models.OrderAddress
            {
                Name = orderAddress.Name,
                City = orderAddress.City,
                CountryCode = orderAddress.CountryCode,
                CountryName = orderAddress.CountryName,
                DaytimePhoneNumber = orderAddress.DaytimePhoneNumber,
                Email = orderAddress.Email,
                EveningPhoneNumber = orderAddress.EveningPhoneNumber,
                FaxNumber = orderAddress.FaxNumber,
                FirstName = orderAddress.FirstName,
                LastName = orderAddress.LastName,
                Line1 = orderAddress.Line1,
                Line2 = orderAddress.Line2,
                PostalCode = orderAddress.PostalCode,
                RegionName = orderAddress.RegionName,
                RegionCode = orderAddress.RegionCode,
                State = orderAddress.State,
                Organization = orderAddress.Organization
            };
        }
    }
}