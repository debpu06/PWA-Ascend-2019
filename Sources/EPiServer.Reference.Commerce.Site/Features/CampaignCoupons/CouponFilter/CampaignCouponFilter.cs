using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Marketing;
using EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.Discount;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Coupons;
using EPiServer.Security;
using Mediachase.Commerce.Security;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.CouponFilter
{
    /// <summary>
    /// Used to filter out promotions that use Episerver Campaign to issue coupon codes
    /// </summary>
    public class CampaignCouponFilter : ICouponFilter
    {
        private readonly ICampaignCoupons _campaignCoupons;
        private readonly UniqueCouponService _couponService;

        public CampaignCouponFilter(ICampaignCoupons campaignCoupons, 
            UniqueCouponService couponService)
        {
            _campaignCoupons = campaignCoupons;
            _couponService = couponService;
        }

        public PromotionFilterContext Filter(PromotionFilterContext filterContext, IEnumerable<string> couponCodes)
        {
            var codes = couponCodes.ToList();
            var userEmail = PrincipalInfo.CurrentPrincipal?.GetCustomerContact()?.Email;

            foreach (var includedPromotion in filterContext.IncludedPromotions)
            {
                // Check if this is the right promotion type, if not ignore
                var couponDrivenDiscount = includedPromotion as ICampaignCouponDiscount;
                if (couponDrivenDiscount == null)
                {
                    var couponCode = includedPromotion.Coupon.Code;
                    var uniqueCodes = _couponService.GetByPromotionId(includedPromotion.ContentLink.ID);
                    if (string.IsNullOrEmpty(couponCode) && uniqueCodes?.Any() != true)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(couponCode))
                    {
                        CheckSingleCoupon(filterContext, codes, couponCode, includedPromotion);
                    }
                    else
                    {
                        CheckMultipleCoupons(filterContext, codes, includedPromotion, uniqueCodes);
                    }

                    
                    continue;
                }

                // If we don't have any codes to check or the users email then unique  
                // coupon code promotion types are not valid so exclude them
                if (!codes.Any() || string.IsNullOrEmpty(userEmail))
                {
                    filterContext.ExcludePromotion(includedPromotion, FulfillmentStatus.CouponCodeRequired,
                        filterContext.RequestedStatuses.HasFlag(RequestFulfillmentStatus.NotFulfilled));
                    continue;
                }

                long couponBlockId;
                if (long.TryParse(couponDrivenDiscount.CouponType, out couponBlockId))
                {
                    foreach (var couponCode in codes)
                    {
                        // Check if the code its assigned to the user and that has not been used
                        if (_campaignCoupons.IsCouponAssigned(userEmail, couponBlockId, couponCode) &&
                            _campaignCoupons.IsCouponUsed(couponBlockId, couponCode) == false)
                        {
                            // The code hasn't been used and is assigned to the user so we can  
                            // allow this to be connected to the promotion for this user 
                            filterContext.AddCouponCode(includedPromotion.ContentGuid, couponCode);
                        }
                        else
                        {
                            // Exclude this promotion as its been used and/or it isn't assigned to the user
                            filterContext.ExcludePromotion(includedPromotion, FulfillmentStatus.CouponCodeRequired,
                                filterContext.RequestedStatuses.HasFlag(RequestFulfillmentStatus.NotFulfilled));
                        }
                    }
                }
            }

            return filterContext;
        }

        protected virtual IEqualityComparer<string> GetCodeEqualityComparer()
        {
            return StringComparer.OrdinalIgnoreCase;
        }

        private void CheckSingleCoupon(PromotionFilterContext filterContext, IEnumerable<string> couponCodes, string couponCode, PromotionData includedPromotion)
        {
            if (couponCodes.Contains(couponCode, GetCodeEqualityComparer()))
            {
                filterContext.AddCouponCode(includedPromotion.ContentGuid, couponCode);
            }
            else
            {
                filterContext.ExcludePromotion(
                    includedPromotion,
                    FulfillmentStatus.CouponCodeRequired,
                    filterContext.RequestedStatuses.HasFlag(RequestFulfillmentStatus.NotFulfilled));
            }
        }

        private void CheckMultipleCoupons(PromotionFilterContext filterContext, IList<string> couponCodes, PromotionData includedPromotion, List<UniqueCoupon> uniqueCoupons)
        {
            foreach (var couponCode in uniqueCoupons)
            {
                // Check if the code its assigned to the user and that has not been used
                if (couponCodes.Contains(couponCode.Code, GetCodeEqualityComparer()) && couponCode.UsedRedemptions < couponCode.MaxRedemptions)
                {
                    filterContext.AddCouponCode(includedPromotion.ContentGuid, couponCode.Code);
                    return;
                }
            }
            filterContext.ExcludePromotion(includedPromotion, FulfillmentStatus.CouponCodeRequired,
                filterContext.RequestedStatuses.HasFlag(RequestFulfillmentStatus.NotFulfilled));
        }
    }
}