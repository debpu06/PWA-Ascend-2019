using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Models
{
    public class ABTestingModel
    {
        public string ABTestingName { get; set; }
        public int ParticipationPercentage { get; set; }
        public int Views { get; set; }
        public double Conversions { get; set; }
    }
}