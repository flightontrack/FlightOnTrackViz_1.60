function ActveFlightsPolyLines(jsonFlights,jsonAreaCenter) {

    if (jsonFlights.length == 0) {
        console.log("No active flights");
        return;
    }
    else {
        var pointList = [];
        var uniqueFligts = [];
        var mapOptions;
        var popupWindowText;
        var cookieName = "fontZoomLevel=";
        var defaultZoomLevel = 13;
        var active_pushpin = "http://maps.google.com/mapfiles/kml/paddle/red-circle.png";
        var frozen_pushpin = "http://maps.google.com/mapfiles/kml/paddle/blu-blank.png";
        var tracepoint = "/Images/flg_blue.png"
        var iconimage;

        $(jsonFlights).each(function (key, value) {

            var pushpin = $(this).attr('isPositionCurrent') == 1 ? active_pushpin : frozen_pushpin;
            //set point icon and array of unique flights
            if (key == 0) {
                //setting pushpin on last position of last flight
                iconimage = pushpin;
                //add the last flight to flight list
                uniqueFligts.push($(this).attr('FlightID'));
            }
            else
                if (uniqueFligts.indexOf($(this).attr('FlightID')) == -1) {
                    //if next flight add the flight to list
                    uniqueFligts.push($(this).attr('FlightID'));
                    //set pushpin on last position of the flight
                    iconimage = pushpin;
                }
                else {
                    iconimage = tracepoint;
                }

            //set point infowindow
            var popupWindowText = "<table border=0 style=\"font-size:95%;font-family:arial,helvetica,sans-serif;\">"
                + "<tr><td align=right>Flight #:</td><td colspan=2>" + $(this).attr('FlightID') + "</td></tr>"
                + "<tr><td align=right>GPS Time:</td><td colspan=2>" + $(this).attr('gpsTimeOnly') + "</td></tr>"
                + "<tr><td align=right>Altitude (m):</td><td colspan=2>" + $(this).attr('AltitudeM') + "</td></tr>"
                + "<tr><td align=right>Altitude (ft):</td><td colspan=2>" + $(this).attr('AltitudeFt') + "</td></tr>"
                + "<tr><td align=right>Gr Speed (km/h):</td><td>" + $(this).attr('SpeedKmpH') + "</td></tr>"
                + "<tr><td align=right>Gr Speed (knot):</td><td>" + $(this).attr('SpeedKnot') + "</td></tr>";

            // array of trackpoint for markers and their infowindows and icons
            pointList[key] = new LatLng($(this).attr('latitude'), $(this).attr('longitude'), iconimage, "Point", popupWindowText, $(this).attr('FlightID'));
        });

        // set the map center dd zoom cookie
        //if (key == $(jsonFlights).length - 1) {
        //var zoomLevel = (getCookie(cookieName) == '' ? defaultZoomLevel : parseInt(getCookie(cookieName)));
        //mapOptions = {
        //    zoom: zoomLevel,
        //    mapTypeId: google.maps.MapTypeId.ROADMAP,
        //    center: new google.maps.LatLng(jsonAreaCenter[0]["CenterLat"], jsonAreaCenter[0]["CenterLong"])
        //    //center: new google.maps.LatLng($(this).attr('latitude'), $(this).attr('longitude'))
        //};
        //map = new google.maps.Map(document.getElementById('map_div'), mapOptions);
        //google.maps.event.addListener(map, 'zoom_changed', function () {
        //    var zooooom = map.getZoom();
        //    document.cookie = cookieName + zooooom;
        //});
        //}
        
        // set tracepoint marker and infowindow
        pointList.forEach(
        function (p, i) {
            var marker = new google.maps.Marker(
                {   position: p,
                    icon: p.micon
                }
              );
            marker.setMap(map);
            var infowindow = new google.maps.InfoWindow({ content: p.infow });
            google.maps.event.addListener(marker, 'click', function () { infowindow.open(map, marker); });
        });

        // set polyline for each individual flight
        uniqueFligts.forEach(function drawPoly(value,j) {
            var flight = value;
            //buid new list of points for the flight taht pass true/false test
            var flightPointList = pointList.filter(function getPoly(val) { if (val.flightid == flight) return true; else false; })
            var flightPath = new google.maps.Polyline({
                path: flightPointList,
                strokeColor: "#0000FF",
                strokeOpacity: 0.8,
                strokeWeight: 3
            });
        flightPath.setMap(map);
        });
    }
}
//---------------------------------------------------------------
function getCookie(cname) {
    var cookieArray = document.cookie.split(';');
    if (cookieArray[0].indexOf(cname) == 0) return cookieArray[0].split('=')[1];
    return "";
}
//---------------------------------------------------------------
LatLng = function (rawLat, rawLng, micon, title, infow, flightid) { // (Number, Number)
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
    this.flightid = flightid
};





