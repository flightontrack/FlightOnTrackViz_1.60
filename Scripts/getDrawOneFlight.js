
function getDrawOneFlight(jsonFlight, imgstartpoint,imgendpoint,imgtracepoint,imgintermpoint) {

    var pointList = [];
    var uniqueFligts = [];
    var iconimage;
    var isPushpin;
    //var q = src='@Url.Content("~/Images/1484079763_green-pin.png"))'
    var tracestartpoint = {
        url: imgstartpoint,
        scaledSize: new google.maps.Size(28, 28),
        origin: new google.maps.Point(0, 0)
    }
    var traceendpoint={
        url: imgendpoint,
        scaledSize: new google.maps.Size(28, 28),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(0, 0)
    }
    var tracepoint = {
        url: imgtracepoint,
        scaledSize: new google.maps.Size(6, 6),
        origin: new google.maps.Point(0, 0)
    }
    var intermpoint = {
        url: imgintermpoint,
        scaledSize: new google.maps.Size(28, 28),
        origin: new google.maps.Point(0, 0)
    }

    //clean up map
    setMapOnAll(null);
    setflightPath(null);
    //clean up store arrays
    markersStore = [];
    flightPathStore = [];
    var Colors = [
        "#0000FF",
        "#00FF00",
        "#8000FF",
        "#808080",
        "#00FFFF",
        "#FF00FF"
    ];

    if (jsonFlight.length == 0) {
        showMessage("No flights to view");
        return;
    }
    else {
        //console.log('There is data to view.');
        $(jsonFlight).each(function (key, value) {

            //set point icon and array of unique flights
            if (uniqueFligts.indexOf($(this).attr('FlightID')) == -1) {
                //if next flight add the flight to list
                uniqueFligts.push($(this).attr('FlightID'));
            }

            if (key == 0) {
                //set pushpin on first position of the route                
                iconimage = tracestartpoint;
                isPushpin = true;
            }
            else if (key == jsonFlight.length - 1) {
                //set pushpin on last position of the route
                iconimage = intermpoint;
                isPushpin = true;
            }
            else if ($(this).attr('onSessionPointNum') == 1) {
                iconimage = intermpoint;
                isPushpin = true;
            }
            else {
                iconimage = tracepoint;
                isPushpin = false;
            }


            currentCenter = new google.maps.LatLng($(this).attr('latitude'), $(this).attr('longitude'));
            //set point infowindow
                var popupWindowText = "<table style=\"font-size:95%;font-family:arial,helvetica,sans-serif;\">"
                    + '<tr><td align=right>Flight #:&nbsp</td><td class="colcustom_1">' + $(this).attr('FlightID') + "</td></tr>"
                    + '<tr><td align=right>Point #:&nbsp</td><td class="colcustom_1">' + $(this).attr('onSessionPointNum') + "</td></tr>"
                    + '<tr><td align=right>GPS Time:&nbsp</td><td class="colcustom_1">' + $(this).attr('gpsTimeOnly') + "</td></tr>"
                    + '<tr><td align=right>Altitude (m):&nbsp</td><td class="colcustom_1">' + $(this).attr('AltitudeM') + "</td></tr>"
                    + '<tr><td align=right>Altitude (ft):&nbsp</td><td class="colcustom_1">' + $(this).attr('AltitudeFt') + "</td></tr>"
                    + '<tr><td align=right>Gr Speed (km/h):&nbsp</td><td class="colcustom_1">' + $(this).attr('SpeedKmpH') + "</td></tr>"
                + '<tr><td align=right>Gr Speed (knot):&nbsp</td><td class="colcustom_1">' + $(this).attr('SpeedKnot') + "</td></tr>"
                + ($(this).attr('AirportCode') == null ? "" : '<tr><td align=right>Airport:&nbsp</td><td class="colcustom_1">' + $(this).attr('AirportCode') + "</td></tr>")
            ;

            // array of trackpoint for markers and their infowindows and icons
            pointList[key] = new LatLng($(this).attr('latitude'), $(this).attr('longitude'), iconimage, "Flight" + $(this).attr('FlightID'), popupWindowText, $(this).attr('FlightID'), isPushpin);

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

            var infowindow = new google.maps.InfoWindow({
                content: p.infow
            });

            marker.addListener('mouseover', function () {
                infowindow.open(map, marker);
                // Reference to the DIV which receives the contents of the infowindow using jQuery
                var iwOuter = $('.gm-style-iw');

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
            marker.addListener('mouseout', function () {
                infowindow.close(map, marker);
            });
            markersStore.push(marker);
        });

        //build store array for lines separately
        uniqueFligts.forEach(function drawPoly(value, j) {
            var flight = value;
            var flightPointList = pointList.filter(function getPoly(val) { if (val.flightid == flight) return true; else false; })
            var flightPath = new google.maps.Polyline({
                path: flightPointList,
                strokeColor: j < Colors.length ? Colors[j] : "#0000FF",
                strokeOpacity: 0.8,
                strokeWeight: 3
            });

            flightPathStore.push(flightPath)
        });

        map.setCenter(currentCenter);
        //draw markers out of store
        setMapOnAll(map);
        // draw polyline out of store
        setflightPath(map);
        ///construct endpoints circles
        pointList.forEach(
            function (p, i) {
                if (p.ispushpin) {
                    var mapCircle = new google.maps.Circle({
                        strokeColor: 'blue',
                        strokeOpacity: 0.2,
                        strokeWeight: 15,
                        fillColor: 'red',
                        fillOpacity: 0.4,
                        map: map,
                        center: { lat: p.lat, lng: p.lng },
                        radius: 5
                    });
                    //circlesStore.push(mapCircle);
                }
            }
        )

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

function setMapOnAll() {
    // Sets  all markers in the store along with infowindow.
    for (var i = 0; i < markersStore.length; i++) {
        markersStore[i].setMap(map);
    }
}

function setflightPath() {
    for (var i = 0; i < flightPathStore.length; i++) {
        flightPathStore[i].setMap(map);
    }
}

function showMessage(message) {
    map.innerHTML = '<strong>' + message + '</strong>';
}



