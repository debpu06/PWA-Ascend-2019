var Coupons = {
    updateCouponRecord(record) {
        var id = $('#Coupons_' + record + '__Id').val();
        var promotionId = $('#Coupons_' + record + '__PromotionId').val();
        var code = $('#Coupons_' + record + '__Code').val();
        var created = $('#Coupons_' + record + '__Created').val();
        var expiration = $('#Coupons_' + record + '__Expiration').val();
        var maxRedemptions = $('#Coupons_' + record + '__MaxRedemptions').val();
        var usedRedemptions = $('#Coupons_' + record + '__UsedRedemptions').val();
        var valid = $('#Coupons_' + record + '__Valid').val();
        $.ajax({
            type: 'POST',
            url: "/SingleUseCoupon/UpdateCoupon",
            data: JSON.stringify(
                {
                    Id:id,
                    PromotionId: promotionId,
                    Code: code,
                    Created: created,
                    Valid : valid,
                    Expiration:expiration,
                    MaxRedemptions: maxRedemptions,
                    UsedRedemptions: usedRedemptions
            }),
            contentType: 'application/json',
            success: function (result) {
                window.location = '/EPiServer/SiteModules/SingleUseCoupon/EditPromotionCoupons/' + result;
            }
        });
    },

    deleteCouponRecord(id, promotionId) {
        $.ajax({
            type: 'POST',
            url: "/SingleUseCoupon/DeleteCoupon",
            data: { id: id, promotionId: promotionId },
            success: function (result) {
                window.location = '/EPiServer/SiteModules/SingleUseCoupon/EditPromotionCoupons/' + result;
            }
        });
    }
}