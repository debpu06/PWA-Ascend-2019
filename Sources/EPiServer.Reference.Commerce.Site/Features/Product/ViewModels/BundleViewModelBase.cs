using EPiServer.Commerce.Catalog.ContentTypes;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Product.ViewModels
{
    public abstract class BundleViewModelBase<TBundle> : EntryViewModelBase<TBundle> where TBundle : BundleContent
    {
        protected BundleViewModelBase()
        {

        }

        protected BundleViewModelBase(TBundle fashionBundle) : base(fashionBundle)
        {
            Bundle = fashionBundle;
        }

        public TBundle Bundle { get; set; }
        public IEnumerable<EntryContentBase> Entries { get; set; }
    }
}