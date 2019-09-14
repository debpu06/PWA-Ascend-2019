using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams
{
    public class ReviewActivity
    {
        public int Rating { get; set; }
        public double OverallRating { get; set; }
        public string Contributor { get; set; }
        public string Product { get; set; }
    }
}
 