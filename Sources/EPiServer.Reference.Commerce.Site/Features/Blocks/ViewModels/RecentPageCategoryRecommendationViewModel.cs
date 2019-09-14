using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Mosey.Models.Pages;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class RecentPageCategoryRecommendationViewModel : BlockViewModel<RecentPageCategoryRecommendation>
    {

        public RecentPageCategoryRecommendationViewModel(RecentPageCategoryRecommendation currentBlock) : base(currentBlock)
        {
        }

        public List<CategoryViewModel> CategoryPages { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ArticlePage Page { get; set; }
    }
}