using EPiServer.Commerce.Marketing;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Campaign.Services;
using EPiServer.Reference.Commerce.Site.Features.Promotion.Services;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Start.ViewModels;
using EPiServer.Tracking.Commerce;
using EPiServer.Tracking.PageView;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Customers;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Start.Controllers
{
    public class StartController : PageController<BaseStartPage>
    {
        private readonly IPromotionService _promotionService;
        private readonly ICampaignService _campaignService;

        public StartController(IPromotionService promotionService, ICampaignService campaignService)
        {
            _promotionService = promotionService;
            _campaignService = campaignService;
        }

        [CommerceTracking(TrackingType.Home)]
        public ViewResult Index(BaseStartPage currentPage)
        {
            var viewModel = new StartPageViewModel(currentPage)
            {
                Promotions = _promotionService.GetActivePromotions(),
                Recommendations = this.GetHomeRecommendations()
            };
            
            return View(viewModel);
        }

        protected virtual ContentReference GetCampaignRoot()
        {
            return SalesCampaignFolder.CampaignRoot;
        } 
        
        [HttpPost]
        public string Subscribe(string email)
        {
            _campaignService.AddNewRecipient(email);
            return "Thanks for your subscribe";
        }
    }
}