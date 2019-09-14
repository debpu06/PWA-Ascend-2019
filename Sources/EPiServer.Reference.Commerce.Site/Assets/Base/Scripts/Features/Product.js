var ProductPage = {
    init: function () {
        $(document).on('change', '.jsVariationSwitch', ProductPage.switchVariant);
        $(document).on('click', '.quickview-button', ProductPage.showQuickview);
        $(document).on('change', '.item-quantity', ProductPage.changeTotals);
        $(document).on('click', '#AddToCartButton', ProductPage.addMultipleToCart);
        $(document).on('click', '#AddToOrderPadButton', ProductPage.addMultipleToOrderPad);

        ProductPage.resetCarousel();
        $('#product-carousel').carousel({
            interval: 6000
        });
        $('#carousel-thumbs').on("click", "a", function () {
            var thumbId = $(this).attr("id");
            var carouselThumbId = parseInt(thumbId);
            $('#product-carousel').carousel(carouselThumbId);

            return false;
        });

        $(document).on('change', '.jsVariationSwitch', ProductPage.switchVariant);

        $(document).on('change', 'input[type=radio][name=store]', (e) => {
            if (e.currentTarget.value === 'delivery') {
                $('#storesSelector').hide();
            } else {
                $('#storesSelector').show();
            }
        });

        $(document).on('click', 'li[class*="colorSwatches"]:not(.active)', (e) => {
            $('select[class*="jsVariationSwitch"][id="Color"]').val(e.currentTarget.getAttribute("color")).change();
        });

        $(document).on('click', 'li[class*="sizeSwatches"]:not(.active)', (e) => {
            $('select[class*="jsVariationSwitch"][id="Size"]').val(e.currentTarget.getAttribute("size")).change();
        });

    },
    resetCarousel: function () {
        $('.carousel-inner .item').removeClass("active");
        $('.carousel-inner .item:first-child()').addClass("active");
    },
    switchVariant: function () {
        var form = $(this).closest('form');
        $.ajax({
            type: "POST",
            url: form[0].action,
            data: form.serialize(),
            success: function (result) {
                var fetched = $(result);
                $('.jsProductDetails').replaceWith(fetched.find('.col-main').html());
                ProductPage.resetCarousel();
            },
            error: function () {
                $('.jsAddToCart button').addClass('disabled');
                alert('The variant is not available.');
            }
        });
    },
    showQuickview: function (url) {
        var skuCode = $(this).data("code");

        $("#ModalDialog").addClass('is-loading');

        $.ajax({
            type: "GET",
            cache: false,
            url: url,
            data: { variantCode: skuCode, useQuickview: true },
            success: function (result) {
                $(".modal-dialog", $("#ModalDialog")).html($(result));
                $("#ModalDialog").removeClass('is-loading');
            }
        });
    },
    changeTotals: function () {

        var totalDiscountedPrice = 0;
        var totalListingPrice = 0;

        var $currency = $(this).closest('.variant-row').find('input[name=CurrentCurrency]').val();

        $("tr.variant-row").each(function () {
            var $row = $(this);

            var $quantity = $row.find('.item-quantity');
            var $discountedPrice = $row.find('span[name*=DiscountedPrice]');
            var $listingPrice = $row.find('span[name*=ListingPrice]');

            var quantityVal = $quantity.val();

            var discountedPriceVal = parseFloat($discountedPrice.text().replace($currency, ""));
            var listingPriceVal = parseFloat($listingPrice.text().replace($currency, ""));

            if (quantityVal !== "0" && !isNaN(quantityVal)) {
                totalDiscountedPrice = (quantityVal * discountedPriceVal) + totalDiscountedPrice;
                totalListingPrice = (quantityVal * listingPriceVal) + totalListingPrice;
            }
        });

        var $totalDiscountedPrice = $('span[name*=TotalDiscountedPrice]');
        var $totalListingPrice = $('span[name*=TotalListingPrice]');
        var $youSave = $('.you-save');

        var youSaveAmount = totalListingPrice - totalDiscountedPrice;

        $totalDiscountedPrice.text($currency + totalDiscountedPrice.toFixed(2));
        if (totalListingPrice !== "0" && !isNaN(totalListingPrice)) {
            $totalListingPrice.text($currency + totalListingPrice.toFixed(2));
        }
        if (youSaveAmount > 0) {
            $youSave.text("You save: " + $currency + youSaveAmount.toFixed(2));
        }
    },
    addMultipleToCart: function (e) {
        e.preventDefault();

        var variants = [];

        $("tr.variant-row").each(function () {
            var $row = $(this);
            var $quantity = $row.find(".item-quantity");
            var $sku = $row.find(".product-sku");

            var skuVal = $sku.text();
            var quantityVal = $quantity.val();
            if (quantityVal !== "0" && !isNaN(quantityVal)) {
                variants.push(skuVal + ";" + quantityVal);
            }
        });

        $.ajax({
            type: "POST",
            url: "/Cart/AddVariantsToCart",
            data: {
                variants: variants
            },
            dataType: "json",
            success: function (result) {
                location.reload();
            }
        });
    },
    addMultipleToOrderPad: function (e) {
        e.preventDefault();

        var variants = [];

        $("tr.variant-row").each(function () {
            var $row = $(this);
            var $quantity = $row.find(".item-quantity");
            var $sku = $row.find(".product-sku");

            var skuVal = $sku.text();
            var quantityVal = $quantity.val();
            if (quantityVal !== "0" && !isNaN(quantityVal)) {
                variants.push(skuVal + ";" + quantityVal);
            }
        });

        $.ajax({
            type: "POST",
            url: "/WishList/AddVariantsToOrderPad",
            data: { variants: variants },
            dataType: "json",
            success: function (result) {
                location.reload();
            }
        });
    },    
    selectStore: function (code, name) {
        $("#selectedStore").val(code);
        $("#storeName").empty();
        $("#storeName").append(name);
    },
    setStore: function (code, name) {
        $.ajax({
            type: "POST",
            url: "/Product/SetStore",
            data: { storeCode: code },
            success: function (response) {
                $("#storeName").text(name);
                infoWindow.close();
            }
        });
    },
};