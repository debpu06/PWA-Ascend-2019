using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Bookmark.Models;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Customers;
using Newtonsoft.Json;
using EPiServer.Web.Routing;
using EPiServer.Commerce.Catalog.ContentTypes;
using Mediachase.Commerce.Catalog;

namespace EPiServer.Reference.Commerce.Site.Features.Bookmark.Services
{
    [ServiceConfiguration(typeof(IBookmarksService))]
    public class BookmarksService : IBookmarksService
    {
        private readonly IContentLoader _contentLoader;
        private readonly IUrlResolver _urlResolver;
        private readonly ReferenceConverter _referenceConverter;
        private readonly IContentRepository _contentRepository;


        public BookmarksService(IContentLoader contentLoader,
            IUrlResolver urlResolver,
            ReferenceConverter referenceConverter,
            IContentRepository contentRepository)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _referenceConverter = referenceConverter;
            _contentRepository = contentRepository;
        }

        public void Add(int contentId)
        {
            var currentContact = CustomerContext.Current.CurrentContact;
            if (currentContact != null)
            {
                var contentReference = new ContentReference(contentId);
                var contact = new BookmarksContact(currentContact);
                var bookmarkModel = new BookmarkModel();
                if (_contentLoader.TryGet<IContent>(contentReference, out IContent content))
                {
                    bookmarkModel.ContentId = contentId;
                    bookmarkModel.ContentGuid = content.ContentGuid;
                    bookmarkModel.Name = content.Name;
                    bookmarkModel.Url = _urlResolver.GetUrl(contentReference);
                }
                else
                {
                    var productLink = _referenceConverter.GetContentLink(contentId, CatalogContentType.CatalogEntry, 0);
                    var productContent = _contentLoader.Get<CatalogContentBase>(productLink);
                    bookmarkModel.ContentId = contentId;
                    bookmarkModel.ContentGuid = productContent.ContentGuid;
                    bookmarkModel.Name = productContent.Name;
                    bookmarkModel.Url = _urlResolver.GetUrl(productLink);
                }
                var contactBookmarks = contact.ContactBookmarks;
                if (contactBookmarks.FirstOrDefault(x => x.ContentGuid == bookmarkModel.ContentGuid) == null)
                {
                    contactBookmarks.Add(bookmarkModel);
                }
                contact.Bookmarks = JsonConvert.SerializeObject(contactBookmarks);
                contact.SaveChanges();
            }
        }

        public List<BookmarkModel> Get()
        {
            var currentContact = CustomerContext.Current.CurrentContact;
            if (currentContact != null)
            {
                var contact = new BookmarksContact(currentContact);
                return contact.ContactBookmarks;
            }
            return new List<BookmarkModel>();
        }

        public void Remove(Guid contentGuid)
        {
            var currentContact = CustomerContext.Current.CurrentContact;
            if (currentContact != null)
            {
                var contact = new BookmarksContact(currentContact);
                var contactBookmarks = contact.ContactBookmarks;
                var content = contactBookmarks.FirstOrDefault(x => x.ContentGuid == contentGuid);
                contactBookmarks.Remove(content);
                contact.Bookmarks = contactBookmarks.Any() ? JsonConvert.SerializeObject(contactBookmarks) : "";
                contact.SaveChanges();
            }
        }
    }
}