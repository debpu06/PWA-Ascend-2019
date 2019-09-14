var CommerceDashboard = {
    init: function () {
        $(document).ready(function () {
            CommerceDashboard.loadData();
        });
    },

    loadData: function () {
        $.ajax({
            url: "/commercedashboard/GetRevenues",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result[0].Revenues, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.Currency + '</td>';
                    html += '<td>' + item.Revenue + '</td>';
                    html += '</tr>;';
                });
                $('.revenues-table tbody').append(html);
            }
        });

        $.ajax({
            url: "/commercedashboard/GetProductsSold",
            type: "GET",
            success: function (result) {
                $('.products-sold-data').text(result[0].ProductsSold);
            }
        });

        $.ajax({
            url: "/commercedashboard/GetNewCustomers",
            type: "GET",
            success: function (result) {
                $('.new-customers-data').text(result[0].NewCustomers);
            }
        });

        $.ajax({
            url: "/commercedashboard/GetCarts",
            type: "GET",
            success: function (result) {
                $('.cart-data').text(result.result);
            }
        });

        $.ajax({
            url: "/commercedashboard/GetTopProducts",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result[0].Products, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.Product.Code + '</td>';
                    html += '<td>' + item.Product.DisplayName + '</td>';
                    html += '<td>' + item.Count + '</td>';
                    html += '</tr>;';
                });

                $('.top-products-table tbody').append(html);
            }
        });

        $.ajax({
            url: "/commercedashboard/GetTopPromotions",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.Promotion + '</td>';
                    html += '<td>' + item.DiscountType + '</td>';
                    html += '<td>' + item.TotalRedemptions + '</td>';
                    html += '</tr>;';
                });

                $('.top-promotions-table tbody').append(html);
            }
        });
    }
};