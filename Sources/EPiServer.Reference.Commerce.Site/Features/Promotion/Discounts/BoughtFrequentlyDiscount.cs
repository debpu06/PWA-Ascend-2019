using EPiServer.Commerce.Marketing;
using EPiServer.DataAnnotations;

namespace EPiServer.Reference.Commerce.Site.Features.Promotion.Discounts
{
    [ContentType(GUID = "9FD9515E-CFEB-452F-BC98-6B3D4673BA66", GroupName = "entrypromotion", Order = 10300)]
    [ImageUrl("Images/BuyQuantityGetItemDiscount.png")]
    public class BoughtFrequentlyDiscount : EntryPromotion
    {
        public virtual int Days { get; set; }

        public virtual int NumberofPurchases { get; set; }

        public virtual MonetaryReward Reward { get; set; }
    }
}