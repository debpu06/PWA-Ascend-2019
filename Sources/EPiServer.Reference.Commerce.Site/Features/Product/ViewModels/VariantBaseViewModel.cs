using EPiServer.Reference.Commerce.Site.Features.Product.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public class VariantBaseViewModel : EntryViewModelBase<VariantBase>
    {
        public VariantBaseViewModel()
        {

        }

        public VariantBaseViewModel(VariantBase variantBase) : base(variantBase)
        {

        }
    }
}