using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class RssReaderBlockViewModel
    {
        public RssReaderBlock CurrentBlock { get; set; }

        public XhtmlString DescriptiveText { get; set; }
        public bool HasHeadingText { get; set; }
        public string Heading { get; set; }
        public List<RssItem> RssList { get; set; }

        public class RssItem
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public string PublishDate { get; set; }
        }
    }
}