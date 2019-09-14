using EPiServer.Tracking.Commerce.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Tracking
{
    public class PageVisitEvent
    {
        public string PageName { get; set; }
        public string PageType { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public Task<TrackingResponseData> Response { get; set; }
    }
}