using System.Collections.Generic;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore
{
    public class BlacklistItems
    {
        [JsonProperty("items")]
        public List<BlacklistModel> BlacklistList { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
    }

    public class BlacklistModel
    {
        public string BlacklistId { get; set; }
        public string Email { get; set; }
        public string Scope { get; set; }
    }
}