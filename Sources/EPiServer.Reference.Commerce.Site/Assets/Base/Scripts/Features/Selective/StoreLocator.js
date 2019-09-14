let StoreLocator = {
    storeMap: {},
    searchManager: null,
    storeInfobox: {},
    storeInfo: "",
    markers: [],
    searched: false,

    init: () => {
        $(document).on('keyup', '#searchMapInput', (e) => {
            if (e.keyCode === 13) {
                e.preventDefault();
                StoreLocator.search();
            }
        });

        $(document).ready(() => {
            $("#searchMapInput").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: "http://dev.virtualearth.net/REST/v1/Locations",
                        dataType: "jsonp",
                        data: {
                            key: "Agf8opFWW3n3881904l3l0MtQNID1EaBrr7WppVZ4v38Blx9l8A8x86aLVZNRv2I",
                            q: request.term
                        },
                        jsonp: "jsonp",
                        success: function (data) {
                            let result = data.resourceSets[0];
                            if (result) {
                                if (result.estimatedTotal > 0) {
                                    response($.map(result.resources, function (item) {
                                        $("#searchMapInput").autocomplete('option', 'autoFocus', true);
                                        return {
                                            data: item,
                                            label: item.name + ' (' + item.address.countryRegion + ')',
                                            value: item.name
                                        };
                                    }));
                                }
                            }
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    if (StoreLocator.searched) {
                        StoreLocator.storeMap.entities.pop();
                        StoreLocator.searched = false;
                    }
                    StoreLocator.addSearchedLocationMarker(new Microsoft.Maps.Location(ui.item.data.point.coordinates[0], ui.item.data.point.coordinates[1]));
                }
            });
        });
    },

    loadMapScenario: () => {
        StoreLocator.storeMap = new Microsoft.Maps.Map('#storeMap', {
            credentials: "Agf8opFWW3n3881904l3l0MtQNID1EaBrr7WppVZ4v38Blx9l8A8x86aLVZNRv2I"
        });
        StoreLocator.storeInfobox = new Microsoft.Maps.Infobox(StoreLocator.storeMap.getCenter(), { visible: false });
        StoreLocator.storeInfobox.setMap(StoreLocator.storeMap);
        StoreLocator.showStoreLocation();
    },

    showStoreLocation: () => {
        let locations = [];
        $('#storeLocators .StoreLocator').each(function () {
            locations.push({
                address: $(this).attr('address'),
                html: $(this).closest(".viewDetailStore").clone()
            });
        });

        for (let i = 0; i < locations.length; i++) {
            const loc = locations[i];
            $(loc.html).removeClass("row").find("div").removeClass("col-md-8").removeClass("col-md-4");
            StoreLocator.getStoreLocation(loc.address, $(loc.html).prop('outerHTML'));
        }
    },

    getStoreLocation: (address, html) => {
        let searchRequest;
        if (!StoreLocator.searchManager) {
            Microsoft.Maps.loadModule('Microsoft.Maps.Search', () => {
                StoreLocator.searchManager = new Microsoft.Maps.Search.SearchManager(StoreLocator.storeMap);
                StoreLocator.getStoreLocation(address, html);
            });
        } else {
            searchRequest = {
                where: address,
                callback: (r) => {
                    if (r && r.results && r.results.length > 0) {
                        let pushpin = new Microsoft.Maps.Pushpin(r.results[0].location, { });
                        Microsoft.Maps.Events.addHandler(pushpin, 'click', (e) => {
                            StoreLocator.storeMap.setView({
                                center: e.target.getLocation(),
                                zoom: 15
                            });
                            StoreLocator.storeInfobox.setOptions({
                                location: e.target.getLocation(),
                                maxHeight: 300,
                                maxWidth: 280,
                                description: html,
                                visible: true
                            });
                        });
                        StoreLocator.storeMap.entities.push(pushpin);
                        StoreLocator.markers.push(r.results[0].location);

                        StoreLocator.storeMap.setView({
                            bounds: new Microsoft.Maps.LocationRect.fromLocations(StoreLocator.markers)
                        });
                    }
                },
                errorCallback: (e) => {
                    alert("No results found");
                }
            };
            StoreLocator.searchManager.geocode(searchRequest);
        }
    },

    search: () => {
        if (!StoreLocator.searchManager) {
            Microsoft.Maps.loadModule('Microsoft.Maps.Search', () => {
                StoreLocator.searchManager = new Microsoft.Maps.Search.SearchManager(StoreLocator.storeMap);
                StoreLocator.search();
            });
        }
        else {
            if (StoreLocator.searched) {
                StoreLocator.storeMap.entities.pop();
                StoreLocator.searched = false;
            }
            let address = $('#searchMapInput').val();
            let searchRequest = {
                where: address,
                callback: (r) => {
                    if (r && r.results && r.results.length > 0) {
                        StoreLocator.addSearchedLocationMarker(r.results[0].location);
                    }
                },
                errorCallback: (e) => {
                    alert("No results found");
                }
            };
            StoreLocator.searchManager.geocode(searchRequest);
        }
    },

    addSearchedLocationMarker: (location) => {
        let pushpin = new Microsoft.Maps.Pushpin(location, {
            icon: 'https://www.bingmapsportal.com/Content/images/poi_custom.png'
        });
        Microsoft.Maps.Events.addHandler(pushpin, 'click', (e) => {
            StoreLocator.storeMap.setView({
                center: e.target.getLocation(),
                zoom: 15
            });
        });

        StoreLocator.storeMap.entities.push(pushpin);
        StoreLocator.markers.push(location);

        StoreLocator.storeMap.setView({
            bounds: Microsoft.Maps.LocationRect.fromLocations(StoreLocator.markers)
        });

        StoreLocator.storeInfobox.setOptions({ visible: false });
        StoreLocator.searched = true;
    },

    showCurrentLocation: () => {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition((position) => {
                let location = new Microsoft.Maps.Location(position.coords.latitude, position.coords.longitude);
                let pushpin = new Microsoft.Maps.Pushpin(location, {
                    color: 'blue'
                });
                Microsoft.Maps.Events.addHandler(pushpin, 'click', (e) => {
                    StoreLocator.storeMap.setView({
                        center: e.target.getLocation(),
                        zoom: 15
                    });
                });

                StoreLocator.storeMap.entities.push(pushpin);
                StoreLocator.markers.push(r.results[0].location);

                StoreLocator.storeMap.setView({
                    bounds: Microsoft.Maps.LocationRect.fromLocations(StoreLocator.markers)
                });

                StoreLocator.storeInfobox.setOptions({ visible: false });
            }, (error) => {
                alert(error.message + " Use Edge to demo this function since it allows geolocation in http.");
            });
        } else {
            x.innerHTML = "Geolocation is not supported by this browser.";
        }
    },

    setStore: function (code, name) {
        $.ajax({
            type: "POST",
            url: "/StorePage/SetDefaultStore",
            data: { storeCode: code },
            success: function (response) {
                $("#storeName").text(name);
                StoreLocator.storeInfobox.setOptions({ visible: false });
            }
        });
    }
};