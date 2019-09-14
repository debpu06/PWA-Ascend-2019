namespace EPiServer.Reference.Commerce.Site.Features.Warehouses.Models
{
    /// <summary>
    /// Model is used to determine nearest warehouse location.
    /// </summary>
    public class DistanceData
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string WarehouseCode { get; set; }
        public double DistanceValue { get; set; }
    }
}