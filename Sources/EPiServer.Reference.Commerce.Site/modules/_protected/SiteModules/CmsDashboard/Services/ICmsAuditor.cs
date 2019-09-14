using System;
using System.Collections.Generic;
using EPiServer.Web;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Models;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Services
{
    public interface ICmsAuditor
    {
        List<ContentTypeAudit> GetContentTypesOfType<T>();

        List<VGAudit> GetVisitorGroups();

        List<ContentTypeAudit> GetContentItemsOfTypes(List<ContentTypeAudit> contentTypes,
            bool includeReferences, bool includeParentDetail);

        void PopulateContentItemsOfType(ContentTypeAudit contentTypeAudit,
            bool includeReferences, bool includeParentDetail);

        ContentTypeAudit GetContentTypeAudit(int contentTypeId, bool includeReferences, bool includeParentDetail);

        SiteAudit GetSiteAudit(Guid siteGuid);

        List<SiteDefinition> GetSiteDefinitions();
    }
}