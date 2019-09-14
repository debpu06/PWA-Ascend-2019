using System.Collections.Generic;
using EPiServer.Security;
using EPiServer.Shell.Navigation;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignMenu
{
    [MenuProvider]
    public class CampaignMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var section = new SectionMenuItem("Campaign", "/global/campaign")
            {
                IsAvailable = (request) => PrincipalInfo.HasEditAccess
            };
            var campaignPortal = new UrlMenuItem("Portal", "/global/campaign/portal", "https://www.campaign.episerver.net/action/workbench/workbench?showWorkbench=")
            {
                Target = "_blank",
                IsAvailable = (request) => PrincipalInfo.HasEditAccess
            };

            return new MenuItem[] { section, campaignPortal };
        }
    }
}