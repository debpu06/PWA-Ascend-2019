using System;
using System.Collections.Generic;
using EPiServer.Core;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models
{
    public class ContentModel
    {
        public ContentReference ContentReference { get; set; }
        public ContentModelReference ContentLink { get; set; }
        public Guid ContentGuid { get; set; }
        public string Name { get; set; }
        public LanguageModel Language { get; set; }
        public List<LanguageModel> ExistingLanguages { get; set; }
        public LanguageModel MasterLanguage { get; set; }
        public List<string> ContentType { get; set; }
        public ContentModelReference ParentLink { get; set; }
        public string RouteSegment { get; set; }
        public string Url { get; set; }
        public DateTime? Changed { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? StartPublish { get; set; }
        public DateTime? StopPublish { get; set; }
        public DateTime? Saved { get; set; }
        public string Status { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }
}