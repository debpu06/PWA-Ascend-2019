using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore
{
    public class TrackEventItems
    {
        [JsonProperty("items")]
        public List<TrackEventModel> TrackEventList { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
    }

    public class TrackEventModel
    {
        public string TrackId { get; set; }
        public string DeviceId { get; set; }
        public string EventType { get; set; }
        public string EventTime { get; set; }
        public string Value { get; set; }
        public string Scope { get; set; }
        public string CountryCode { get; set; }
        public string PageUri { get; set; }
        public string PageTitle { get; set; }
        public string RemoteAddress { get; set; }
        public JObject Payload { get; set; }
        public Dictionary<string, string> User { get; set; }
    }

    public class VisualizationItems
    {
        [JsonProperty("items")]
        public List<VisualizationModel> VisualizationModels { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
    }

    public class VisualizationModel
    {
        public string DeviceId { get; set; }
        public string EventType { get; set; }
        public string EventTime { get; set; }
        public string Value { get; set; }
        public string CountryCode { get; set; }
        public object User { get; set; }
    }
}