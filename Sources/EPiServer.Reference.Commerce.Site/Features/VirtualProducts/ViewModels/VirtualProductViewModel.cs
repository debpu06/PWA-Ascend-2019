using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models;

namespace EPiServer.Reference.Commerce.Site.Features.VirtualProducts.ViewModels
{
    public class VirtualProductViewModel : ProductViewModelBase<VirtualProduct, VirtualVariant>
    {
        public VirtualProductViewModel()
        {

        }

        public VirtualProductViewModel(VirtualProduct virtualProduct) : base(virtualProduct)
        {

        }

        public bool FileUrlIsWorking { get; set; }
    }
}