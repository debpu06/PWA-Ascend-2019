using System.Collections.Generic;
using EPiServer.Security;
using EPiServer.Shell;
using EPiServer.Shell.Navigation;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.PowerBi
{
    [MenuProvider]
    public class BiMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var powerbi = new SectionMenuItem("Power Bi", "/global/powerbi")
            {
                IsAvailable = (request) => PrincipalInfo.CurrentPrincipal.IsInRole("CommerceAdmins"),
                SortIndex = 400
            };

            var ga = new UrlMenuItem("Google Analytics",
                "/global/powerbi/google", 
                $"{Paths.ToResource("SiteModules", "bireports")}/index/google")
            {
                SortIndex = 13,
                IsAvailable = (request) => PrincipalInfo.CurrentPrincipal.IsInRole("CommerceAdmins")
            };

            var advance = new UrlMenuItem("Marketing",
                "/global/powerbi/marketing",
                $"{Paths.ToResource("SiteModules", "bireports")}/index/marketing")
            {
                SortIndex = 11,
                IsAvailable = (request) => PrincipalInfo.CurrentPrincipal.IsInRole("CommerceAdmins")
            };

            var perform = new UrlMenuItem("Commerce",
                "/global/powerbi/commerce",
                $"{Paths.ToResource("SiteModules", "bireports")}/index/commerce")
            {
                SortIndex = 10,
                IsAvailable = (request) => PrincipalInfo.CurrentPrincipal.IsInRole("CommerceAdmins")
            };

            var segment = new UrlMenuItem("Segments",
                "/global/powerbi/commerce",
                $"{Paths.ToResource("SiteModules", "bireports")}/index/segments")
            {
                SortIndex = 12,
                IsAvailable = (request) => PrincipalInfo.CurrentPrincipal.IsInRole("CommerceAdmins")
            };

            return new MenuItem[]
            {
                powerbi,
                perform,
                advance,
                ga,
                segment
            };
        }
    }
}
