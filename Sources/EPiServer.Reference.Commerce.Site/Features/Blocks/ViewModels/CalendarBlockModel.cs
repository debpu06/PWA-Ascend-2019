using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels
{
    public class CalendarEventBlockModel
    {
        public CalendarEventBlockModel(CalendarEventBlock block)
        {
            ViewMode = block.ViewMode;

        }
        public string ViewMode { get; set; }
        public IEnumerable<PageData> Events { get; set; }
    }
}
