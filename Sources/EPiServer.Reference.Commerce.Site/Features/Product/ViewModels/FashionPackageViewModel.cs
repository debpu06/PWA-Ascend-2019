using EPiServer.Reference.Commerce.Site.Features.Product.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public class FashionPackageViewModel : PackageViewModelBase<FashionPackage>
    {
        public FashionPackageViewModel()
        {

        }

        public FashionPackageViewModel(FashionPackage fashionPackage) : base(fashionPackage)
        {
        }

    }
}