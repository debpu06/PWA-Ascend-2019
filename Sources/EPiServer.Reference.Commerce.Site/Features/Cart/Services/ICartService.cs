using System;
using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using Mediachase.Commerce;
using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Cart.Services
{
    public interface ICartService
    {
        ShippingMethodInfoModel InStorePickupInfoModel { get; }
        AddToCartResult AddToCart(ICart cart, string code, decimal quantity, string deliveryMethod, string warehouseCode);
        Dictionary<ILineItem, List<ValidationIssue>> ChangeCartItem(ICart cart, int shipmentId, string code, decimal quantity, string size, string newSize);
        void SetCartCurrency(ICart cart, Currency currency);
        Dictionary<ILineItem, List<ValidationIssue>> ValidateCart(ICart cart);
        Dictionary<ILineItem, List<ValidationIssue>> RequestInventory(ICart cart);
        string DefaultCartName { get; }
        string DefaultWishListName { get; }
        string DefaultSharedCardName { get; }
        CartWithValidationIssues LoadCart(string name, bool validate);
        CartWithValidationIssues LoadCart(string name, string contactId, bool validate);
        ICart LoadOrCreateCart(string name);
        ICart LoadOrCreateCart(string name, string contactId);
        bool AddCouponCode(ICart cart, string couponCode);
        void RemoveCouponCode(ICart cart, string couponCode);
        void RecreateLineItemsBasedOnShipments(ICart cart, IEnumerable<CartItemViewModel> cartItems, IEnumerable<AddressModel> addresses);
        void MergeShipments(ICart cart);
        ICart LoadWishListCardByCustomerId(Guid currentContactId);
        ICart LoadSharedCardByCustomerId(Guid currentContactId);
        Dictionary<ILineItem, List<ValidationIssue>> ChangeQuantity(ICart cart, int shipmentId, string code, decimal quantity);
        Money? GetDiscountedPrice(ICart cart, ILineItem lineItem);
    }
}