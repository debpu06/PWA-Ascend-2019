using EPiServer.Core;
using EPiServer.Personalization.CMS.Model;
using EPiServer.Reference.Commerce.Shared;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class ContentRecommendationsBlockViewModel
    {
        public List<ContentRecommendationItem> ContentRecommendationItems { get; set; } 
    }

    public class ContentRecommendationItem
    {
        public SitePageData Content { get; set; }
        public string ContentUrl { get; set; }
        public string ContentType { get; set; }
    }
}