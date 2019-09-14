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
    public class BookmarkModel
    {
        public int ContentId { get; set; }
        public Guid ContentGuid { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}