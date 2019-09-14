using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Models
{
    public class UpdateContentModel
    {
        public IEnumerable<ContentModel> Contents { get; set; }
        public string Properties { get; set; }
    }
}