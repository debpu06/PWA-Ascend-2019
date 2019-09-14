using EPiServer.Commerce.Marketing;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.ViewModels
{
    public class DiscountViewModel<T> : ContentViewModel<T> where T : IContent
    {
        public DiscountViewModel()
        {
            
        }

        public DiscountViewModel(T currentContent) : base(currentContent)
        {
                
        }

        public List<ProductTileViewModel> Items { get; set; }
        public PromotionData Promotion { get; set; }
    }
}