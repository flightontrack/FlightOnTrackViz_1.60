//function MarkersUpdate(jsonFlights) {
function getDrawActveFlights(jsonFlights, centerPosition) {
    var pointList = [];
    var uniqueFligts = [];
    var iconimage;
    var isPushpin = [];

    var active_pushpin = {
        url: "/Images/1484079763_green-pin.png",
        scaledSize: new google.maps.Size(28, 28),
        origin: new google.maps.Point(0, 0)
    };
    var frozen_pushpin = {
        url: "/Images/1484298852_blue-pin.png",
        scaledSize: new google.maps.Size(24, 24),
        origin: new google.maps.Point(0, 0)
    };
    var tracepoint = {
        url: "/Images/1484009648_circle_blue.png",
        scaledSize: new google.maps.Size(6, 6),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(0, 0)
    };

    //map set to null all previous positions
    setMapOnAll(null);
    setflightPath(null);
    setConnectionLines(null);

    //clean up store arrays
    markersStore = [];
    infowindowStore = [];
    flightPathStore = [];
    connectionLinesStore = [];


    if (jsonFlights.length == 0) {
        console.log("No active flights");
        return;
    }
    else {
        //console.log('There is data to view.');
        $(jsonFlights).each(function (key, value) {

            //setting point icon and array of unique flights
            var pushpin = $(this).attr('isPositionCurrent') == 1 ? active_pushpin : frozen_pushpin;

            if (uniqueFligts.indexOf($(this).attr('FlightID')) == -1) {
                //if next flight add the flight to list
                uniqueFligts.push($(this).attr('FlightID'));
                //set pushpin on last position of the flight
                iconimage = pushpin;
                isPushpin = true;
            }
            else {
                iconimage = tracepoint;
                isPushpin = false;
            }

            //set point infowindow
            var popupWindowText = "<table border=0 style=\"font-size:95%;font-family:arial,helvetica,sans-serif;\">"
                + "<tr><td align=left>N<b>&nbsp&nbsp" + $(this).attr('AcftNumLocal') + "</b></td></tr>"
                + "<tr><td align=left>#<b>&nbsp&nbsp" + $(this).attr('FlightID') + "</b></td></tr>"
                + "<tr><td align=left>Ft<b>&nbsp" + $(this).attr('AltitudeFt') + "</b></td></tr>"
                + "<tr><td align=left>T<b>&nbsp&nbsp" + $(this).attr('durHhMm') + "</b></td></tr>";

            // array of trackpoint for markers and their infowindows and icons
            pointList[key] = new LatLng($(this).attr('latitude'), $(this).attr('longitude'), iconimage, "Flight" + $(this).attr('FlightID'), popupWindowText, $(this).attr('FlightID'), isPushpin);
            if (isPushpin) {
                var connectionLine = [
                    centerPosition,
                    new google.maps.LatLng(this.latitude, this.longitude)
                ]
                connectionLinesStore.push(
                    new google.maps.Polyline({
                        path: connectionLine,
                        strokeColor: "#FF0000",
                        strokeOpacity: 0.5,
                        strokeWeight: 1
                    })
                )
            }
        });

        //build store arrays for markers and infowindow for later use
        pointList.forEach(
        function (p, i) {
            var marker = new google.maps.Marker(
                {
                    position: p,
                    icon: p.micon,
                    title: p.title
                }
              );
            //var aa = marker.showInfoWindow;
            p.ispushpin ? markersStore.push(marker) : null;// : markersStore.push(null);
            var infowindow = new google.maps.InfoWindow({
                content: p.infow,
                disableAutoPan: true
                //maxWidth: 55
            });
            p.ispushpin ? infowindowStore.push(infowindow) : null // infowindowStore.push(null);
        });

        //build store array for lines separately
        uniqueFligts.forEach(function drawPoly(value, j) {
            var flight = value;
            var flightPointList = pointList.filter(function getPoly(val) { return (val.flightid == flight) ? true : false; })
            var flightPath = new google.maps.Polyline({
                path: flightPointList,
                strokeColor: "#0000FF",
                strokeOpacity: 0.8,
                strokeWeight: 3
            });
            flightPathStore.push(flightPath)


        });
        //draw markers out of stores
        setMapOnAll(map);
        // draw polyline out of store
        setflightPath(map);
        //draw connection lines
        setConnectionLines(map);
    }
}

//---------------------------------------------------------------
LatLng = function (rawLat, rawLng, micon, title, infow, flightid, isPushpin) { // (Number, Number)
    var lat = parseFloat(rawLat),
	    lng = parseFloat(rawLng);

    if (isNaN(lat) || isNaN(lng)) {
        throw new Error('Invalid LatLng object: (' + rawLat + ', ' + rawLng + ')');
    }
    this.lat = lat;
    this.lng = lng;
    this.micon = micon;
    this.title = title;
    this.infow = infow;
    this.flightid = flightid;
    this.ispushpin = isPushpin;
};

function setMapOnAll(p_map) {
    // Sets  all markers in the store along with infowindow.
    for (var i = 0; i < markersStore.length; i++) {
        markersStore[i].setMap(p_map);
        if (infowindowStore[i] != null) {
            google.maps.event.addListener(infowindowStore[i], 'domready', function () {
                //$(".gm-style-iw").next("div").hide();

                // Reference to the DIV which receives the contents of the infowindow using jQuery
                var iwOuter = $('.gm-style-iw');
                //hiding cross
                iwOuter.next("div").hide();
                /* The DIV we want to change is above the .gm-style-iw DIV.
                 * So, we use jQuery and create a iwBackground variable,
                 * and took advantage of the existing reference to .gm-style-iw for the previous DIV with .prev().
                 */
                var iwBackground = iwOuter.prev();

                // Remove the background shadow DIV
                iwBackground.children(':nth-child(2)').css({ 'display': 'none' });

                // Remove the white background DIV
                iwBackground.children(':nth-child(4)').css({ 'display': 'none' });

            });
            infowindowStore[i].open(p_map, markersStore[i]);
        }
    }
}

function setflightPath(p_map) {
    for (var i = 0; i < flightPathStore.length; i++) {
        flightPathStore[i].setMap(p_map);
    }
}
function setConnectionLines(p_map) {
    for (var i = 0; i < connectionLinesStore.length; i++) {
        connectionLinesStore[i].setMap(p_map);
    }
}

function showMessage(message) {
    map.innerHTML = '<strong>' + message + '</strong>';
}



