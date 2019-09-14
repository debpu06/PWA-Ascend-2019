using System;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Commerce.Order;
using EPiServer.Commerce.Order.Internal;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using OrderForm = Mediachase.Commerce.Orders.OrderForm;
using Shipment = Mediachase.Commerce.Orders.Shipment;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Mappings
{
    internal static class OrderFormMappings
    {
        public static OrderForm ConvertToOrderForm(
            this Models.OrderForm orderFormDto, OrderForm orderForm)
        {
            orderForm.ReturnComment = orderFormDto.ReturnComment;
            orderForm.ReturnType = orderFormDto.ReturnType;
            orderForm.ReturnAuthCode = orderFormDto.ReturnAuthCode;
            orderForm.Name = orderFormDto.Name;
            orderForm.BillingAddressId = orderFormDto.BillingAddressId;
            orderForm.ShippingTotal = orderFormDto.ShippingTotal;
            orderForm.HandlingTotal = orderFormDto.HandlingTotal;
            orderForm.TaxTotal = orderFormDto.TaxTotal;
            orderForm.DiscountAmount = orderFormDto.DiscountAmount;
            orderForm.SubTotal = orderFormDto.SubTotal;
            orderForm.Total = orderFormDto.Total;
            orderForm.Status = orderFormDto.Status;
            orderForm.RMANumber = orderFormDto.RmaNumber;
            orderForm.AuthorizedPaymentTotal = orderFormDto.AuthorizedPaymentTotal;
            orderForm.CapturedPaymentTotal = orderFormDto.CapturedPaymentTotal;

            orderFormDto.MapPropertiesToModel(orderForm);
            MapShipments(orderFormDto, orderForm);

            return orderForm;
        }

        private static void MapShipments(Models.OrderForm orderFormDto, OrderForm orderForm)
        {
            orderForm.Shipments.Clear();
            foreach (var shipmentDto in orderFormDto.Shipments)
            {
                var shipment = orderForm.Shipments.AddNew();
                shipmentDto.ConvertToShipment(shipment, orderForm);
            }
        }

        public static Models.OrderForm ConvertToOrderForm(this OrderForm orderForm)
        {
            return new Models.OrderForm
            {
                ReturnComment = orderForm.ReturnComment,
                ReturnType = orderForm.ReturnType,
                ReturnAuthCode = orderForm.ReturnAuthCode,
                Name = orderForm.Name,
                BillingAddressId = orderForm.BillingAddressId,
                ShippingTotal = orderForm.ShippingTotal,
                HandlingTotal = orderForm.HandlingTotal,
                TaxTotal = orderForm.TaxTotal,
                DiscountAmount = orderForm.DiscountAmount,
                SubTotal = orderForm.SubTotal,
                Total = orderForm.Total,
                Status = orderForm.Status,
                RmaNumber = orderForm.RMANumber,
                AuthorizedPaymentTotal = orderForm.AuthorizedPaymentTotal,
                CapturedPaymentTotal = orderForm.CapturedPaymentTotal,

                Shipments = orderForm.Shipments.Select(ConvertToShipment).ToArray(),
                LineItems = orderForm.LineItems.Select(ConvertToLineItem).ToArray(),
                //Discounts = orderForm.Discounts.Select(ConvertToDiscount).ToArray(),

                OrderFormId = orderForm.OrderFormId,
                Properties = orderForm.ToPropertyList()
            };
        }

        //private static Models.Discount ConvertToDiscount(Discount discount)
        //{
        //    return new Models.Discount
        //    {
        //        DiscountAmount = discount.DiscountAmount,
        //        DiscountCode = discount.DiscountCode,
        //        DiscountId = discount.DiscountId,
        //        DiscountName = discount.DiscountName,
        //        DiscountValue = discount.DiscountValue,
        //        DisplayMessage = discount.DisplayMessage
        //    };
        //}

        private static Models.LineItem ConvertToLineItem(ILineItem lineItem)
        {
            var defaultCurrency = Currency.USD; // Using USD as currency doesn't matter - calculation result will not use currency only amount.
            return new Models.LineItem
            {
                LineItemId = lineItem.LineItemId,
                Code = lineItem.Code,
                DisplayName = lineItem.DisplayName,
                PlacedPrice = lineItem.PlacedPrice,
                ExtendedPrice = lineItem.GetExtendedPrice(defaultCurrency).Amount,
                DiscountedPrice = lineItem.GetDiscountedPrice(defaultCurrency).Amount,
                LineItemDiscountAmount = lineItem.GetEntryDiscount(),
                OrderLevelDiscountAmount = lineItem.TryGetDiscountValue(x => x.OrderAmount),
                Quantity = lineItem.Quantity,
                ReturnQuantity = lineItem.ReturnQuantity,
                InventoryTrackingStatus = lineItem.InventoryTrackingStatus,
                IsInventoryAllocated = lineItem.IsInventoryAllocated,
                IsGift = lineItem.IsGift,
                Properties = lineItem.ToPropertyList()
            };
        }

        private static Models.Shipment ConvertToShipment(Shipment shipment)
        {
            return new Models.Shipment
            {
                LineItems = shipment.LineItems.Select(ConvertToLineItem).ToArray(),
                //Discounts = shipment.Discounts.Select(ConvertToDiscount).ToArray(),
                ShipmentId = shipment.ShipmentId,
                Status = shipment.Status,
                ShippingSubTotal = shipment.ShippingSubTotal,
                ShippingTotal = shipment.ShippingTotal,
                ShippingMethodId = shipment.ShippingMethodId,
                SubTotal = shipment.SubTotal,
                ShippingTax = shipment.ShippingTax,
                ShippingDiscountAmount = shipment.ShippingDiscountAmount,
                ShipmentTrackingNumber = shipment.ShipmentTrackingNumber,
                WarehouseCode = shipment.WarehouseCode,
                ShippingAddressId = shipment.ShippingAddressId,
                ShippingMethodName = shipment.ShippingMethodName,
                PrevStatus = shipment.PrevStatus,
                PickListId = shipment.PickListId,
                Properties = shipment.ToPropertyList()
            };
        }
    }
}