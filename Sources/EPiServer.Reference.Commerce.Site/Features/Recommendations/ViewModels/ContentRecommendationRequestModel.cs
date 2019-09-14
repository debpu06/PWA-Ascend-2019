namespace EPiServer.Reference.Commerce.Site.Features.Recommendations.ViewModels
{
    public class ContentRecommendationRequestModel
    {
        public string ContentId { get; set; }
        public string SiteId { get; set; }
        public string LanguageId { get; set; }
        public int NumberOfRecommendations { get; set; }
    }
}