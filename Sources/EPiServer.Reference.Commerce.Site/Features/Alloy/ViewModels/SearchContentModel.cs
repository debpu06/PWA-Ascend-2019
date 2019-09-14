using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels
{
    public class SearchContentModel : ContentViewModel<AlloySearchPage>
    {
        public SearchContentModel()
        {
            
        }
        public SearchContentModel(AlloySearchPage currentPage) : base(currentPage)
        {
        }

        public bool SearchServiceDisabled { get; set; }
        public string SearchedQuery { get; set; }
        public int NumberOfHits { get; set; }
        public IEnumerable<SearchHit> Hits { get; set; }  
    }
}
