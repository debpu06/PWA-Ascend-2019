using EPiServer.Reference.Commerce.Site.Features.Editorial.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.Editorial.ViewModels
{
    public class StandardPageViewModel : ContentViewModel<StandardPage>
    {
        public StandardPageViewModel()
        {
            
        }
        public StandardPageViewModel(StandardPage currentPage)
            : base(currentPage)
        {
            
        }

        public bool IsAlloy { get; set; }
    }
}