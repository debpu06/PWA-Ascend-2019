using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Models;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Services
{
    [ServiceConfiguration(typeof(ICmsAuditor))]
    public class CmsAuditor : ICmsAuditor
    {
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentModelUsage _contentModelUsage;
        private readonly IContentRepository _contentRepository;
        private readonly ISiteDefinitionResolver _siteDefinitionResolver;
        private readonly ISiteDefinitionRepository _siteDefinitionRepository;
        private readonly IVisitorGroupRepository _vgRepo;
        private readonly IScheduledJobRepository _scheduledJobRepo;

        public CmsAuditor(IContentTypeRepository contentTypeRepository, IContentModelUsage contentModelUsage,
            IContentRepository contentRepository, ISiteDefinitionResolver siteDefinitionResolver,
            ISiteDefinitionRepository siteDefinitionRepository, IVisitorGroupRepository vgRepo, IScheduledJobRepository scheduledJobRepo)
        {
            _contentTypeRepository = contentTypeRepository;
            _contentModelUsage = contentModelUsage;
            _contentRepository = contentRepository;
            _siteDefinitionResolver = siteDefinitionResolver;
            _siteDefinitionRepository = siteDefinitionRepository;
            _vgRepo = vgRepo;
            _scheduledJobRepo = scheduledJobRepo;
        }

        /// <summary>
        /// Returns a list of content types where "content type is T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<ContentTypeAudit> GetContentTypesOfType<T>()
        {
            return _contentTypeRepository.List()
                .Where(ct => ct is T && ct.Name != "SysRoot" && ct.Name != "SysRecycleBin")
                .Select(ct =>
                    new ContentTypeAudit
                    {
                        ContentTypeId = ct.ID,
                        Name = string.IsNullOrEmpty(ct.DisplayName) ? ct.Name : ct.DisplayName,
                        Usages = new List<ContentTypeAudit.ContentItem>()
                    })
                .ToList();
        }

        public List<VGAudit> GetVisitorGroups()
        {
            var vga = new VisitorGroupAudit();
            vga.Execute();
            Thread.Sleep(500);
            return _vgRepo.List().Select(vg => new VGAudit() { Name = vg.Name, CriteriaCount = vg.Criteria.Count, Id = vg.Id, Usages = VisitorGroupUse.ListForVisitorGroup(vg.Id.ToString()).ToList() }).Take(5).ToList();
        }

        /// <summary>
        /// Returns a list of content items that use the provided content types
        /// </summary>
        /// <param name="contentTypes">Specify the content types you want to get content items for</param>
        /// <param name="includeReferences">"True" if you want references to the content item to be included</param>
        /// <param name="includeParentDetail">"True" if you want to include basic details of the content item's parent</param>
        /// <returns></returns>
        public List<ContentTypeAudit> GetContentItemsOfTypes(List<ContentTypeAudit> contentTypes, bool includeReferences, bool includeParentDetail)
        {
            foreach (var contentTypeAudit in contentTypes)
            {
                PopulateContentItemsOfType(contentTypeAudit, includeReferences, includeParentDetail);
            }
            return contentTypes;
        }

        /// <summary>
        /// Populates a list of content items of the provided content type
        /// </summary>
        /// <param name="contentTypeAudit"></param>
        /// <param name="includeReferences"></param>
        /// <param name="includeParentDetail"></param>
        public void PopulateContentItemsOfType(ContentTypeAudit contentTypeAudit,
            bool includeReferences, bool includeParentDetail)
        {
            var contentType = _contentTypeRepository.Load(contentTypeAudit.ContentTypeId);
            var contentModelUsages = _contentModelUsage.ListContentOfContentType(contentType)
                .Where(cmu => cmu.ContentLink != ContentReference.WasteBasket
                              && !_contentRepository.GetAncestors(cmu.ContentLink).Select(ic => ic.ContentLink).Contains(ContentReference.WasteBasket))
                .Select(contentUsage => new
                {
                    ContentLink = contentUsage.ContentLink.ToReferenceWithoutVersion(),
                    contentUsage.Name
                })
                .Distinct()
                .Select(distinctContentUsage => new
                {
                    distinctContentUsage.ContentLink,
                    Name = distinctContentUsage.Name,
                    ContentItem = _contentRepository.Get<IContent>(distinctContentUsage.ContentLink)
                });


            contentTypeAudit.Usages = contentModelUsages.Select(cmu => new ContentTypeAudit.ContentItem
            {
                Name = cmu.Name,
                ContentLink = cmu.ContentLink,
                SiteId = _siteDefinitionResolver.GetByContent(cmu.ContentLink, true).Id,
                Parent = includeParentDetail && cmu.ContentItem.ParentLink != ContentReference.EmptyReference
                    ? new ContentTypeAudit.ContentItem
                    {
                        Name = _contentRepository.Get<IContent>(cmu.ContentItem.ParentLink).Name,
                        ContentLink = cmu.ContentItem.ParentLink
                    }
                    : null,
                PageReferences = includeReferences
                    ? _contentRepository.GetReferencesToContent(cmu.ContentLink, true)
                        .Select(rtc => new ContentTypeAudit.ContentItem.PageReference
                        {
                            Name = rtc.OwnerName,
                            ContentLink = rtc.OwnerID,
                            SiteId = _siteDefinitionResolver.GetByContent(rtc.OwnerID, true).Id
                        }).ToList()
                    : new List<ContentTypeAudit.ContentItem.PageReference>()
            }).ToList();
        }

        /// <summary>
        /// Gets a list of content items of the provided content type
        /// </summary>
        /// <param name="contentTypeId"></param>
        /// <param name="includeReferences"></param>
        /// <param name="includeParentDetail"></param>
        /// <returns></returns>
        public ContentTypeAudit GetContentTypeAudit(int contentTypeId, bool includeReferences, bool includeParentDetail)
        {
            var contentType = _contentTypeRepository.Load(contentTypeId);
            var contentTypeAudit = new ContentTypeAudit
            {
                ContentTypeId = contentTypeId,
                Name = contentType.Name,
                Usages = new List<ContentTypeAudit.ContentItem>()
            };

            PopulateContentItemsOfType(contentTypeAudit, includeReferences, includeParentDetail);

            return contentTypeAudit;
        }

        /// <summary>
        /// Adds the content type of the specified contentReference to the provided list of contentTypes
        /// </summary>
        /// <param name="contentReference"></param>
        /// <param name="contentTypes"></param>
        /// <param name="parentContentItem"></param>
        private void AddPageContentTypeToList(ContentReference contentReference, List<ContentTypeAudit> contentTypes, ContentTypeAudit.ContentItem parentContentItem)
        {
            var pageData = _contentRepository.Get<PageData>(contentReference);
            var newContentItem = new ContentTypeAudit.ContentItem
            {
                Name = pageData.Name,
                ContentLink = contentReference,
                Parent = parentContentItem
            };


            var knownType = contentTypes.FirstOrDefault(ct => ct.ContentTypeId == pageData.ContentTypeID);
            if (knownType == null)
            {
                var contentType = _contentTypeRepository.Load(pageData.ContentTypeID);
                contentTypes.Add(new ContentTypeAudit
                {
                    ContentTypeId = pageData.ContentTypeID,
                    Name = string.IsNullOrEmpty(contentType.DisplayName)
                        ? contentType.Name
                        : contentType.DisplayName,
                    Usages = new List<ContentTypeAudit.ContentItem> { newContentItem }
                });
            }
            else
            {
                knownType.Usages.Add(newContentItem);
            }

            var children = _contentRepository.GetChildren<PageData>(contentReference);
            foreach (var child in children)
            {
                AddPageContentTypeToList(child.ContentLink, contentTypes, newContentItem);
            }
        }

        /// <summary>
        /// Returns an audit of page/block type usages within the site
        /// </summary>
        /// <param name="siteDefo"></param>
        /// <returns></returns>
        public SiteAudit GetSiteAudit(Guid siteGuid)
        {
            var siteDefo = _siteDefinitionRepository.Get(siteGuid);
            var siteAudit = new SiteAudit
            {
                Site = siteDefo.Name
            };

            // Do start page
            AddPageContentTypeToList(siteDefo.StartPage, siteAudit.ContentTypes, null);

            return siteAudit;
        }

        /// <summary>
        /// Gets a list of all sites
        /// </summary>
        /// <returns></returns>
        public List<SiteDefinition> GetSiteDefinitions()
        {
            return _siteDefinitionRepository.List()
                .ToList();
        }
    }
}