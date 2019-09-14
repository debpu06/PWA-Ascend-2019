namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CommerceDashboard.Models
{
    /// <summary>Purchase Order is the actual recorded sale.</summary>
    public class PurchaseOrder : OrderGroup
    {
        /// <summary>
        /// An order number used for tracking the order.
        /// </summary>
        public string OrderNumber { get; set; }
    }
}