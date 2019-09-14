using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Personalization.Commerce.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Social.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Stores.ViewModels;
using Mediachase.Commerce;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public abstract class EntryViewModelBase<T> : ContentViewModel<T> where T : EntryContentBase
    {
        protected EntryViewModelBase()
        {

        }

        protected EntryViewModelBase(T currentContent) : base(currentContent)
        {

        }

        public Injected<UrlResolver> UrlResolver { get; set; }

        public IList<string> Images { get; set; }
        public IEnumerable<Recommendation> AlternativeProducts { get; set; }
        public IEnumerable<Recommendation> CrossSellProducts { get; set; }
        public StoreViewModel Stores { get; set; }
        public ReviewsViewModel Reviews { get; set; }
        public IEnumerable<ProductTileViewModel> StaticAssociations { get; set; }
        public bool HasOrganization { get; set; }
        public List<string> ReturnedMessages { get; set; }
        public Money? DiscountedPrice { get; set; }
        public Money ListingPrice { get; set; }
        public Money? SubscriptionPrice { get; set; }
        public Money? MsrpPrice { get; set; }
        public Money? MapPrice { get; set; }
        public bool IsAvailable { get; set; }
        public bool ShowRecommendations { get; set; }
        public bool IsSalesRep { get; set; }
        public List<MediaData> SalesMaterials { get; set; }
        public List<MediaData> Documents { get; set; }
    }
}