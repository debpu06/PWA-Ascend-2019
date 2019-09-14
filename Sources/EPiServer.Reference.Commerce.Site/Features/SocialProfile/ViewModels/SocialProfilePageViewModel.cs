using EPiServer.Reference.Commerce.Site.Features.Loyalty.Models;
using EPiServer.Reference.Commerce.Site.Features.OrderHistory.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.Comments;
using EPiServer.Reference.Commerce.Site.Features.SocialProfile.Pages;
using EPiServer.Social.Comments.Core;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.SocialProfile.ViewModels
{
    public class SocialProfilePageViewModel : ContentViewModel<SocialProfilePage>
    {
        public SocialProfilePageViewModel(SocialProfilePage profilePage) : base(profilePage)
        {
            Comments = new List<Comment>();
            Feeds = new List<CommunityFeedItemViewModel>();
        }
        public string User { get; set; }
        public LoyaltyContact LoyaltyContact { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public List<OrderViewModel> Orders { get; set; }
        public IEnumerable<CommunityFeedItemViewModel> Feeds { get; set; }
        public string TierUrl { get; set; }
    }
}