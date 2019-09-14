using System.Collections.Generic;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore
{
    public class SegmentItems
    {
        [JsonProperty("items")]
        public List<SegmentModel> SegmentList { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
    }

    public class SegmentModel
    {
        public string SegmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Scope { get; set; }
        public string SegmentManager { get; set; }
        public string ProfileQuery { get; set; }
        public bool AvailableForPersonalization { get; set; }
    }
}