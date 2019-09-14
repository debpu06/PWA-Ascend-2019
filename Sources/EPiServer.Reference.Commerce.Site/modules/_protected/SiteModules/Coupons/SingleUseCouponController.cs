using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.Navigation;
using System.Web.Mvc;
using EPiServer.Commerce.Marketing;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Reference.Commerce.Site.Features.CampaignCoupons.Discount;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Coupons
{
    public class SingleUseCouponController : Controller
    {
        private readonly IContentLoader _contentLoader;
        private readonly UniqueCouponService _couponService;

        public SingleUseCouponController(IContentLoader contentLoader, 
            UniqueCouponService couponService)
        {
            _contentLoader = contentLoader;
            _couponService = couponService;
        }

        [MenuItem("/global/commerce/coupons", TextResourceKey = "/Shared/Coupons")]
        [HttpGet]
        public ActionResult Index()
        {

            var promotions = GetPromotions(_contentLoader.GetDescendents(GetCampaignRoot()))
                .Where(x => !(x is ICampaignCouponDiscount))
                .ToList();

            return View(new PromotionsViewModel
            {
                Promotions = promotions
            });
        }

        [HttpPost]
        public ActionResult Index(PagingInfo pagingInfo)
        {
            var promotions = GetPromotions(_contentLoader.GetDescendents(GetCampaignRoot()))
                .Where(x => !(x is ICampaignCouponDiscount))
                .Skip(pagingInfo.PageNumber * pagingInfo.PageSize)
                .Take(pagingInfo.PageSize)
                .ToList();

            return View(new PromotionsViewModel
            {
                Promotions = promotions
            });
        }

        [HttpGet]
        public ActionResult EditPromotionCoupons(int id)
        {
            var promotion = _contentLoader.Get<PromotionData>(new ContentReference(id));
            var coupons = _couponService.GetByPromotionId(id);

            return View(new PromotionCouponsViewModel
            {
                Coupons = coupons ?? new List<UniqueCoupon>(),
                Promotion = promotion,
                PromotionId = promotion.ContentLink.ID,
                MaxRedemptions = 1,
                ValidFrom = DateTime.Now
            });
        }

        [HttpPost]
        public ActionResult UpdateCoupon(UniqueCoupon model)
        {
            var coupon = _couponService.GetById(model.Id);
            if (coupon != null)
            {
                coupon.Code = model.Code;
                coupon.Expiration = model.Expiration;
                coupon.MaxRedemptions = model.MaxRedemptions;
                coupon.UsedRedemptions = model.UsedRedemptions;
                coupon.Valid = model.Valid;
                _couponService.SaveCoupons(new List<UniqueCoupon> {coupon});
            }

            return new ContentResult
            {
                Content = model.PromotionId.ToString()
            };
        }

        [HttpPost]
        public ActionResult DeleteCoupon(long id, int promotionId)
        {
            _couponService.DeleteById(id);
            return new ContentResult
            {
                Content = promotionId.ToString()
            };
        }

        [HttpPost]
        public ActionResult Generate(PromotionCouponsViewModel model)
        {
            var couponRecords = new List<UniqueCoupon>();
            for (var i = 0; i < model.Quantity; i++)
            {
                couponRecords.Add(new UniqueCoupon
                {
                    Code = _couponService.GenerateCoupon(),
                    Created = DateTime.UtcNow,
                    Expiration = model.Expiration,
                    MaxRedemptions = model.MaxRedemptions,
                    PromotionId = model.PromotionId,
                    UsedRedemptions = 0,
                    Valid = model.ValidFrom
                });
            }

            _couponService.SaveCoupons(couponRecords);
            return RedirectToAction("EditPromotionCoupons", new { id = model.PromotionId });
        }

        private ContentReference GetCampaignRoot()
        {
            return _contentLoader.GetChildren<SalesCampaignFolder>(ContentReference.RootPage)
                .FirstOrDefault()?.ContentLink ?? ContentReference.EmptyReference;

        }

        private List<PromotionData> GetPromotions(IEnumerable<ContentReference> references)
        {
            return _contentLoader.GetItems(references, ContentLanguage.PreferredCulture)
                .OfType<PromotionData>()
                .ToList();
        }
    }
}