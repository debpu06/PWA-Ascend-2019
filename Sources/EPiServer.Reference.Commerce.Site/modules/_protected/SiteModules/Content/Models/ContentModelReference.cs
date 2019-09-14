using System;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models
{
    public class ContentModelReference
    {
        public int? Id { get; set; }

        public int? WorkId { get; set; }

        public Guid? GuidValue { get; set; }

        public string ProviderName { get; set; }
    }
}