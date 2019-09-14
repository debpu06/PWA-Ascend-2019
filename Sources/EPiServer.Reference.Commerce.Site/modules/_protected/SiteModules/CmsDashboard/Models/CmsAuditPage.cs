using EPiServer.Personalization.VisitorGroups;
using System;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Models
{
    public class CmsAuditPage
    {
        public List<SiteAudit> Sites { get; set; }
        public List<ContentTypeAudit> ContentTypes { get; set; }

        public List<VGAudit> VisitorGroups { get; set; }
        public DateTime VGLastRunTime { get; set; }

        public CmsAuditPage()
        {
            Sites = new List<SiteAudit>();
        }

    }
}