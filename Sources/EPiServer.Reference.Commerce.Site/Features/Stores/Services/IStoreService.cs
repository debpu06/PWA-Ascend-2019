using EPiServer.Reference.Commerce.Site.Features.Stores.ViewModels;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Stores.Services
{
    public interface IStoreService
    {
        List<StoreItemViewModel> GetEntryStoresViewModels(string entryCode);
        List<StoreItemViewModel> GetAllStoreViewModels();
        StoreItemViewModel GetCurrentStoreViewModel();
        bool SetCurrentStore(string storeCode);
    }
}