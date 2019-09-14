var Visualization = {
    init: function () {
        Visualization.initData();
        $(document).on("click", ".visit", Visualization.render.chart.visit);
        $(document).on("click", ".bounced", Visualization.render.chart.bounced);
        $(document).on("click", ".pageviews", Visualization.render.chart.pageviews);
        $(document).on("click", ".page", Visualization.render.chart.page);
        $(document).on("click", ".duration", Visualization.render.chart.duration);
        $(document).on("click", ".search", Visualization.render.chart.search);
        $(".visit").focus();
    },
    data: {
        visitLabels: [],
        visitDatasets: [],
        pageLabels: [],
        pageData: [],
        pageColors: [],
        bouncedLabels: [],
        bouncedDatasets: [],
        durationLabels: [],
        durationDatasets: [],
        pageviewsLabels: [],
        pageviewsDatasets: [],
        searchLabels: [],
        searchDatasets: [],
        colors: [
            'rgb(255, 99, 132)',
            'rgb(255, 159, 64)',
            'rgb(255, 205, 86)',
            'rgb(75, 192, 192)',
            'rgb(54, 162, 235)',
            'rgb(153, 102, 255)',
            'rgb(201, 203, 207)',
            'rgb(255, 182, 193)'
        ],
        numberOfVisit: 0,
        numberOfVisitor: 0,
        avgDuration: 0,
        numberOfPageviews: 0,
        numberOfUniquePageviews: 0,
        numberOfMaxPageviews: 0,
        bouncedRate: 0,
        numberOfSearch: 0,
        numberOfKeywords: 0,
        dateRange: "",
    },
    util: {
        findObjectByKey: function (array, key, value) {
            for (var i = 0; i < array.length; i++) {
                if (array[i][key] === value) {
                    return array[i];
                }
            }
            return null;
        },
        getRandomColor: function () {
            var letters = '0123456789ABCDEF'.split('');
            var color = '#';
            for (var i = 0; i < 6; i++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            return color;
        },
        convertSecondstoMinutes: function (time) {
            var minutes = Math.floor(time / 60);
            var seconds = (time - minutes * 60).toFixed(0);
            return minutes + " min " + seconds + "s";
        }
    },
    initData: function () {
        var visitArr = [];
        var visitData = [];
        var visitorData = [];
        var bouncedAndDurationArr = [];
        var bouncedArr = [];
        var bouncedData = [];
        var durationArr = [];
        var durationData = [];
        var pageLabels = [];
        var pageData = [];
        var pageColors = [];
        var pageArr = [];
        var pageviewsData = [];
        var pageviewsArr = [];
        var uniquePageViewsData = [];
        var uniquePageViewsArr = [];
        $.ajax({
            type: 'GET',
            async: false,
            url: "/EditProfileStore/GetPageViewEvents",
            success: function (result) {
                $.each(result, function (index, item) {
                    var eventTime = new Date(item.EventTime);
                    var date = new Date(item.EventTime).toLocaleDateString();
                    var deviceId = item.DeviceId;
                    var visitor = item.User.Email ? item.User.Email : item.DeviceId;
                    var visitKey = item.DeviceId + "-" + visitor;
                    if (visitArr.length == 0) {
                        visitArr.push({ date: date, visits: [visitKey], visitors: [visitor] });
                    } else {
                        var obj = Visualization.util.findObjectByKey(visitArr, "date", date);
                        if (obj) {
                            if ($.inArray(visitKey, obj.visits) == -1) {
                                obj.visits.push(visitKey);
                            }
                            if ($.inArray(visitor, obj.visitors) == -1) {
                                obj.visitors.push(visitor);
                            }
                        } else {
                            visitArr.push({ date: date, visits: [visitKey], visitors: [visitor] });
                        }
                    }
                    var defaultItem = {
                        date: date, visits: [{
                            deviceId: deviceId,
                            count: 1,
                            firtSeen: eventTime,
                            lastSeen: eventTime,
                            duration: 0
                        }]
                    };
                    if (bouncedAndDurationArr.length == 0) {
                        bouncedAndDurationArr.push(defaultItem);
                    } else {
                        var obj = Visualization.util.findObjectByKey(bouncedAndDurationArr, "date", date);
                        if (obj) {
                            var visit = Visualization.util.findObjectByKey(obj.visits, "deviceId", deviceId);
                            if (visit) {
                                visit.count += 1;
                                visit.firtSeen = eventTime;
                                visit.duration = Math.abs((new Date(visit.lastSeen).getTime() - new Date(eventTime).getTime()) / 1000);
                            } else {
                                obj.visits.push({
                                    deviceId: deviceId,
                                    count: 1,
                                    firtSeen: eventTime,
                                    lastSeen: eventTime,
                                    duration: 0
                                })
                            }
                        } else {
                            bouncedAndDurationArr.push(defaultItem);
                        }
                    }
                    var page = item.Value.replace("Viewed ", "");
                    if (pageArr.length == 0) {
                        pageArr.push({ page: page, count: 1, color: Visualization.util.getRandomColor() });
                    } else {
                        var obj = Visualization.util.findObjectByKey(pageArr, "page", page);
                        if (obj) {
                            obj.count += 1;
                        } else {
                            pageArr.push({ page: page, count: 1, color: Visualization.util.getRandomColor() });
                        }
                    }
                    if (pageviewsArr.length == 0) {
                        pageviewsArr.push({ date: date, count: 1 });
                    } else {
                        var obj = Visualization.util.findObjectByKey(pageviewsArr, "date", date);
                        if (obj) {
                            obj.count += 1;
                        } else {
                            pageviewsArr.push({ date: date, count: 1 });
                        }
                    }
                    if (uniquePageViewsArr.length == 0) {
                        uniquePageViewsArr.push({ date: date, pages: [page] });
                    } else {
                        var obj = Visualization.util.findObjectByKey(uniquePageViewsArr, "date", date);
                        if (obj) {
                            if ($.inArray(page, obj.pages) == -1) {
                                obj.pages.push(page);
                            }
                        } else {
                            uniquePageViewsArr.push({ date: date, pages: [page] });
                        }
                    }
                });

                // Visit Chart Data
                $.each(visitArr.reverse(), function (i, v) {
                    Visualization.data.visitLabels.push(v.date);
                    visitData.push(v.visits.length);
                    visitorData.push(v.visitors.length);
                });

                Visualization.data.dateRange = Visualization.data.visitLabels[0] + "-" + Visualization.data.visitLabels[Visualization.data.visitLabels.length - 1];
                Visualization.data.numberOfVisit = visitData[visitData.length - 1];
                Visualization.data.numberOfVisitor = visitorData[visitorData.length - 1];
                Visualization.data.visitDatasets = [
                    {
                        label: 'Visits',
                        data: visitData,
                        backgroundColor: 'rgb(255, 99, 132)',
                        borderColor: 'rgb(255, 99, 132)',
                        borderWidth: 2,
                        fill: false
                    },
                    {
                        label: 'Unique Visitors',
                        data: visitorData,
                        backgroundColor: 'rgb(75, 192, 192)',
                        borderColor: 'rgb(75, 192, 192)',
                        borderWidth: 2,
                        fill: false
                    }
                ];

                // Bounced and Duration Data
                $.each(bouncedAndDurationArr.reverse(), function (index, item) {
                    var bounced = 0;
                    var avgDuration = 0;
                    $.each(item.visits, function (i, v) {
                        if (v.count == 1) { bounced += 1; }
                        avgDuration = (parseFloat(avgDuration) + parseFloat(v.duration / v.count)).toFixed(2);
                    });
                    var bouncedRate = ((bounced / item.visits.length) * 100).toFixed(2);
                    var totalAvgDuration = (avgDuration / item.visits.length).toFixed(2);
                    bouncedArr.push({ date: item.date, bouncedRate: bouncedRate });
                    durationArr.push({ date: item.date, duration: totalAvgDuration });
                });

                // Bounced Chart Data
                $.each(bouncedArr, function (i, b) {
                    Visualization.data.bouncedLabels.push(b.date);
                    bouncedData.push(b.bouncedRate);
                });

                Visualization.data.bouncedRate = bouncedData[bouncedData.length - 1];
                Visualization.data.bouncedDatasets = [
                    {
                        label: 'Bounce Rate',
                        data: bouncedData,
                        backgroundColor: 'rgb(255, 99, 132)',
                        borderColor: 'rgb(255, 99, 132)',
                        borderWidth: 2,
                        fill: false
                    }
                ];

                // Duration Chart Data
                $.each(durationArr, function (i, d) {
                    Visualization.data.durationLabels.push(d.date);
                    durationData.push(d.duration);
                });

                Visualization.data.avgDuration = durationData[durationData.length - 1];
                Visualization.data.durationDatasets = [
                    {
                        label: 'Average visit duration',
                        data: durationData,
                        backgroundColor: 'rgb(255, 99, 132)',
                        borderColor: 'rgb(255, 99, 132)',
                        borderWidth: 2,
                        fill: false
                    }
                ];

                // Page Chart Data
                $.each(pageArr, function (i, p) {
                    Visualization.data.pageLabels.push(p.page);
                    Visualization.data.pageData.push(p.count);
                    Visualization.data.pageColors.push(p.color);
                });
                Visualization.data.numberOfMaxPageviews = Math.max(...Visualization.data.pageData);

                // Pageviews Chart Data
                $.each(pageviewsArr.reverse(), function (i, p) {
                    Visualization.data.pageviewsLabels.push(p.date);
                    pageviewsData.push(p.count);
                });
                $.each(uniquePageViewsArr.reverse(), function (i, p) {
                    uniquePageViewsData.push(p.pages.length);
                });

                Visualization.data.numberOfPageviews = pageviewsData[pageviewsData.length - 1];
                Visualization.data.numberOfUniquePageviews = uniquePageViewsData[uniquePageViewsData.length - 1];
                Visualization.data.pageviewsDatasets = [
                    {
                        label: 'Pageviews',
                        data: pageviewsData,
                        backgroundColor: 'rgb(255, 99, 132)',
                        borderColor: 'rgb(255, 99, 132)',
                        borderWidth: 2,
                        fill: false
                    },
                    {
                        label: 'Unique Pageviews',
                        data: uniquePageViewsData,
                        backgroundColor: 'rgb(75, 192, 192)',
                        borderColor: 'rgb(75, 192, 192)',
                        borderWidth: 2,
                        fill: false
                    }
                ];
            }
        });
        var searchLabels = [];
        var searchData = [];
        var searchArr = [];
        var uniqueSearchData = [];
        var uniqueSearchArr = [];
        $.ajax({
            type: 'GET',
            async: false,
            url: "/EditProfileStore/GetSearchEvents",
            success: function (result) {
                $.each(result, function (index, item) {
                    var date = new Date(item.EventTime).toLocaleDateString();
                    var keyword = item.Value.replace("Searched ", "");
                    if (searchArr.length == 0) {
                        searchArr.push({ date: date, count: 1 });
                    } else {
                        var obj = Visualization.util.findObjectByKey(searchArr, "date", date);
                        if (obj) {
                            obj.count += 1;
                        } else {
                            searchArr.push({ date: date, count: 1 });
                        }
                    }
                    if (uniqueSearchArr.length == 0) {
                        uniqueSearchArr.push({ date: date, keywords: [keyword] });
                    } else {
                        var obj = Visualization.util.findObjectByKey(uniqueSearchArr, "date", date);
                        if (obj) {
                            if ($.inArray(keyword, obj.keywords) == -1) {
                                obj.keywords.push(keyword);
                            }
                        } else {
                            uniqueSearchArr.push({ date: date, keywords: [keyword] });
                        }
                    }
                });
            }
        });

        //Search Chart Data
        $.each(searchArr.reverse(), function (i, s) {
            Visualization.data.searchLabels.push(s.date);
            searchData.push(s.count);
        });
        $.each(uniqueSearchArr.reverse(), function (i, s) {
            uniqueSearchData.push(s.keywords.length);
        });
        Visualization.data.numberOfSearch = searchData.length == 0 ? 0 : searchData[searchData.length - 1];
        Visualization.data.numberOfKeywords = uniqueSearchData.length == 0 ? 0 : uniqueSearchData[uniqueSearchData.length - 1];
        Visualization.data.searchDatasets = [
            {
                label: 'Search',
                data: searchData,
                backgroundColor: 'rgb(255, 99, 132)',
                borderColor: 'rgb(255, 99, 132)',
                borderWidth: 2,
                fill: false
            },
            {
                label: 'Unique Search',
                data: uniqueSearchData,
                backgroundColor: 'rgb(75, 192, 192)',
                borderColor: 'rgb(75, 192, 192)',
                borderWidth: 2,
                fill: false
            }
        ];

        //Render Overview
        Visualization.render.overview();
        //Render Chart
        Visualization.render.chart.visit();
    },
    render: {
        chart: {
            visit: function () {
                $('#chart').remove();
                $('#chart-container').append('<canvas id="chart"><canvas>');
                var ctx = document.getElementById("chart").getContext('2d');
                var myChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: Visualization.data.visitLabels,
                        datasets: Visualization.data.visitDatasets
                    },
                    options: {
                        responsive: true,
                        layout: {
                            padding: {
                                left: 0,
                                right: 0,
                                top: 200,
                                bottom: 0
                            }
                        },
                        title: {
                            display: true,
                            text: 'Visits Overview'
                        },
                        tooltips: {
                            mode: 'index',
                            intersect: false,
                        },
                        hover: {
                            mode: 'nearest',
                            intersect: true
                        },
                        scales: {
                            xAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Day'
                                }
                            }],
                            yAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Visits'
                                },
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        }
                    }
                });
            },
            page: function () {
                $('#chart').remove();
                $('#chart-container').append('<canvas id="chart"><canvas>');
                var ctx = document.getElementById("chart").getContext('2d');
                var myChart = new Chart(ctx, {
                    type: "bar",
                    data: {
                        labels: Visualization.data.pageLabels,
                        datasets: [
                            {
                                label: "Pageviews",
                                data: Visualization.data.pageData,
                                fill: false,
                                backgroundColor: Visualization.data.pageColors,
                                borderColor: Visualization.data.pageColors,
                                borderWidth: 1
                            }]
                    },
                    options: {
                        responsive: true,
                        layout: {
                            padding: {
                                left: 0,
                                right: 0,
                                top: 200,
                                bottom: 0
                            }
                        },
                        title: {
                            display: true,
                            text: 'Pageviews by Page'
                        },
                        scales: {
                            xAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Page'
                                }
                            }],
                            yAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Pageviews'
                                },
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        }
                    }
                });
            },
            pageviews: function () {
                $('#chart').remove();
                $('#chart-container').append('<canvas id="chart"><canvas>');
                var ctx = document.getElementById("chart").getContext('2d');
                var myChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: Visualization.data.pageviewsLabels,
                        datasets: Visualization.data.pageviewsDatasets
                    },
                    options: {
                        responsive: true,
                        layout: {
                            padding: {
                                left: 0,
                                right: 0,
                                top: 200,
                                bottom: 0
                            }
                        },
                        title: {
                            display: true,
                            text: 'Pageviews Overview'
                        },
                        tooltips: {
                            mode: 'index',
                            intersect: false,
                        },
                        hover: {
                            mode: 'nearest',
                            intersect: true
                        },
                        scales: {
                            xAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Day'
                                }
                            }],
                            yAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Pageviews'
                                },
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        }
                    }
                });
            },
            duration: function () {
                $('#chart').remove();
                $('#chart-container').append('<canvas id="chart"><canvas>');
                var ctx = document.getElementById("chart").getContext('2d');
                var myChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: Visualization.data.durationLabels,
                        datasets: Visualization.data.durationDatasets
                    },
                    options: {
                        responsive: true,
                        layout: {
                            padding: {
                                left: 0,
                                right: 0,
                                top: 200,
                                bottom: 0
                            }
                        },
                        title: {
                            display: true,
                            text: 'Average visit duration'
                        },
                        tooltips: {
                            mode: 'index',
                            intersect: false,
                        },
                        hover: {
                            mode: 'nearest',
                            intersect: true
                        },
                        scales: {
                            xAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Day'
                                }
                            }],
                            yAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Average visit duration (in seconds)'
                                },
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        }
                    }
                });
            },
            bounced: function () {
                $('#chart').remove();
                $('#chart-container').append('<canvas id="chart"><canvas>');
                var ctx = document.getElementById("chart").getContext('2d');
                var myChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: Visualization.data.bouncedLabels,
                        datasets: Visualization.data.bouncedDatasets
                    },
                    options: {
                        responsive: true,
                        layout: {
                            padding: {
                                left: 0,
                                right: 0,
                                top: 200,
                                bottom: 0
                            }
                        },
                        title: {
                            display: true,
                            text: 'Bounce Rate % (left the website after one page)'
                        },
                        tooltips: {
                            mode: 'index',
                            intersect: false,
                        },
                        hover: {
                            mode: 'nearest',
                            intersect: true
                        },
                        scales: {
                            xAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Day'
                                }
                            }],
                            yAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Bounce Rate (%)'
                                },
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        }
                    }
                });
            },
            search: function () {
                $('#chart').remove();
                $('#chart-container').append('<canvas id="chart"><canvas>');
                var ctx = document.getElementById("chart").getContext('2d');
                var myChart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: Visualization.data.searchLabels,
                        datasets: Visualization.data.searchDatasets
                    },
                    options: {
                        responsive: true,
                        layout: {
                            padding: {
                                left: 0,
                                right: 0,
                                top: 200,
                                bottom: 0
                            }
                        },
                        title: {
                            display: true,
                            text: 'Search Overview'
                        },
                        tooltips: {
                            mode: 'index',
                            intersect: false,
                        },
                        hover: {
                            mode: 'nearest',
                            intersect: true
                        },
                        scales: {
                            xAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Day'
                                }
                            }],
                            yAxes: [{
                                display: true,
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Search'
                                },
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        }
                    }
                });
            }
        },
        overview: function () {
            $("#numberOfVisit").text(Visualization.data.numberOfVisit);
            $("#numberOfVisitor").text(Visualization.data.numberOfVisitor);
            $("#numberOfPageviews").text(Visualization.data.numberOfPageviews);
            $("#numberOfUniquePageviews").text(Visualization.data.numberOfUniquePageviews);
            $("#bouncedRate").text(Visualization.data.bouncedRate + "%");
            $("#avgDuration").text(Visualization.util.convertSecondstoMinutes(Visualization.data.avgDuration));
            $("#numberOfMaxPageviews").text(Visualization.data.numberOfMaxPageviews);
            $("#dateRange").text("(" + Visualization.data.dateRange + ")");
            $("#numberOfSearch").text(Visualization.data.numberOfSearch);
            $("#numberOfKeywords").text(Visualization.data.numberOfKeywords);
        }
    }
}