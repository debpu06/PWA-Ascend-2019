using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Stores.ViewModels;
using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public class ProductTileViewModel : IProductModel
    {
        public int ProductId { get; set; }
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }
        public string Url { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public Money? DiscountedPrice { get; set; }
        public Money PlacedPrice { get; set; }
        public string Code { get; set; }
        public bool IsAvailable { get; set; }
        public ReviewStatisticsViewModel ReviewStatistics { get; set; }
        public bool OnSale { get; set; }
        public bool NewArrival { get; set; }
        public StoreViewModel Stores { get; set; }
        public bool IsFeaturedProduct { get; set; }
        public bool IsBestBetProduct { get; set; }
        public bool ShowRecommendations { get; set; }
        public string FirstVariationCode { get; set; }
    }
}