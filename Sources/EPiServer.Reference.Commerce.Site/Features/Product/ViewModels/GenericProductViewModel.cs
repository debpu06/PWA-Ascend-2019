using EPiServer.Reference.Commerce.Site.Features.Product.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public class GenericProductViewModel : ProductViewModelBase<GenericProduct, GenericVariant>
    {
        public GenericProductViewModel()
        {

        }

        public GenericProductViewModel(GenericProduct genericProduct) : base(genericProduct)
        {

        }
    }
}