using System.Linq;
using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models;
using LineItem = Mediachase.Commerce.Orders.LineItem;
using OrderForm = Mediachase.Commerce.Orders.OrderForm;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Mappings
{
    internal static class ShipmentMappings
    {
        public static void ConvertToShipment(
            this Shipment shipmentDto,
            Mediachase.Commerce.Orders.Shipment shipment,
            OrderForm orderForm)
        {
            shipment.Status = shipmentDto.Status;
            shipment.ShippingMethodId = shipmentDto.ShippingMethodId;
            shipment.SubTotal = shipmentDto.SubTotal;
            shipment.ShippingTax = shipmentDto.ShippingTax;
            shipment.ShippingDiscountAmount = shipmentDto.ShippingDiscountAmount;
            shipment.ShipmentTrackingNumber = shipmentDto.ShipmentTrackingNumber;
            shipment.WarehouseCode = shipmentDto.WarehouseCode;
            shipment.ShippingAddressId = shipmentDto.ShippingAddressId;
            shipment.ShippingMethodName = shipmentDto.ShippingMethodName;
            shipment.PrevStatus = shipmentDto.PrevStatus;
            shipment.PickListId = shipmentDto.PickListId;

            shipmentDto.MapPropertiesToModel(shipment);
            MapLineItems(shipmentDto, shipment, orderForm);
        }

        private static void MapLineItems(
            Shipment shipmentDto,
            Mediachase.Commerce.Orders.Shipment shipment,
            OrderForm orderForm)
        {
            foreach (var lineItemDto in shipmentDto.LineItems)
            {
                var lineItem = CreateLineItem(orderForm, lineItemDto);
                lineItemDto.ConvertToLineItem(lineItem);
                shipment.LineItems.Add(lineItem);
            }
        }

        private static LineItem CreateLineItem(OrderForm orderForm, Models.LineItem lineItemDto)
        {
            var existing = orderForm.LineItems.FirstOrDefault(x => x.Code == lineItemDto.Code);
            if (existing == null)
            {
                return new LineItem {Code = lineItemDto.Code};
            }

            var lineItem = new LineItem
            {
                Code = lineItemDto.Code,
                LineItemDiscountAmount =
                    CalculateItemDiscount(existing.LineItemDiscountAmount, existing.Quantity, lineItemDto.Quantity),
                OrderLevelDiscountAmount =
                    CalculateItemDiscount(existing.OrderLevelDiscountAmount, existing.Quantity, lineItemDto.Quantity)
            };

            return lineItem;
        }

        private static decimal CalculateItemDiscount(
            decimal totalItemDiscount,
            decimal totalItemQuantity,
            decimal itemQuantity)
        {
            if (totalItemQuantity == 0)
            {
                return 0;
            }
            return totalItemDiscount/totalItemQuantity*itemQuantity;
        }

        private static void ConvertToLineItem(this Models.LineItem lineItemDto, ILineItem lineItem)
        {
            var li = lineItem;
            li.DisplayName = lineItemDto.DisplayName;
            li.PlacedPrice = lineItemDto.PlacedPrice;
            li.Quantity = lineItemDto.Quantity;
            li.ReturnQuantity = lineItemDto.ReturnQuantity;
            li.InventoryTrackingStatus = lineItemDto.InventoryTrackingStatus;
            li.IsInventoryAllocated = lineItemDto.IsInventoryAllocated;
            li.IsGift = lineItemDto.IsGift;
            lineItemDto.MapPropertiesToModel(li);
        }
    }
}