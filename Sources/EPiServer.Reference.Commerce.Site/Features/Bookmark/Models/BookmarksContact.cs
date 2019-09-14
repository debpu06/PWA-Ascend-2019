using EPiServer.Core;
using EPiServer.Reference.Commerce.B2B.Extensions;
using Mediachase.Commerce.Customers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Bookmark.Models
{
    public class BookmarksContact
    {
        public BookmarksContact(CustomerContact contact)
        {
            Contact = contact;
        }

        public CustomerContact Contact { get; }

        public string Bookmarks
        {
            get { return Contact.GetStringValue("Bookmarks"); }
            set { Contact["Bookmarks"] = value; }
        }

        public List<BookmarkModel> ContactBookmarks {
            get
            {
                var bookmarks = string.IsNullOrWhiteSpace(Bookmarks) ? new List<BookmarkModel>() : JsonConvert.DeserializeObject<List<BookmarkModel>>(Bookmarks);
                return bookmarks;
            }
        }

        public void SaveChanges()
        {
            Contact.SaveChanges();
        }
    }
}