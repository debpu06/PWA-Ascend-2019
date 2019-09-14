using System.Collections.Generic;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class BoughtFrequentlyViewModel : IBlockViewModel<BoughtFrequentlyBlock>
    {
        public BoughtFrequentlyViewModel(BoughtFrequentlyBlock boughtFrequentlyBlock)
        {
            CurrentBlock = boughtFrequentlyBlock;
            Entries = new Dictionary<MonetaryReward, List<EntryContentBase>>();
        }

        public BoughtFrequentlyBlock CurrentBlock { get; }

        public Dictionary<MonetaryReward, List<EntryContentBase>> Entries { get; set; }
    }
}