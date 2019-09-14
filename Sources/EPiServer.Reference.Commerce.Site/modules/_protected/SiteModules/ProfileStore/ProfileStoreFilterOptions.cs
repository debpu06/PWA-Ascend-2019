﻿using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore
{
    public class ProfileStoreFilterOptions
    {
        public KeyValuePair<string, string> Filter { get; set; }
        public int Skip { get; set; }
        public int Top { get; set; }
        public KeyValuePair<string, string> OrderBy { get; set; }
        public OrderDirection OrderDirection { get; set; }
    }

    public enum OrderDirection
    {
        ASC, DESC 
    };
}