var Cart = {
    init: function () {

        $(document)
            .on('keypress', '.jsChangeCartItem', Cart.preventSubmit)
            .on('click', '.jsRemoveCartItem', Cart.removeCartItem)
            .on('change', '.jsChangeCartItem', Cart.changeCartItem)
            .on('click', '.jsAddToCart', Cart.addCartItem)
            .on('click', '.jsBuyNow', Cart.buyNow)
            .on('click', '.jsSubscription', Cart.subscription)
            .on('click', '.jsCartRequestQuote', Cart.requestQuote)
            .on('click', '.jsCartLoadOrder', Cart.loadOrder)
            .on('click', '.jsClearQuotedCart', Cart.clearQuotedCart)
            .on('click', '.jsWishListQuote', Cart.wishListQuote)
            .on('click', '.jsApproveOrder', Cart.approveOrder)
            .on('click', '.jsCreateReturn', Cart.createReturn)
            .on('change', '#MiniCart', function() { $("#MiniCartResponsive").html($(this).html()); })
            .on('change', '#WishListMiniCart', function() { $("#WishListMiniCartResponsive").html($(this).html()); })
            .on('click',
                '.jsCartContinueShopping',
                function() {
                    if ($(this).closest('#cart-dropdown')) {
                        $(this).closest('#cart-dropdown').collapse('hide');
                    }
                })
            .on('click',
                '.jsWishListContinueShopping',
                function() {
                    if ($(this).closest('#wishlist-dropdown')) {
                        $(this).closest('#wishlist-dropdown').collapse('hide');
                    }
                })
            .on('click',
                '.jsCartDropdown',
                function(e) {
                    return ($(e.target).hasClass('btn') || $(e.target).parent().is('a'));
                })
            .on('click', '.link-wishlist1 ', Cart.addToWishlist);
            //.on("change", ".jsCartChangeCountry", Cart.setRegion);;

            $('.cart-dropdown').on('show.bs.dropdown', function (e) {
                if ($('#CartItemCount', $(this)).val() == 0) {
                    e.preventDefault();
                }
            });

            $('#returnSettingModal').on('show.bs.modal', function (e) {

                var btnClose = $(e.currentTarget).find('button[id="btnCloseDlg"]');

                $(btnClose).attr("data-order-link", $(e.relatedTarget).data('order-link'));
                $(btnClose).attr("data-shipment-link", $(e.relatedTarget).data('shipment-link'));
                $(btnClose).attr("data-lineItem-link", $(e.relatedTarget).data('lineitem-link'));
                $(btnClose).attr("data-total-return", $(e.relatedTarget).data('total-return'));

                var txtQuantity = $(e.currentTarget).find('input[id="txtQuantity"]');
                $(txtQuantity).val(parseInt($(e.relatedTarget).data('total-return')));

            });

    },
    changeCartItem: function (e) {

        e.preventDefault();
        var form = $(this).closest("form");
        var quantity = $("#quantity", form).val();

        if (parseInt(quantity, 10) < 0) {
            return;
        }

        var formContainer = $("#" + form.data("container"));
        $.ajax({
            type: "POST",
            url: form[0].action,
            data: form.serialize(),
            success: function (result) {

                formContainer.html($(result));
                $('.cartItemCountLabel', formContainer.parent()).text($('#CartItemCount', formContainer).val());
                $('.cartTotalAmountLabel', formContainer.parent()).text($('#CartTotalAmount', formContainer).val());

                formContainer.change();

                if (formContainer.is($('#WishListMiniCart'))) {
                    if (result.indexOf('list-group-item') === -1) {
                        $('.delete-wishlist').hide();
                    }
                    // If items where removed from the wishlist cart from the wishlist view, they should be removed from the view.
                    var wishListAction = form.closest(".wishlist-actions");
                    if (wishListAction.length > 0) {
                        wishListAction.parent().remove();
                    }
                }
            }
        });

    },
    removeCartItem: function (e) {

        e.preventDefault();
        var form = $(this).closest('form');
        $("#quantity", form).val(0).change();
        var formContainer = $("#" + form.data("container"));
        $.ajax({
            type: "POST",
            url: form[0].action,
            data: form.serialize(),
            success: function(result) {
                formContainer.html($(result));
            }
        });


    },
    addCartItem: function (e) {

        e.preventDefault();
        var form = $(this).closest("form");
        var formContainer = $("#" + form.data("container"));
        var skuCode = $("#code", form).val();
        var $this = $(this);
        var formUrl = $this.data("formurl");
        var url = form[0].action;
        if (formUrl) {
            url = formUrl;
        }
        $("#CartWarningMessage").hide();
        $(".warning-message", $("#CartWarningMessage")).html("");

        $this.addClass('is-loading');

        $.ajax({
            type: "POST",
            url: url,
            data: form.serialize(),
            success: function (result) {
                formContainer.html($(result));
                $('.cartItemCountLabel', formContainer.parent()).text($('#CartItemCount', formContainer).val());
                $('.cartTotalAmountLabel', formContainer.parent()).text($('#CartTotalAmount', formContainer).val());
                formContainer.change();

                $this.removeClass('is-loading');

                if ($('#ModalDialog').is(':visible')) {
                    $('#ModalDialog').find('.close').trigger('click');
                }
            },
            error: function (xhr, status, error) {
                $(".warning-message", $("#CartWarningMessage")).html(xhr.statusText);
                $("#CartWarningMessage").show();
            }
        });
    },
    preventSubmit: function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();
        }
    },
    addToWishlist: function (e) {
        e.preventDefault();
        var form = $(this).closest("form");
        var skuCode = $("#code", form).val();

        $.ajax({
            type: "POST",
            url: $(this).data("url"),
            data: { code: skuCode },
            success: function (result) {
                $("#CheckoutView").replaceWith($(result));
            }
        });
    },
    requestQuote: function (e) {
        var form = $(this).closest("form");
        $.ajax({
            type: "POST",
            url: form[0].action,
            success: function (result) {
                $("#CartQuoteSucceedMessage").show();
                window.location.reload();
            },
            error: function (xhr, status, error) {
                $(".warning-message", $("#CartWarningMessage")).html(xhr.statusText);
                $("#CartWarningMessage").show();
            }
        });
    },
    wishListQuote: function (e) {
        var form = $(this).closest("form");
        $.ajax({
            type: "POST",
            url: form[0].action,
            complete: function (result) {
                window.location.reload();
            }
        });
    },
    loadOrder: function (e) {
        var form = $(this).closest("form");
        var orderLink = e.currentTarget.getAttribute("data-order-link");
        $.ajax({
            type: "POST",
            url: form[0].action,
            data: { orderLink: orderLink },
            success: function (result) {
                window.location = result.link;
            },
            error: function (xhr, status, error) {
                $(".warning-message", $("#CartWarningMessage")).html(xhr.statusText);
                $("#CartWarningMessage").show();
            }
        });
    },
    clearQuotedCart: function (e) {
        var form = $(this).closest("form");
        $.ajax({
            type: "POST",
            url: form[0].action,
            success: function (result) {
                window.location.reload();
            },
            error: function (xhr, status, error) {
                $(".warning-message", $("#CartWarningMessage")).html(xhr.statusText);
                $("#CartWarningMessage").show();
            }
        });
    },
    approveOrder: function (e) {
        var form = $(this).closest("form");
        var orderLink = e.currentTarget.getAttribute("data-order-link");
        $.ajax({
            type: "POST",
            url: form[0].action,
            data: { orderGroupId: orderLink },
            success: function (result) {
                if (result.result == true)
                    window.location = $(".cancelUpdateBudget").attr('href');
                else {
                    $("#BudgetWarningMessage").show();
                    $(".warning-message-data").text(result.result);
                }
            }
        });
    },
    buyNow: function (e) {

        e.preventDefault();
        var form = $(this).closest("form");
        var formContainer = $("#" + form.data("container"));
        var skuCode = $("#code", form).val();

        $("#CartWarningMessage").hide();
        $(".warning-message", $("#CartWarningMessage")).html("");

        $.ajax({
            type: "POST",
            url: '/cart/buynow',
            data: form.serialize(),
            success: function (result) {

                window.location = '/en/checkout/order-confirmation/?contactId=' + result.contactId + '&orderNumber=' + result.orderNumber;
            },
            error: function (xhr, status, error) {
                $(".warning-message", $("#CartWarningMessage")).html(xhr.statusText);
                $("#CartWarningMessage").show();
            }
        });
    },
    subscription: function (e) {

        e.preventDefault();
        var form = $(this).closest("form");
        var formContainer = $("#" + form.data("container"));
        var skuCode = $("#code", form).val();

        $("#CartWarningMessage").hide();
        $(".warning-message", $("#CartWarningMessage")).html("");

        $.ajax({
            type: "POST",
            url: '/cart/subscription',
            data: form.serialize(),
            success: function (result) {

                formContainer.html($(result));
                $('.cartItemCountLabel', formContainer.parent()).text($('#CartItemCount', formContainer).val());
                $('.cartTotalAmountLabel', formContainer.parent()).text($('#CartTotalAmount', formContainer).val());
                formContainer.change();

                $this.removeClass('is-loading');
            },
            error: function (xhr, status, error) {
                $(".warning-message", $("#CartWarningMessage")).html(xhr.statusText);
                $("#CartWarningMessage").show();
            }
        });
    },
    createReturn: function (e) {

        var form = $(this).closest("form");
        var orderLink = parseInt(e.currentTarget.getAttribute("data-order-link"));
        var shipmentLink = parseInt(e.currentTarget.getAttribute("data-shipment-link"));
        var lineItemLink = parseInt(e.currentTarget.getAttribute("data-lineItem-link"));
        var quantity = Math.min(parseInt(e.currentTarget.getAttribute("data-total-return")), parseInt($(".modal-content #txtQuantity").val()));
        var reason = $("#optReason option:selected").text();
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: form[0].action,
            data: { orderGroupId: orderLink, shipmentId: shipmentLink, lineItemId: lineItemLink, returnQuantity: quantity, reason: reason },
            success: function (result) {
                if (result.result == true) {
                    window.location = $(".cancelUpdateBudget").attr('href');
                    $(e.currentTarget).closest("td").prev().html(result.ReturnFormStatus);
                }
                else {
                    $(".warning-message-data").text(result.result);
                }
            }
        });

    }
   

};

