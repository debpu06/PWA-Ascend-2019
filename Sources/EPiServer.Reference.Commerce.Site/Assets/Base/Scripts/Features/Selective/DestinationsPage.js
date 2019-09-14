var map;
var markers = [];

function initializeMap() {
    //var mapCenter = {
    //    lat: $('#destinations-map').attr('data-mapcenter-lat'),
    //    lon: $('#destinations-map').attr('data-mapcenter-lon')
    //};

    var locations = [];
    $('#destinations .destination').each(function () {
        locations.push({
            name: $(this).attr('data-mapname'),
            lat: $(this).attr('data-maplat'),
            lon: $(this).attr('data-maplon'),
            url: $(this).attr('data-mapurl')
        });
    });

    var mapOptions = {
        //zoom: 2,
        //center: new google.maps.LatLng(mapCenter.lat, mapCenter.lon),
        mapTypeId: google.maps.MapTypeId.TERRAIN,
        mapTypeControl: false,
        streetViewControl: false,
        scrollwheel: false,
        maxZoom: 4,
        minZoom: 2
    };
    if (!map) {
        map = new google.maps.Map(document.getElementById("destinations-map"), mapOptions);
    } else {
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(null);
        }
        markers = new Array();
    }
    var infowindow = new google.maps.InfoWindow();
    var bounds = new google.maps.LatLngBounds();
    var marker;

    for (var j = 0; j < locations.length; j++) {
        marker = new google.maps.Marker({
            position: new google.maps.LatLng(locations[j].lat, locations[j].lon),
            map: map,
            optimized: false
            //title: locations[j].name
        });
        markers.push(marker);
        
        bounds.extend(marker.position);

        google.maps.event.addListener(marker, 'click', (function (m, j) {
            return function () {
                infowindow.setContent("<a href='" + locations[j].url + "' style='white-space:nowrap'>" + locations[j].name + "</a>");
                infowindow.open(map, m);
            };
        })(marker, j));
    }
    map.fitBounds(bounds);
    map.panToBounds(bounds);
}

var tempvals = [-20, 40];
var originalVal;

function initializeFilters() {
    $('#slider-range').slider({ min: -20, max: 40, value: [-20, 40] });

    $('#slider-range').on('slideStop', function () {
        var newVal = $('#slider-range').data('slider').getValue();
        tempvals = newVal;
        DoAjaxCallback($('.filterblock'));
    });

    $(".filterblock").on('change', 'input[type=checkbox].select-all', function(event) {
        if ($(event.target).prop('checked')) {
            $(event.target).closest('.filterblock').find('input[type=checkbox].select-some').prop('checked', false);
        }
    });

    $('.filterblock').on('change', 'input[type=checkbox].select-some', function (event) {
        if ($(event.target).prop('checked')) {
            $(event.target).closest('.filterblock').find('input[type=checkbox].select-all').prop('checked', false);
        }
        else {
            if ($(event.target).closest('.filterblock').find("input[type=checkbox]:checked").length === 0) {
                $(event.target).closest('.filterblock').find('input[type=checkbox].select-all').prop('checked', true);
            }
        }
    });

    $('.filterblock').on('change', 'input[type=checkbox]', function () {
        var triggerFilterId = $(this).closest('.filterblock').attr('id');
        var filtersToUpdate = $('.filterblock:not([id=' + triggerFilterId + '])');
        if ($(this).is('.select-all')) {
            filtersToUpdate = $('.filterblock');
        }
        DoAjaxCallback(filtersToUpdate);
    });
}

function setFilterSelection() {
    $('.filterblock').each(function () {
        if ($(this).find('input[type = checkbox]').attr('checked')) {
            $(this).addClass('selected');
        } else {
            $(this).removeClass('selected');
        }
    });
}

function getFilterUrl() {
    var uri = new Uri(location.URL);
    $('.filterblock').each(function () {
        var filterName = $(this).attr('data-filtertype');
        var value = '';
        $(this).find('input[type=checkbox].select-some:checked').each(function () {
            if (value !== '') {
                value += ',';
            }
            value += $(this).val();
        });
        uri.replaceQueryParam(filterName, value);
    });
    uri.replaceQueryParam("t", tempvals[0] + "," + tempvals[1]);
    return uri;
}

function DoAjaxCallback(filtersToUpdate) {
    $.ajax({
        url: getFilterUrl(),
        success: function (data) {
            var fetched = $(data);
            $('#destinations').html(fetched.find('#destinations').html());
            initializeMap();
            filtersToUpdate.each(function () {
                $(this).html(fetched.find('#' + $(this).attr('id')).html());
            });
            setFilterSelection();
            //history.pushState({}, document.title, getFilterUrl());
        }
    });
}

$(initializeMap);
$(initializeFilters);
$(setFilterSelection);