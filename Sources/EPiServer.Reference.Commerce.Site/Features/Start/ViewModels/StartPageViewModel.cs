using EPiServer.Personalization.Commerce.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Promotion.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Start.ViewModels
{
    public class StartPageViewModel : ContentViewModel<BaseStartPage>
    {
        public StartPageViewModel(BaseStartPage startPage)
        {
            CurrentContent = startPage;
        }

        public IEnumerable<PromotionViewModel> Promotions { get; set; }
        public IEnumerable<Recommendation> Recommendations { get; set; }
    }
}