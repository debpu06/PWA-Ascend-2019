var CmsDashboard = { 
    init: function () {
        $(document).ready(function () {
            CmsDashboard.loadData();
        });
    },

    loadData: function () {
        $.ajax({
            url: "/cmsdashboard/GetBounceRates",
            type: "GET",
            success: function (result) {
                $('.bounce-rates').text(result[0].Value);
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.Key + '</td>';
                    html += '<td>' + item.Value + '</td>';
                    html += '</tr>;'
                });

                $('.bouce-rates-table tbody').append(html);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetVisitors",
            type: "GET",
            success: function (result) {
                $('.visitors-stats').text(result[0].Value);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetAmountOfFormsSubmitted",
            type: "GET",
            success: function (result) {
                $('.amount-of-forms-submitted').text(result[0].Value);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetVisitorGroupAudit",
            type: "GET",
            success: function (result) {
                result.sort(function (a, b) {
                    return b.Usages.length - a.Usages.length;
                });
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.Name + '</td>';
                    html += '<td>' + item.PageCount + ' page(s), ' + item.BlockCount + ' block(s)</td>';
                    html += '</tr>;'
                });
                    
                $('.visitor-group-table tbody').append(html);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetPageTypeAudits",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.Name + '</td>';
                    html += '<td>' + item.UsagesSummary + '</td>';
                    html += '</tr>;'
                });

                $('.page-type-table tbody').append(html);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetBlockTypeAudits",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.Name + '</td>';
                    html += '<td>' + item.UsagesSummary + '</td>';
                    html += '</tr>;'
                });

                $('.block-type-table tbody').append(html);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetTopLandingPages",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.Key + '</td>';
                    html += '<td>' + item.Value + '</td>';
                    html += '</tr>;'
                });

                $('.top-landing-pages-table tbody').append(html);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetAbTestContents",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.Title + '</td>';
                    html += '</tr>;'
                });

                $('.top-AB-testing-table tbody').append(html);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetAbTestingMostParticipationPercentage",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.ABTestingName + '</td>';
                    html += '<td>' + item.ParticipationPercentage + '</td>';
                    html += '</tr>;'
                });

                $('.AB-participation-percentage-table tbody').append(html);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetAbTestingMostPageViews",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.ABTestingName + '</td>';
                    html += '<td>' + item.Views + '</td>';
                    html += '</tr>;'
                });

                $('.AB-most-views-table tbody').append(html);
            },
        });

        $.ajax({
            url: "/cmsdashboard/GetAbTestingMostConversions",
            type: "GET",
            success: function (result) {
                var html = "";
                $.each(result, function (index, item) {
                    html += '<tr>';
                    html += '<td>' + (index + 1) + '</td>';
                    html += '<td>' + item.ABTestingName + '</td>';
                    html += '<td>' + item.Conversions + '</td>';
                    html += '</tr>;'
                });

                $('.AB-most-conversions-table tbody').append(html);
            },
        });
    }
}