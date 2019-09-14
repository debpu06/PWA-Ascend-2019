using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Stores.ViewModels;
using Mediachase.Commerce;
namespace EPiServer.Reference.Commerce.Site.Features.Shared.Models
{
    public interface IProductModel
    {
        int ProductId { get; set; }
        string Brand { get; set; }
        string Code { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        string LongDescription { get; set; }
        Money? DiscountedPrice { get; set; }
        string ImageUrl { get; set; }
        Money PlacedPrice { get; set; }
        string Url { get; set; }
        bool IsAvailable { get; set; }
        ReviewStatisticsViewModel ReviewStatistics { get; set; }
        bool OnSale { get; set; }
        bool NewArrival { get; set; }
        StoreViewModel Stores { get; set; }
        bool IsFeaturedProduct { get; set; }
        bool IsBestBetProduct { get; set; }
    }
}
