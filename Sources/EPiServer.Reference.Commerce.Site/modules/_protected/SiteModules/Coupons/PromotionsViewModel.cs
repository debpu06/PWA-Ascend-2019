using EPiServer.Commerce.Marketing;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Coupons
{
    public class PromotionsViewModel
    {
        public PromotionsViewModel()
        {
            Promotions = new List<PromotionData>();
            PagingInfo = new PagingInfo();
        }

        public PagingInfo PagingInfo { get; set; }
        public List<PromotionData> Promotions { get; set; }
    }
}