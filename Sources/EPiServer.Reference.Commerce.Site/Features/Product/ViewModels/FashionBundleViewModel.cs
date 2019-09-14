using EPiServer.Reference.Commerce.Site.Features.Product.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public class FashionBundleViewModel : BundleViewModelBase<FashionBundle>
    {
        public FashionBundleViewModel()
        {

        }

        public FashionBundleViewModel(FashionBundle fashionBundle) : base(fashionBundle)
        {

        }
    }
}