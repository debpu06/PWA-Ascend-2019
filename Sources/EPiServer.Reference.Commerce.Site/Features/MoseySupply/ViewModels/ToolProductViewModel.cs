using EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models;
using EPiServer.Reference.Commerce.Site.Features.Product.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public class ToolProductViewModel : ProductViewModelBase<ToolProduct, ToolVariant>
    {
        public ToolProductViewModel()
        {

        }

        public ToolProductViewModel(ToolProduct toolProduct) : base(toolProduct)
        {

        }
    }
}