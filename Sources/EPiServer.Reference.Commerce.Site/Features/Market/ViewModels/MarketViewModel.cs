using EPiServer.Core;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Features.Market.ViewModels
{
    public class MarketViewModel
    {
        public IEnumerable<MarketItem> Markets { get; set; }
        public string MarketId { get; set; }
        public string CurrentMarket { get; set; }
        public ContentReference ContentLink { get; set; }
    }

    public class MarketItem
    {
        public string FlagUrl { get; set; }
        public bool Selected { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
}