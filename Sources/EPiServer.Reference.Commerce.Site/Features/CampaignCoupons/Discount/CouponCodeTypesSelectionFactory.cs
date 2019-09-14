using System.Collections.Generic;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.Discount
{
    public class CouponCodeTypesSelectionFactory : ISelectionFactory
    {
        private readonly ICampaignCouponDefinition _campaignCouponDefinition;

        public CouponCodeTypesSelectionFactory()
        {
            _campaignCouponDefinition = ServiceLocator.Current.GetInstance<ICampaignCouponDefinition>();
        }

        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            var allCouponDefinitions = _campaignCouponDefinition.GetAllCouponDefinitions();
            var couponItems = new List<ISelectItem>();
            foreach (var couponDefinition in allCouponDefinitions)
            {
                couponItems.Add(new SelectItem()
                {
                    Text = couponDefinition.CouponName + " (" + couponDefinition.CouponSource + ")",
                    Value = couponDefinition.CouponId.ToString()
                });
            }
            return couponItems;
        }
    }
}