using EPiServer.Reference.Commerce.Site.Features.Product.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public class FashionProductViewModel : ProductViewModelBase<FashionProduct, FashionVariant>
    {
        public FashionProductViewModel()
        {

        }

        public FashionProductViewModel(FashionProduct fashionProduct) : base(fashionProduct)
        {

        }


    }
}