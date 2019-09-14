using EPiServer.Find.UnifiedSearch;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Models.Pages;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Alloy.ViewModels
{
    public class FindPageViewModel : ContentViewModel<FindPage>
    {
        public FindPageViewModel()
        {
            
        }

        public FindPageViewModel(FindPage currentPage): base(currentPage)
        {
        }

        public UnifiedSearchResults Results { get; set; }
        public string SearchedQuery { get; set; }
        public int NumberOfHits { get; set; }
    }
}