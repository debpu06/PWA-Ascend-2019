var CampaignDashboard = {
    init: function () {
        $(document).ready(function () {
            CampaignDashboard.loadData();
        });
    },
    //campaign-emails-table
    loadData: function () {
        $.ajax({
            url: "/campaigndashboard/GetRecipientListQuantity",
            type: "GET",
            success: function (result) {
                $('.campaign-recipient-list').text(result);
            },
        });

        $.ajax({
            url: "/campaigndashboard/GetEmailsSentQuantity",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.EmailName + '</td>';
                    html += '<td>' + item.EmailQuantity + '</td>';
                    html += '<td>' + item.ClickCount + '</td>';
                    html += '</tr>;'
                });
                $('.campaign-emails-table tbody').append(html);
            },
        });
    }
}