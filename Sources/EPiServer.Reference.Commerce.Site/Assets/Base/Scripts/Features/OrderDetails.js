var OrderDetails = {
    init: function () {

        $(document)
            .on('change', '.jsChangeDetailsPrice', OrderDetails.changePrice)
            .on('change', '.jsChangeDetailsQuantity', OrderDetails.changeQuantity)
            .on('click', '.jsChangeDetailsNote', OrderDetails.addNote)
            .on('click', '.jsCreateReturn', Cart.createReturn);
    },

    changePrice: function (e) {
        e.preventDefault();
        var form = $(this).closest("form");
        var formContainer = $("#" + form.data("container"));
        $.ajax({
            type: "POST",
            url: form[0].action,
            data: form.serialize(),
            success: function (result) {
                formContainer.html($(result));
            }
        });
    },
    changeQuantity: function (e) {
        e.preventDefault();
        var form = $(this).closest("form");
        var formContainer = $("#" + form.data("container"));
        $.ajax({
            type: "POST",
            url: form[0].action,
            data: form.serialize(),
            success: function (result) {
                formContainer.html($(result));
            }
        });
    },
    addNote: function (e) {
        e.preventDefault();
        var form = $(this).closest("form");
        var formContainer = $("#" + form.data("container"));
        $.ajax({
            type: "POST",
            url: form[0].action,
            data: form.serialize(),
            success: function (result) {
                formContainer.html($(result));
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

