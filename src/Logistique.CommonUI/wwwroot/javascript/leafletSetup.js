window.map = null;
window.markers = null;

window.Color = net.brehaut.Color;

window.hashCode = function (str) {
    var hash = 0;
    for (var i = 0; i < str.length; i++) {
        hash = str.charCodeAt(i) + ((hash << 5) - hash);
    }
    return hash;
};

window.intToRGB = function (i) {
    var c = (i & 0x00FFFFFF)
        .toString(16)
        .toUpperCase();

    return "00000".substring(0, 6 - c.length) + c;
};
// http://google.github.io/palette.js/
window.colors = palette('tol', 12);
window.ClosestColor = (function () {
    function dist(s, t) {
        if (!s.length || !t.length) return 0;
        return dist(s.slice(2), t.slice(2)) +
            Math.abs(parseInt(s.slice(0, 2), 16) - parseInt(t.slice(0, 2), 16));
    }

    return function (arr, str) {
        return [].slice.call(arr).sort(function (a, b) {
            return dist(a, str) - dist(b, str);
        });
    };
}());
window.StringToColor = function (str) {
    let color = ClosestColor(colors, intToRGB(hashCode(str)))[0];
    return color;
};
window.Leaflet =
{
    InitMap: function () {
        // Init Map
        var mapContainer = document.getElementById('mapContainer');
        mapContainer.innerHTML = "<div id='map' style='width: 100%; height: 100%;z-index:5;'></div>";
        map = L.map("map", {
            renderer: L.svg(),
            center: [35.706677, 51.3966922],
            zoom: 12,
            maxZoom: 18,
            zoomControl: false
        });
        map.on('zoomend', function () {
            map.closePopup();
            if (markers) map.removeLayer(markers);
            markers.addTo(map);
        });
        let OSM = true;
        if (OSM) {
            L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png").addTo(map);
        }
        else {
            //var gl = L.mapboxGL({
            //    accessToken: 'pk.eyJ1IjoibW9iaW5zZXZlbiIsImEiOiJjazZnMmN3N20yMGMzM21tZ2U5ZXE1dXl0In0.7s7AjIXOK-Qbv0xdeYhW4A',
            //    style: 'mapbox://styles/mapbox/streets-v11'
            //}).addTo(map);
            //try {
            //    mapboxgl.setRTLTextPlugin('_content/Logistique.CommonUI/other/Mapbox/mapbox-gl-rtl-text.min.js');
            //}
            //catch{ }
            //gl.getMapboxMap().addControl(new MapboxLanguage({
            //    defaultLanguage: 'ar'
            //}));
        }
    },
    InitEditingFunctions: function (helper) {
        // Definitions
        window.ExistingLocationPopupContent = function (Marker) {
            let Location = Marker.location;
            let template = document.querySelector('#ExistingLocation div.form');
            let elem = template.cloneNode(true);
            elem.querySelector("#LocationName").innerHTML = Location.name;
            elem.querySelector("#LocationType").innerHTML = Location.locationType;
            document.querySelector("#DeleteLocationName").innerHTML = Location.name;
            document.querySelector("#DeleteLocationType").innerHTML = Location.locationType;
            elem.querySelector("div.custom-checkbox input.custom-control-input").id = "CheckBox" + Location.id;
            elem.querySelector("div.custom-checkbox input.custom-control-input").checked = Location.selected;
            elem.querySelector("div.custom-checkbox input.custom-control-input").onclick = function () { Select(this, Location.id); };
            elem.querySelector("div.custom-checkbox label.custom-control-label").htmlFor = "CheckBox" + Location.id;
            elem.querySelector("#EditCollapseButton").dataset.target = "#EditCollapse" + Location.id;
            elem.querySelector("#EditCollapse").id += Location.id;
            elem.querySelector("#LocationNameInput").value = Location.name;
            elem.querySelector("#LocationNameInput").id += Location.id;
            elem.querySelector("#TypeSelect").value = Location.locationTypeId;
            elem.querySelector("#TypeSelect").id += Location.id;
            document.querySelector("#DeleteLocationBtn").onclick = function () { DeleteLocation(Location.id) };
            elem.querySelector("#EditLocationBtn").onclick = function () { EditLocation(Location.id) };
            return elem;
        };
        window.PopUp = function (marker) {
            let popup = new L.Popup();
            popup.setContent(ExistingLocationPopupContent(marker));
            popup.setLatLng(marker.getLatLng());
            map.openPopup(popup);
        }
        window.Icon = function (Location) {
            let ColourNum = StringToColor(Location.locationType);
            let Colour = Color(`#${ColourNum}`).setLightness(.6);
            let fa_icon = 'store-alt';
            if (Location._LocationType.iconName)
                fa_icon = Location._LocationType.iconName;
            if (!Location.selected) {
                Colour = Color(`#${ColourNum}`).setSaturation(.15);
                fa_icon = 'times';
            }
            return L.divIcon({
                className: 'custom-div-icon',
                html: `<div style='background-color:${Colour};' class='marker-pin'></div><i class='fa fa-${fa_icon} fa-xs'>`,
                iconSize: [30, 42],
                iconAnchor: [15, 42]
            });
        };
        window.NewLocationPopupContent = function (lat, lng) {
            var template = document.querySelector('#NewLocation div.form');
            var elem = template.cloneNode(true);
            elem.querySelector("#SaveLocationBtn").onclick = function () { SaveLocation(lat, lng) };
            return elem;
        };
        window.onMapClick = function (e) {
            lat = e.latlng.lat;
            lng = e.latlng.lng;
            L.popup()
                .setLatLng(e.latlng)
                .setContent(NewLocationPopupContent(lat, lng))
                .openOn(map);
        };
        window.Select = function (cb, id) {
            helper.invokeMethodAsync("SetLocationSelected", id, cb.checked);
        };
        window.DeleteLocation = function (id) {
            helper.invokeMethodAsync("DeleteLocation", id);
        };
        window.EditLocation = function (id) {
            var input = document.getElementById('LocationNameInput' + id);
            var typeSelect = document.getElementById("TypeSelect" + id);
            var type = typeSelect.options[typeSelect.selectedIndex].value;
            helper.invokeMethodAsync("EditLocation", id, input.value, type);
        };
        window.SaveLocation = function (lat, lng) {
            map.closePopup();
            var input = document.getElementById('NewLocationNameInput');
            var typeSelect = document.getElementById("TypeSelect");
            var type = typeSelect.options[typeSelect.selectedIndex].value;
            helper.invokeMethodAsync("AddNewLocation", input.value, lat, lng, type);
        };

        window.CreateMarker = function (location) {
            var marker = L.marker([location.latitude, location.longitude], { icon: Icon(location) });
            marker.location = location;
            return marker;
        };
        // Init `Add Location` pop-ups
        if (L.Browser.mobile) {
            var spinner = function (x, y) {
                return $(`<div id="touchIndicator" style="position:fixed; top:` + y + `px;left:` + x + `px;z-index:99;"><div class="spinner-grow  text-success" style="width: 5rem; height: 5rem;" role="status"></div></div>`);
            };
            mapContainer.addEventListener("touchstart", function (e) {
                $("body").append(spinner(e.touches[0].pageX - mapContainer.offsetLeft - 50, e.touches[0].pageY - mapContainer.offsetTop - 50));
            });
            mapContainer.addEventListener("touchend", function (e) {
                if (document.querySelector("#touchIndicator"))
                    document.body.removeChild(document.querySelector("#touchIndicator"));
            });
            mapContainer.addEventListener("touchmove", function (e) {
                if (document.querySelector("#touchIndicator"))
                    document.body.removeChild(document.querySelector("#touchIndicator"));
            });
        }
        map.on('contextmenu', onMapClick);
    },
    MakeEditingMarkers: function (Locations) {
        if (map) {
            //Init Location Markers
            map.closePopup();
            if (markers) map.removeLayer(markers);
            markers = L.featureGroup();
            for (var d = 0; d < Locations.length; d++) {
                markers.addLayer(CreateMarker(Locations[d]));
            }
            markers.on('click', function (a) {
                PopUp(a.layer);
            });
            markers.addTo(map);
        }
    },
    InitDisplayFunctions: function () {
        // Definitions
        window.OpenNavigation = function (lat, lng) {
            window.open(`https://www.google.com/maps/dir/?api=1&travelmode=driving&layer=traffic&destination=${lat},${lng}`);
        };

        window.CenterMarker = function (marker) {
            if (!marker) return;
            var latLngs = [marker.getLatLng()];
            var markerBounds = L.latLngBounds(latLngs);
            map.fitBounds(markerBounds, {
                padding: [50, 50],
                maxZoom: 16,
                animate: true
            });
            this.setTimeout(function () { PopUp(marker) }, 1000);
        }
        function HasNext(Marker) {
            let nextMarker = markers.getLayers().find(m => m.order.driverId == Marker.order.driverId && m.order.value == Marker.order.value + 1);
            return nextMarker;
        }
        function HasPrev(Marker) {
            let prevMarker = markers.getLayers().find(m => m.order.driverId == Marker.order.driverId && m.order.value == Marker.order.value - 1);
            return prevMarker;
        }
        window.LocationPopupContent = function (Marker) {
            let Order = Marker.order;
            var template = document.querySelector('#ExistingLocation');
            var elem = template.content.cloneNode(true);
            elem.querySelector("#LocationName").innerHTML = Order.location.name;
            elem.querySelector("#LocationType").innerHTML = Order.location.locationType;
            elem.querySelector("#GetDirectionsBtn").onclick = function () { OpenNavigation(Order.location.latitude.toString(), Order.location.longitude.toString()) };
            elem.querySelector("#DriverUsername").innerHTML = Order.driver.userName;
            elem.querySelector("#OrderValue").innerHTML = Order.value + 1;
            elem.querySelector("#OrdersCount").innerHTML = Order.driver.orders.length;
            elem.querySelector("#MarkerNextBtn").disabled = !HasNext(Marker);
            elem.querySelector("#MarkerNextBtn").onclick = function () {
                CenterMarker(HasNext(Marker));
            };
            elem.querySelector("#MarkerPrevBtn").disabled = !HasPrev(Marker);
            elem.querySelector("#MarkerPrevBtn").onclick = function () {
                CenterMarker(HasPrev(Marker));
            };
            return elem;
        };
        window.PopUp = function (marker) {
            var popup = new L.Popup();
            popup.setContent(LocationPopupContent(marker));
            popup.setLatLng(marker.getLatLng());
            map.openPopup(popup);
        }
        window.Icon = function (orderValue, ordersCount, color) {
            let Colour = Color(`#${color}`).setLightness(.4);
            return L.divIcon({
                className: 'custom-div-icon',
                html: `<div style='background-color:${Colour};' class='marker-pin'></div><i class=''>` + orderValue,
                iconSize: [30, 42],
                iconAnchor: [15, 42]
            });
        };
    },
    MakeDisplayMarkers: function (Orders) {
        if (map) {
            //Init Location Pop-Ups
            if (markers) map.removeLayer(markers);
            const MarkerClusterOptions = {
                showCoverageOnHover: false,
                maxClusterRadius: 0,
                removeOutsideVisibleBounds: false,
                zoomToBoundsOnClick: false,
                iconCreateFunction: function (cluster) {
                    return L.divIcon({
                        html: '<div><i class="fa fa-warehouse fa-xs"></i></div>', className: 'marker-cluster marker-cluster-small'
                    });
                }
            };
            markers = L.markerClusterGroup(MarkerClusterOptions);
            for (var o = 0; o < Orders.length; o++) {
                Orders[o].driver.orders = Orders.filter(or => or.driverId == Orders[o].driverId);
            }
            for (var o = 0; o < Orders.length; o++) {
                let order = Orders[o];
                if (order.done) continue; // dont show if marked as done
                let marker = L.marker([order.location.latitude, order.location.longitude], { icon: Icon(order.value + 1, Orders[o].driver.orders.length, StringToColor(order.driverId)) });
                marker.order = order;
                //marker.addTo(markers);
                markers.addLayer(marker);
            }
            markers.on('click', function (a) {
                PopUp(a.layer);
            });
            map.addLayer(markers);
            this.AdjustWithMarkers();
        }
    },
    AdjustWithMarkers: function () {
        if (markers)
            if (markers.getLayers().length > 0) {
                map.fitBounds(markers.getBounds(), {
                    padding: [50, 50],
                    maxZoom: 15,
                    animate: false
                });
            }
    },
    SetupForEditing: function (helper, Locations) {
        this.InitMap();
        this.InitEditingFunctions(helper);
        this.MakeEditingMarkers(Locations);
        this.AdjustWithMarkers();
    },
    SetupForDisplay: function () {
        this.InitMap();
        this.InitDisplayFunctions();
    },
    CenterLocation: function (lat, lng, zoom) {
        if (!zoom) zoom = 15;
        map.setView([lat, lng], zoom);
    }
};