using System;
using System.Linq;
using EPiServer.Commerce.Order;
using Mediachase.Commerce.Orders;
using OrderGroup = EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models.OrderGroup;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Mappings
{
    internal static class OrderGroupMappings
    {
        public static Cart ConvertToCart(this OrderGroup orderGroup, Cart cart)
        {
            if (!string.IsNullOrEmpty(orderGroup.AddressId))
            {
                cart.AddressId = orderGroup.AddressId;
            }

            if (orderGroup.AffiliateId != Guid.Empty)
            {
                cart.AffiliateId = orderGroup.AffiliateId;
            }

            if (!string.IsNullOrEmpty(orderGroup.BillingCurrency))
            {
                cart.BillingCurrency = orderGroup.BillingCurrency;
            }

            if (!string.IsNullOrEmpty(orderGroup.CustomerName))
            {
                cart.CustomerName = orderGroup.CustomerName;
            }

            if (orderGroup.HandlingTotal >= 0)
            {
                cart.HandlingTotal = orderGroup.HandlingTotal;
            }

            if (orderGroup.InstanceId != Guid.Empty)
            {
                cart.InstanceId = orderGroup.InstanceId;
            }

            if (orderGroup.MarketId != null)
            {
                cart.MarketId = orderGroup.MarketId;
            }

            if (!string.IsNullOrEmpty(orderGroup.Owner))
            {
                cart.Owner = orderGroup.Owner;
            }

            if (!string.IsNullOrEmpty(orderGroup.OwnerOrg))
            {
                cart.OwnerOrg = orderGroup.OwnerOrg;
            }

            if (!string.IsNullOrEmpty(orderGroup.ProviderId))
            {
                cart.ProviderId = orderGroup.ProviderId;
            }

            if (orderGroup.ShippingTotal >= 0)
            {
                cart.ShippingTotal = orderGroup.ShippingTotal;
            }

            if (!string.IsNullOrEmpty(orderGroup.Status))
            {
                cart.Status = orderGroup.Status;
            }

            if (orderGroup.SubTotal >= 0)
            {
                cart.SubTotal = orderGroup.SubTotal;
            }

            if (orderGroup.TaxTotal >= 0)
            {
                cart.TaxTotal = orderGroup.TaxTotal;
            }

            if (orderGroup.Total >= 0)
            {
                cart.Total = orderGroup.Total;
            }

            orderGroup.MapPropertiesToModel(cart);
            MapOrderAddresses(orderGroup, cart);
            MapOrderNotes(orderGroup, cart);
            MapOrderForms(orderGroup, cart);

            return cart;
        }

        public static PaymentPlan ConvertToPaymentPlan(this OrderGroup orderGroup, PaymentPlan paymentPlan)
        {
            if (!string.IsNullOrEmpty(orderGroup.AddressId))
            {
                paymentPlan.AddressId = orderGroup.AddressId;
            }

            if (orderGroup.AffiliateId != Guid.Empty)
            {
                paymentPlan.AffiliateId = orderGroup.AffiliateId;
            }

            if (!string.IsNullOrEmpty(orderGroup.BillingCurrency))
            {
                paymentPlan.BillingCurrency = orderGroup.BillingCurrency;
            }

            if (!string.IsNullOrEmpty(orderGroup.CustomerName))
            {
                paymentPlan.CustomerName = orderGroup.CustomerName;
            }

            if (orderGroup.HandlingTotal >= 0)
            {
                paymentPlan.HandlingTotal = orderGroup.HandlingTotal;
            }

            if (orderGroup.InstanceId != Guid.Empty)
            {
                paymentPlan.InstanceId = orderGroup.InstanceId;
            }

            if (orderGroup.MarketId != null)
            {
                paymentPlan.MarketId = orderGroup.MarketId;
            }

            if (!string.IsNullOrEmpty(orderGroup.Owner))
            {
                paymentPlan.Owner = orderGroup.Owner;
            }

            if (!string.IsNullOrEmpty(orderGroup.OwnerOrg))
            {
                paymentPlan.OwnerOrg = orderGroup.OwnerOrg;
            }

            if (!string.IsNullOrEmpty(orderGroup.ProviderId))
            {
                paymentPlan.ProviderId = orderGroup.ProviderId;
            }

            if (orderGroup.ShippingTotal >= 0)
            {
                paymentPlan.ShippingTotal = orderGroup.ShippingTotal;
            }

            if (!string.IsNullOrEmpty(orderGroup.Status))
            {
                paymentPlan.Status = orderGroup.Status;
            }

            if (orderGroup.SubTotal >= 0)
            {
                paymentPlan.SubTotal = orderGroup.SubTotal;
            }

            if (orderGroup.TaxTotal >= 0)
            {
                paymentPlan.TaxTotal = orderGroup.TaxTotal;
            }

            if (orderGroup.Total >= 0)
            {
                paymentPlan.Total = orderGroup.Total;
            }

            orderGroup.MapPropertiesToModel(paymentPlan);
            MapOrderAddresses(orderGroup, paymentPlan);
            MapOrderNotes(orderGroup, paymentPlan);
            MapOrderForms(orderGroup, paymentPlan);

            return paymentPlan;
        }

        public static PurchaseOrder ConvertToPurchaseOrder(this OrderGroup orderGroup, PurchaseOrder purchaseOrder)
        {
            if (!string.IsNullOrEmpty(orderGroup.AddressId))
            {
                purchaseOrder.AddressId = orderGroup.AddressId;
            }

            if (orderGroup.AffiliateId != Guid.Empty)
            {
                purchaseOrder.AffiliateId = orderGroup.AffiliateId;
            }

            if (!string.IsNullOrEmpty(orderGroup.BillingCurrency))
            {
                purchaseOrder.BillingCurrency = orderGroup.BillingCurrency;
            }

            if (!string.IsNullOrEmpty(orderGroup.CustomerName))
            {
                purchaseOrder.CustomerName = orderGroup.CustomerName;
            }

            if (orderGroup.HandlingTotal >= 0)
            {
                purchaseOrder.HandlingTotal = orderGroup.HandlingTotal;
            }

            if (orderGroup.InstanceId != Guid.Empty)
            {
                purchaseOrder.InstanceId = orderGroup.InstanceId;
            }

            if (orderGroup.MarketId != null)
            {
                purchaseOrder.MarketId = orderGroup.MarketId;
            }

            if (!string.IsNullOrEmpty(orderGroup.Owner))
            {
                purchaseOrder.Owner = orderGroup.Owner;
            }

            if (!string.IsNullOrEmpty(orderGroup.OwnerOrg))
            {
                purchaseOrder.OwnerOrg = orderGroup.OwnerOrg;
            }

            if (!string.IsNullOrEmpty(orderGroup.ProviderId))
            {
                purchaseOrder.ProviderId = orderGroup.ProviderId;
            }

            if (orderGroup.ShippingTotal >= 0)
            {
                purchaseOrder.ShippingTotal = orderGroup.ShippingTotal;
            }

            if (!string.IsNullOrEmpty(orderGroup.Status))
            {
                purchaseOrder.Status = orderGroup.Status;
            }

            if (orderGroup.SubTotal >= 0)
            {
                purchaseOrder.SubTotal = orderGroup.SubTotal;
            }

            if (orderGroup.TaxTotal >= 0)
            {
                purchaseOrder.TaxTotal = orderGroup.TaxTotal;
            }

            if (orderGroup.Total >= 0)
            {
                purchaseOrder.Total = orderGroup.Total;
            }

            orderGroup.MapPropertiesToModel(purchaseOrder);
            MapOrderAddresses(orderGroup, purchaseOrder);
            MapOrderNotes(orderGroup, purchaseOrder);
            MapOrderForms(orderGroup, purchaseOrder);

            return purchaseOrder;
        }

        public static T ConvertToOrderGroup<T>(
            this Mediachase.Commerce.Orders.OrderGroup orderGroup, T outOrderGroup)
            where T : OrderGroup
        {
            outOrderGroup.OrderForms = orderGroup.OrderForms.Select(x => x.ConvertToOrderForm()).ToArray();
            outOrderGroup.OrderAddresses = orderGroup.OrderAddresses.Select(x => x.ConvertToOrderAddress()).ToArray();
            outOrderGroup.OrderNotes = orderGroup.OrderNotes.Select(x => x.ConvertToOrderNote()).ToArray();
            outOrderGroup.Status = orderGroup.Status;
            outOrderGroup.OrderGroupId = orderGroup.OrderGroupId;
            outOrderGroup.CustomerId = orderGroup.CustomerId;
            outOrderGroup.ShippingTotal = orderGroup.ShippingTotal;
            outOrderGroup.SubTotal = orderGroup.SubTotal;
            outOrderGroup.Total = orderGroup.Total;
            outOrderGroup.HandlingTotal = orderGroup.HandlingTotal;
            outOrderGroup.TaxTotal = orderGroup.TaxTotal;
            outOrderGroup.Name = orderGroup.Name;
            outOrderGroup.AddressId = orderGroup.AddressId;
            outOrderGroup.AffiliateId = orderGroup.AffiliateId;
            outOrderGroup.BillingCurrency = orderGroup.BillingCurrency;
            outOrderGroup.CustomerName = orderGroup.CustomerName;
            outOrderGroup.InstanceId = orderGroup.InstanceId;
            outOrderGroup.MarketId = orderGroup.MarketId;
            outOrderGroup.Owner = orderGroup.Owner;
            outOrderGroup.OwnerOrg = orderGroup.OwnerOrg;
            outOrderGroup.ProviderId = orderGroup.ProviderId;
            outOrderGroup.Modified = orderGroup.Modified;
            outOrderGroup.Created = orderGroup.Created;
            outOrderGroup.Properties = orderGroup.ToPropertyList();

            return outOrderGroup;
        }

        public static Models.PurchaseOrder ConvertToPurchaseOrder(this PurchaseOrder purchaseOrder)
        {
            var ipo = (IPurchaseOrder) purchaseOrder;
            var po = new Models.PurchaseOrder {OrderNumber = ipo.OrderNumber};
            return purchaseOrder.ConvertToOrderGroup(po);
        }

        public static Models.PaymentPlan ConvertToPaymentPlan(this PaymentPlan paymentPlan)
        {
            var pp = new Models.PaymentPlan();
            return paymentPlan.ConvertToOrderGroup(pp);
        }

        private static void MapOrderAddresses(
            OrderGroup orderGroupDto, Mediachase.Commerce.Orders.OrderGroup orderGroup)
        {
            foreach (var orderAddress in orderGroupDto.OrderAddresses)
            {
                var address = orderGroup.OrderAddresses
                    .FirstOrDefault(x => x.OrderGroupAddressId == orderAddress.OrderGroupAddressId)
                              ?? orderGroup.OrderAddresses.AddNew();
                orderAddress.ConvertToOrderAddress(address);
            }
        }

        private static void MapOrderNotes(
            OrderGroup orderGroupDto, Mediachase.Commerce.Orders.OrderGroup orderGroup)
        {
            foreach (var orderNote in orderGroupDto.OrderNotes)
            {
                var note = orderGroup.OrderNotes
                    .FirstOrDefault(x => x.OrderNoteId == orderNote.OrderNoteId)
                              ?? orderGroup.OrderNotes.AddNew();
                orderNote.ConvertToOrderNote(note);
            }
        }

        private static void MapOrderForms(
            OrderGroup orderGroupDto, Mediachase.Commerce.Orders.OrderGroup orderGroup)
        {
            if (orderGroupDto.OrderForms.Length == 0)
            {
                return;
            }
            var orderForm = orderGroupDto.OrderForms.First();
            var form = orderGroup.OrderForms.First();
            orderForm.ConvertToOrderForm(form);
        }
    }
}