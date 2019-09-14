var Dashboard = {
    init: function () {
        Dashboard.initData();
    },
    util: {
        convertCountryCodes: function (code) {
            var obj = Dashboard.util.findObjectByKey(Dashboard.data.countries, 'trackCode', code);
            if (obj) {
                return obj.mapCode;
            }
            return null;
        },
        findObjectByKey: function (array, key, value) {
            for (var i = 0; i < array.length; i++) {
                if (array[i][key] === value) {
                    return array[i];
                }
            }
            return null;
        },
        mapFillColor: function (number) {
            if (number <= 10) {
                return '#CDDAEF';
            } else if (number > 10 && number <= 20) {
                return '#C2D0E8';
            } else if (number > 20 && number <= 40) {
                return '#ACBDDB';
            } else if (number > 40) {
                return '#385993';
            }
        }
    },
    data: {
        visitData: {},
        visitFills: {
            defaultFill: '#d3d3d3'
        },
        countries: [
            { trackCode: "AE", mapCode: "ARE" },
            { trackCode: "AF", mapCode: "AFG" },
            { trackCode: "AL", mapCode: "ALB" },
            { trackCode: "AM", mapCode: "ARM" },
            { trackCode: "AO", mapCode: "AGO" },
            { trackCode: "AQ", mapCode: "ATA" },
            { trackCode: "AR", mapCode: "ARG" },
            { trackCode: "AT", mapCode: "AUT" },
            { trackCode: "AU", mapCode: "AUS" },
            { trackCode: "AZ", mapCode: "AZE" },
            { trackCode: "BA", mapCode: "BIH" },
            { trackCode: "BD", mapCode: "BGD" },
            { trackCode: "BE", mapCode: "BEL" },
            { trackCode: "BF", mapCode: "BFA" },
            { trackCode: "BG", mapCode: "BGR" },
            { trackCode: "BI", mapCode: "BDI" },
            { trackCode: "BJ", mapCode: "BEN" },
            { trackCode: "BN", mapCode: "BRN" },
            { trackCode: "BO", mapCode: "BOL" },
            { trackCode: "BR", mapCode: "BRA" },
            { trackCode: "BS", mapCode: "BHS" },
            { trackCode: "BT", mapCode: "BTN" },
            { trackCode: "BW", mapCode: "BWA" },
            { trackCode: "BY", mapCode: "BLR" },
            { trackCode: "BZ", mapCode: "BLZ" },
            { trackCode: "CA", mapCode: "CAN" },
            { trackCode: "CD", mapCode: "COD" },
            { trackCode: "CF", mapCode: "CAF" },
            { trackCode: "CG", mapCode: "COG" },
            { trackCode: "CH", mapCode: "CHE" },
            { trackCode: "CI", mapCode: "CIV" },
            { trackCode: "CL", mapCode: "CHL" },
            { trackCode: "CM", mapCode: "CMR" },
            { trackCode: "CN", mapCode: "CHN" },
            { trackCode: "CO", mapCode: "COL" },
            { trackCode: "CR", mapCode: "CRI" },
            { trackCode: "CU", mapCode: "CUB" },
            { trackCode: "CY", mapCode: "CYP" },
            { trackCode: "CZ", mapCode: "CZE" },
            { trackCode: "DE", mapCode: "DEU" },
            { trackCode: "DJ", mapCode: "DJI" },
            { trackCode: "DK", mapCode: "DNK" },
            { trackCode: "DO", mapCode: "DOM" },
            { trackCode: "DZ", mapCode: "DZA" },
            { trackCode: "EC", mapCode: "ECU" },
            { trackCode: "EE", mapCode: "EST" },
            { trackCode: "EG", mapCode: "EGY" },
            { trackCode: "ER", mapCode: "ERI" },
            { trackCode: "ES", mapCode: "ESP" },
            { trackCode: "ET", mapCode: "ETH" },
            { trackCode: "FI", mapCode: "FIN" },
            { trackCode: "FJ", mapCode: "FJI" },
            { trackCode: "FK", mapCode: "FLK" },
            { trackCode: "FR", mapCode: "FRA" },
            { trackCode: "GA", mapCode: "GAB" },
            { trackCode: "GB", mapCode: "GBR" },
            { trackCode: "GE", mapCode: "GEO" },
            { trackCode: "GF", mapCode: "GUF" },
            { trackCode: "GH", mapCode: "GHA" },
            { trackCode: "GL", mapCode: "GRL" },
            { trackCode: "GM", mapCode: "GMB" },
            { trackCode: "GN", mapCode: "GIN" },
            { trackCode: "GQ", mapCode: "GNQ" },
            { trackCode: "GR", mapCode: "GRC" },
            { trackCode: "GT", mapCode: "GTM" },
            { trackCode: "GW", mapCode: "GNB" },
            { trackCode: "GY", mapCode: "GUY" },
            { trackCode: "HN", mapCode: "HND" },
            { trackCode: "HR", mapCode: "HRV" },
            { trackCode: "HT", mapCode: "HTI" },
            { trackCode: "HU", mapCode: "HUN" },
            { trackCode: "ID", mapCode: "IDN" },
            { trackCode: "IE", mapCode: "IRN" },
            { trackCode: "IL", mapCode: "ISR" },
            { trackCode: "IN", mapCode: "IDN" },
            { trackCode: "IQ", mapCode: "IRQ" },
            { trackCode: "IR", mapCode: "IRN" },
            { trackCode: "IS", mapCode: "ISL" },
            { trackCode: "IT", mapCode: "ITA" },
            { trackCode: "JM", mapCode: "JAM" },
            { trackCode: "JO", mapCode: "JOR" },
            { trackCode: "JP", mapCode: "JPN" },
            { trackCode: "KE", mapCode: "KEN" },
            { trackCode: "KG", mapCode: "KGZ" },
            { trackCode: "KH", mapCode: "KHM" },
            { trackCode: "KP", mapCode: "PRK" },
            { trackCode: "KR", mapCode: "KOR" },
            { trackCode: "KW", mapCode: "KWT" },
            { trackCode: "KZ", mapCode: "KAZ" },
            { trackCode: "LA", mapCode: "LAO" },
            { trackCode: "LB", mapCode: "LBN" },
            { trackCode: "LK", mapCode: "LKA" },
            { trackCode: "LR", mapCode: "LBR" },
            { trackCode: "LS", mapCode: "LSO" },
            { trackCode: "LT", mapCode: "LTU" },
            { trackCode: "LU", mapCode: "LUX" },
            { trackCode: "LV", mapCode: "LVA" },
            { trackCode: "LY", mapCode: "LBY" },
            { trackCode: "MA", mapCode: "MAR" },
            { trackCode: "MD", mapCode: "MDA" },
            { trackCode: "ME", mapCode: "MNE" },
            { trackCode: "MG", mapCode: "MDG" },
            { trackCode: "MK", mapCode: "MKD" },
            { trackCode: "ML", mapCode: "MLI" },
            { trackCode: "MM", mapCode: "MMR" },
            { trackCode: "MN", mapCode: "MNG" },
            { trackCode: "MR", mapCode: "MRT" },
            { trackCode: "MW", mapCode: "MWI" },
            { trackCode: "MX", mapCode: "MEX" },
            { trackCode: "MY", mapCode: "MYS" },
            { trackCode: "MZ", mapCode: "MOZ" },
            { trackCode: "NA", mapCode: "NAM" },
            { trackCode: "NC", mapCode: "NCL" },
            { trackCode: "NE", mapCode: "NER" },
            { trackCode: "NG", mapCode: "NGA" },
            { trackCode: "NI", mapCode: "NIC" },
            { trackCode: "NL", mapCode: "NLD" },
            { trackCode: "NO", mapCode: "NOR" },
            { trackCode: "NP", mapCode: "NPL" },
            { trackCode: "NZ", mapCode: "NZL" },
            { trackCode: "OM", mapCode: "OMN" },
            { trackCode: "PA", mapCode: "PAN" },
            { trackCode: "PE", mapCode: "PER" },
            { trackCode: "PG", mapCode: "PNG" },
            { trackCode: "PH", mapCode: "PHL" },
            { trackCode: "PK", mapCode: "PAK" },
            { trackCode: "PL", mapCode: "POL" },
            { trackCode: "PR", mapCode: "PRI" },
            { trackCode: "PT", mapCode: "PRT" },
            { trackCode: "PY", mapCode: "PRY" },
            { trackCode: "QA", mapCode: "QAT" },
            { trackCode: "RO", mapCode: "ROU" },
            { trackCode: "RS", mapCode: "SRB" },
            { trackCode: "RU", mapCode: "RUS" },
            { trackCode: "RW", mapCode: "RWA" },
            { trackCode: "SA", mapCode: "SAU" },
            { trackCode: "SB", mapCode: "SLB" },
            { trackCode: "SD", mapCode: "SDN" },
            { trackCode: "SE", mapCode: "SWE" },
            { trackCode: "SI", mapCode: "SVN" },
            { trackCode: "SK", mapCode: "SVK" },
            { trackCode: "SL", mapCode: "SLE" },
            { trackCode: "SN", mapCode: "SEN" },
            { trackCode: "SO", mapCode: "SOM" },
            { trackCode: "SR", mapCode: "SUR" },
            { trackCode: "SS", mapCode: "SSD" },
            { trackCode: "SV", mapCode: "SLV" },
            { trackCode: "SY", mapCode: "SYR" },
            { trackCode: "SZ", mapCode: "SWZ" },
            { trackCode: "TD", mapCode: "TCD" },
            { trackCode: "TF", mapCode: "ATF" },
            { trackCode: "TG", mapCode: "TGO" },
            { trackCode: "TH", mapCode: "THA" },
            { trackCode: "TJ", mapCode: "TJK" },
            { trackCode: "TL", mapCode: "TLS" },
            { trackCode: "TM", mapCode: "TKM" },
            { trackCode: "TN", mapCode: "TUN" },
            { trackCode: "TR", mapCode: "TUR" },
            { trackCode: "TT", mapCode: "TTO" },
            { trackCode: "TW", mapCode: "TWN" },
            { trackCode: "TZ", mapCode: "TZA" },
            { trackCode: "UA", mapCode: "UKR" },
            { trackCode: "UG", mapCode: "UGA" },
            { trackCode: "US", mapCode: "USA" },
            { trackCode: "UY", mapCode: "URY" },
            { trackCode: "UZ", mapCode: "UZB" },
            { trackCode: "VE", mapCode: "VEN" },
            { trackCode: "VN", mapCode: "VNM" },
            { trackCode: "VU", mapCode: "VUT" },
            { trackCode: "YE", mapCode: "YEM" },
            { trackCode: "ZA", mapCode: "ZAF" },
            { trackCode: "ZM", mapCode: "ZMB" },
            { trackCode: "ZW", mapCode: "ZWE" }
        ]
    },
    initData: function () {
        var visitArr = [];
        var visitData = {};
        var visitFill = {};
        $.ajax({
            type: 'GET',
            async: false,
            url: "/EditProfileStore/GetPageViewEvents",
            success: function (result) {
                $.each(result, function (index, item) {
                    var countryCode = Dashboard.util.convertCountryCodes(item.CountryCode);
                    var deviceId = item.DeviceId;
                    var visitor = item.User.Email ? item.User.Email : item.DeviceId;
                    var visitKey = item.DeviceId + "-" + visitor;
                    if (countryCode) {
                        if (visitArr.length == 0) {
                            visitArr.push({ countryCode: countryCode, visits: [visitKey], visitors: [visitor] });
                        } else {
                            var obj = Dashboard.util.findObjectByKey(visitArr, "countryCode", countryCode);
                            if (obj) {
                                if ($.inArray(visitKey, obj.visits) == -1) {
                                    obj.visits.push(visitKey);
                                }
                                if ($.inArray(visitor, obj.visitors) == -1) {
                                    obj.visitors.push(visitor);
                                }
                            } else {
                                visitArr.push({ countryCode: countryCode, visits: [visitKey], visitors: [visitor] });
                            }
                        }
                    }
                });
                $.each(visitArr, function (i, v) {
                    Dashboard.data.visitData[v.countryCode] = { fillKey: v.countryCode, visits: v.visits.length, visitors: v.visitors.length };
                    Dashboard.data.visitFills[v.countryCode] = Dashboard.util.mapFillColor(v.visits.length);
                });
                Dashboard.render.map();
            }
        });
    },
    render: {
        map: function () {
            var map = new Datamap({
                element: document.getElementById('map'),
                scope: 'world',
                geographyConfig: {
                    popupOnHover: true,
                    popupTemplate: function (geography, data) {
                        if (data) {
                            return '<div class="hoverinfo"><strong>' + geography.properties.name + '</strong><br/><strong>' + data.visits + '</strong> visits<br/><strong>' + data.visitors + '</strong> visitors</div>';
                        } else {
                            return '<div class="hoverinfo"><strong>' + geography.properties.name + '</strong><br/>No visit</div>';
                        }
                    },
                    highlightOnHover: true
                },
                fills: Dashboard.data.visitFills,
                data: Dashboard.data.visitData
            });
        }
    }
}