using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels
{
    public class ShipmentViewModel
	{
        public int ShipmentId { get; set; }

        public IList<CartItemViewModel> CartItems { get; set; }

        public AddressModel Address { get; set; }

        public Guid ShippingMethodId { get; set; }

        public IEnumerable<ShippingMethodViewModel> ShippingMethods { get; set; }

        public string CurrentShippingMethodName { get; set; }

        public Money CurrentShippingMethodPrice { get; set; }

	    public Money GetShipmentItemsTotal(Currency currency)
	    {
	        return CartItems.Aggregate(new Money(0, currency), (current, item) => current + item.DiscountedPrice.GetValueOrDefault());
	    }
    }
}