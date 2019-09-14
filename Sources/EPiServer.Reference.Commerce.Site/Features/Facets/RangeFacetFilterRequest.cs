using EPiServer.Find.Api.Querying;
using System.Collections.Generic;
using EPiServer.Find.Api.Facets;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.Features.Facets
{
    [JsonConverter(typeof(RangeFacetRequestConverter))]
    public class RangeFacetFilterRequest : FacetFilterRequest
    {
        public RangeFacetFilterRequest(string name, Filter facetFilter)
            : base(name, facetFilter)
        {
            Ranges = new List<NumericRange>();
        }

        [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }

        [JsonProperty("key_field", NullValueHandling = NullValueHandling.Ignore)]
        public string KeyField { get; set; }

        [JsonProperty("value_field", NullValueHandling = NullValueHandling.Ignore)]
        public string ValueField { get; set; }

        [JsonProperty("ranges")]
        public List<NumericRange> Ranges { get; }
    }
}