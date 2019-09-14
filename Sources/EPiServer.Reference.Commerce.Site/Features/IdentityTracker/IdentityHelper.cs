using EPiServer.Personalization.VisitorGroups.Internal;
using EPiServer.Personalization.VisitorGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.IdentityTracker
{
    public static class IdentityHelper
    {
        public static IEnumerable<string> ActiveVisitorGroups()
        {

            var repository = new VisitorGroupStore();
            var list = repository.List();
            var helper = new VisitorGroupHelper();

            foreach (var g in list)
            {
                if (helper.IsPrincipalInGroup(System.Threading.Thread.CurrentPrincipal, g.Name))
                {
                    yield return g.Name;
                }

            }
        }

        public static string GuessedLocation()
        {
            var local=GeoPosition.GetUsersLocation();
            if (local != null)
            {
                return local.Region + ", " + local.CountryCode + ", " + local.ContinentCode;
            }
            return "N/A";
        }
    }
}