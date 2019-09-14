using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Models
{
    public class VGAudit
    {
        #region Dynamic Properties

        public string UsagesSummary =>
            $"{PageCount} page(s), {BlockCount} block(s), {Usages.Count} properties";

        #endregion

        public string Name { get; set; }
        public int CriteriaCount { get; set; }
        public Guid Id { get; set; }
        public List<VisitorGroupUse> Usages { get; set; }

        public int PageCount
        {
            get { return Usages.Where(u => u.ContentType == "Page").Count(); }
        }
        public int BlockCount
        {
            get { return Usages.Where(u => u.ContentType == "Block").Count(); }
        }

        public IEnumerable<ContentReference> ContentItems
        {
            get
            {
                return Usages.Select(vu => vu.Content).Distinct();
            }
        }

        public IEnumerable<string> PropertyNamesForUse(ContentReference cr)
        {
            return Usages.Where(u => u.Content == cr).Select(u => u.PropertyName);
        }

    }
}
