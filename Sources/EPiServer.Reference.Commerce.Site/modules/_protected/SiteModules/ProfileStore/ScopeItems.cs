using System.Collections.Generic;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore
{
    public class ScopeItems
    {
        [JsonProperty("items")]
        public List<ScopeModel> ScopeList { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
    }

    public class ScopeModel
    {
        public string ScopeId { get; set; }
        public string Description { get; set; }
    }
}