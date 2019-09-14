var Checkout = {
    init: function () {
        $(document)
            .on('change', '.jsChangePayment', Checkout.changePayment)
            .on('click', '.jsRemovePayment', Checkout.removePayment)
            .on('change', '.jsChangeShipment', Checkout.changeShipment)
            .on('change', '.jsChangeAddress', Checkout.changeAddress)
            .on('change', '.jsChangeTaxAddress', Checkout.changeTaxAddress)
            .on('change', '#MiniCart', Checkout.refreshView)
            .on('click', '.jsNewAddress', Checkout.newAddress)
            .on('click', '#AlternativeAddressButton', Checkout.enableShippingAddress)
            .on('click', '.remove-shipping-address', Checkout.removeShippingAddress)
            .on('click', '.js-add-couponcode', Checkout.addCouponCode)
            .on('click', '.js-remove-couponcode', Checkout.removeCouponCode)
            .on('change', '.jsSingleAddress', Checkout.toggleAddress)
            .on('change', '.jsBillingAddress', Checkout.toggleBillingAddress)
            .on('click', '.jsSelectCreditCard', Checkout.toggleNewCreditCard)
            .on('click', '.button-billing', Checkout.validateBillingInformation)
            .on('click', '.button-shipping-single-address', Checkout.validateShippingSingleAddress);

        Checkout.initializeAddressAreas();
    },

    initializeAddressAreas: function () {
        if ($("#UseBillingAddressForShipment").val() == "False") {
            Checkout.doEnableShippingAddress();
        }
        else {
            Checkout.doRemoveShippingAddress();
        }
    },

    addCouponCode: function (e) {
        e.preventDefault();
        var couponCode = $('#coupon_code').val();
        if (couponCode.trim()) {
            $.ajax({
                type: "POST",
                url: $(this).data("url"),
                data: { couponCode: couponCode },
                success: function (result) {
                    if (!result) {
                        $('.couponcode-errormessage').show();
                        return;
                    }
                    $('.couponcode-errormessage').hide();
                    $("#CheckoutView").replaceWith($(result).find("#CheckoutView"));
                    Checkout.initializeAddressAreas();
                }
            });
        }
    },
    removeCouponCode: function (e) {
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: $(this).attr("href"),
            data: { couponCode: $(this).siblings().text() },
            success: function (result) {
                $("#CheckoutView").replaceWith($(result).find("#CheckoutView"));
                Checkout.initializeAddressAreas();
            }
        });
    },
    refreshView: function () {

        var view = $("#CheckoutView");

        if (view.length == 0) {
            return;
        }
        var url = view.data('url');
        $.ajax({
            cache: false,
            type: "GET",
            url: view.data('url'),
            success: function (result) {
                view.replaceWith($(result).find("#CheckoutView"));
                Checkout.initializeAddressAreas();
            }
        });
    },
    newAddress: function (e) {
        e.preventDefault();
        AddressBook.showNewAddressDialog($(this));
    },
    changeAddress: function () {

        var form = $('.jsCheckoutForm');
        var id = $(this).attr("id");
        var isBilling = id.indexOf("Billing") > -1;
        if (isBilling) {
            $("#ShippingAddressIndex").val(-1);
        } else {
            $("#ShippingAddressIndex").val($(".jsChangeAddress").index($(this)) - 1);
        }

        $.ajax({
            type: "POST",
            cache: false,
            url: $(this).closest('.jsCheckoutAddress').data('url'),
            data: form.serialize(),
            success: function (result) {
                if (isBilling) {
                    $("#billingAddressContainer").html($(result));
                } else {
                    $("#AddressContainer").html($(result));
                }
                Checkout.initializeAddressAreas();
                Checkout.updateOrderSummary();
            }
        });
    },

    changeTaxAddress: function () {
        var id = $(this).attr("id");
        if ((id.indexOf("Billing") > -1) && $("#UseBillingAddressForShipment").val() == "False") {
            return;
        }
        var form = $('.jsCheckoutForm');
        if (form.length == 0) {
            return;
        }
        $.ajax({
            type: "POST",
            cache: false,
            url: $(this).closest('.jsCheckoutAddress').data('url'),
            data: form.serialize(),
            success: function (result) {
                Checkout.updateOrderSummary();
            }
        });
    },

    changePayment: function () {
        var form = $('.jsCheckoutForm');
        $.ajax({
            type: "POST",
            url: form.data("updateurl"),
            data: form.serialize(),
            success: function (result) {
                $('.jsPaymentMethod').replaceWith($(result).find('.jsPaymentMethod'));
                //Checkout.updateOrderSummary();
                //Misc.updateValidation('jsCheckoutForm');
            }
        });
    },

    removePayment: (e) => {
        e.preventDefault();
        let paymentMethodRow = $(e.currentTarget).closest('tr');
        let systemKeyword = $(e.currentTarget).data('system-keyword');
        let url = $(e.currentTarget).data('url');
        let paymentOption = {
            SystemKeyword: systemKeyword
        };

        $.ajax({
            type: "POST",
            url: url,
            data: paymentOption,
            success: (result) => {
                $("#billingInformationView").replaceWith($(result));
            }
        });
    },

    changeShipment: function () {
        var form = $('.jsCheckoutForm');
        $.ajax({
            type: "POST",
            url: form.data("updateurl"),
            data: form.serialize(),
            success: function (result) {
                Checkout.updateOrderSummary();
            }
        });
    },

    updateOrderSummary: function () {
        $.ajax({
            cache: false,
            type: "GET",
            url: $('.jsOrderSummary').data('url'),
            success: function (result) {
                $('.jsOrderSummary').replaceWith($(result).filter('.jsOrderSummary'));
            }
        });
    },
    doEnableShippingAddress: function () {
        $("#AlternativeAddressButton").hide();
        $(".shipping-address:hidden").slideToggle(300);
        $(".shipping-address").css("display", "block");
        $("#UseBillingAddressForShipment").val("False");
    },
    enableShippingAddress: function (event) {

        event.preventDefault();

        Checkout.doEnableShippingAddress();

        var form = $('.jsCheckoutForm');
        $("#ShippingAddressIndex").val(0);

        $.ajax({
            type: "POST",
            cache: false,
            url: $('.jsCheckoutAddress').data('url'),
            data: form.serialize(),
            success: function (result) {
                $("#AddressContainer").html($(result));
                Checkout.initializeAddressAreas();
                Checkout.updateOrderSummary();
            }
        });
    },
    doRemoveShippingAddress: function () {
        $("#AlternativeAddressButton").show();
        $(".shipping-address:visible").slideToggle(300);
        $(".shipping-address").css("display", "none");
        $("#UseBillingAddressForShipment").val("True");
    },
    removeShippingAddress: function (event) {

        event.preventDefault();

        Checkout.doRemoveShippingAddress();

        var form = $('.jsCheckoutForm');
        $("#ShippingAddressIndex").val(-1);

        $.ajax({
            type: "POST",
            cache: false,
            url: $('.jsCheckoutAddress').data('url'),
            data: form.serialize(),
            success: function (result) {
                Checkout.initializeAddressAreas();
                Checkout.updateOrderSummary();
            }
        });
    },
    toggleAddress: function () {
        var addressType = $(this).val();
        $.ajax({
            type: "GET",
            url: $(this).data("url") + '?addressType=' + addressType,
            success: function (result) {
                $("#singleAddressView").html($(result));
            }
        });
    },
    toggleBillingAddress: function () {
        var addressType = $(this).val();
        $.ajax({
            type: "GET",
            url: $(this).data("url") + '?addressType=' + addressType,
            success: function (result) {
                $("#billingInformationView").html($(result));
            }
        });
    },
    toggleNewCreditCard: function (event) {
        var type = $(this).val();
        $(".selectCreditCardType").css("display", "none");
        $("#" + type).css("display", "block");

        var usedSelected = type == "avaiableCreditCard" ? "true" : "false";
        $("#UseSelectedCreditCard").attr("data-value", usedSelected);
        $("#UseSelectedCreditCard").attr("Value", usedSelected);
    },
    addPayment: function (url) {
        $(".jsAddPayment").addClass('is-loading');
        var form = $('.jsCheckoutForm');
        $.ajax({
            type: "POST",
            data: form.serialize(),
            url: url,
            success: function (result) {
                $("#billingInformationView").replaceWith($(result));
                $(".jsAddPayment").removeClass('is-loading');
            }
        });
    },
    validateBillingInformation: function () {
        if ($(".billing-address").val() == undefined) {
            $(".jsCheckoutForm").submit();
        } else if ($(".billing-address :selected").val().length === 0) {
            $(".address-required-message").show();
        }
        else {
            $(".jsCheckoutForm").submit();
        }
    },
    validateShippingSingleAddress: function (e) {
        if ($(".shipping-single-address :selected").val().length === 0) {
            $(".address-required-message").show();
            e.preventDefault();
        }
    }
};