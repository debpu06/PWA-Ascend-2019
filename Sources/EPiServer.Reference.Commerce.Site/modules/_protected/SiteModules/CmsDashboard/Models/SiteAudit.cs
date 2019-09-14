using System.Collections.Generic;
using EPiServer.Web;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Models
{
	public class SiteAudit
	{
		public string Site { get; set; }
		public List<ContentTypeAudit> ContentTypes { get; set; }

		public SiteAudit()
		{
			ContentTypes = new List<ContentTypeAudit>();
		}
	}
}