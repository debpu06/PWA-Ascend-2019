using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content
{
    public interface IContentService
    {
        ContentModel Get(ContentReference contentReference, string language, string properties = "*");
        ContentModel Get(Guid guid, string language, string properties = "*");
        IEnumerable<ContentModel> GetByContentType(int contentTypeId, string language, string properties = "*", string keyword = "");
        bool UpdateContent(ContentModel contentModel, string properties, out string message);
        IEnumerable<object> GetContentTypes(string type);
        IEnumerable<object> GetProperties(int id);
        IEnumerable<object> GetLanguages();
    }
}
