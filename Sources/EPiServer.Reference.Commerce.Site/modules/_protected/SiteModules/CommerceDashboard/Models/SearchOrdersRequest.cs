using System;
using Mediachase.Commerce.Orders;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models
{
    /// <summary>
    /// Order search parameter model.
    /// </summary>
    public class SearchOrdersRequest
    {
        /// <summary>
        /// Order shipment status to filter.
        /// Valid values:
        /// - AwaitingInventory
        /// - Cancelled
        /// - InventoryAssigned
        /// - OnHold
        /// - Packing
        /// - Released
        /// - Shipped
        /// </summary>
        public OrderShipmentStatus OrderShipmentStatus { get; set; }

        /// <summary>
        /// Shipping method ID (GUID) to filter.
        /// </summary>
        public Guid? ShippingMethodId { get; set; }

        /// <summary>
        /// Modified from date to filter.
        /// </summary>
        public DateTime? ModifiedFrom { get; set; }

        /// <summary>
        /// Array of statuses to filter.
        /// Valid values:
        /// - AwaitingExchange
        /// - Cancelled
        /// - Completed
        /// - InProgress
        /// - OnHold
        /// - PartiallyShipped
        /// </summary>
        public string[] Status { get; set; }
    }
}