var Recommendations = {
    init: function () {
        $(".recommendations").find("[data-recommendation-id]").on("click", ".item-img-info > a.quickview-btn", Recommendations.clickTracking);
        $(".recommendations").find("[data-recommendation-id]").on("mouseup", ".item-img-info > a.product-image", Recommendations.clickTracking);
        $(".recommendations").find("[data-recommendation-id]").on("contextmenu", ".product > a", Recommendations.keyboardHandle);
        $(".recommendations").find("[data-recommendation-id]").on("keydown", ".product > a, .product img", Recommendations.keyboardHandle);
    },

    clickTracking: function (evt) {
        var recommendationId = $(evt.delegateTarget).data("recommendation-id");
        Misc && Misc.setCookie("EPiServer_Commerce_RecommendationId", recommendationId, 60);//set lifetime of this cookie to only 1 minute.
    },

    keyboardHandle: function (evt) {
        if ((evt.type == "keydown" && evt.which == 13) || (evt.type == "contextmenu" && evt.which != 3)) {
            Recommendations.clickTracking(evt); //handle keyup event of enter key, contextmenu event of menu key.
        }
    },

    getRecommendations: function (widget, numberOfRecs, name, value) {
        $.ajax({
            type: "POST",
            url: '/RecommendationWidgetBlock/GetRecommendations',
            data: {
                widgetType: widget,
                numberOfRecs: numberOfRecs,
                name: name,
                value:value
            },
            success: function (result) {
                $('.recommendationwidgetblock ').html(result);
            },
        });
    }
};