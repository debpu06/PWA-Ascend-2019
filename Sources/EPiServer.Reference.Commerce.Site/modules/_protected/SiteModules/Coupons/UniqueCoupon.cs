﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Coupons
{
    public class UniqueCoupon
    {
        public long Id { get; set; }
        public int PromotionId { get; set; }
        public string Code { get; set; }
        public DateTime Valid { get; set; }
        public DateTime? Expiration { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime Created { get; set; }
        public int MaxRedemptions { get; set; }
        public int UsedRedemptions { get; set; }
    }
}