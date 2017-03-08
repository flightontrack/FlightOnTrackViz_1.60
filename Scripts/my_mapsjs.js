function hasMap() {
    if (routeSelect.selectedIndex == 0) { // means no map
        return false;
    }
    else {
        return true;
    }
}

function CreatePolyLine(json) {
    console.log("CreatePolyLine");
    if (json.length == 0) {
        showMessage('There is no tracking data to view.');
        map.innerHTML = '';
    }
    else {
        var finalLocation = 0;
        var startLocation = 0;
        var counter = 0;
        var pointList = [];
        var nonUniqueFligts = [];
        var uniqueFligts = [];
        var mapOptions;
        var finalPoint;
        var popupWindowText;
        var cookieName = "fontZoomLevel=";
        var defaultZoomLevel = 13;

        // iterate through the locations and create map markers for each location
        $(json).each(function(key, value){

            var popupWindowText = "<table border=0 style=\"font-size:95%;font-family:arial,helvetica,sans-serif;\">"
                + "<tr><td align=right>Flight #:</td><td colspan=2>" + $(this).attr('FlightID') + "</td></tr>"
                + "<tr><td align=right>Point #:</td><td colspan=2>" + $(this).attr('onSessionPointNum') + "</td></tr>"
                + "<tr><td align=right>GPS Time:</td><td colspan=2>" + $(this).attr('gpsTimeOnly') + "</td></tr>"
                + "<tr><td align=right>Altitude (m):</td><td colspan=2>" + $(this).attr('AltitudeM') + "</td></tr>"
                + "<tr><td align=right>Altitude (ft):</td><td colspan=2>" + $(this).attr('AltitudeFt') + "</td></tr>"
                + "<tr><td align=right>Gr Speed (km/h):</td><td>" + $(this).attr('SpeedKmpH') + "</td></tr>"
            + "<tr><td align=right>Gr Speed (knot):</td><td>" + $(this).attr('SpeedKnot') + "</td></tr>";
            //+ "<tr><td align=right>Airport:</td><td>" + $(this).attr('AirportCode') + "</td></tr>";

            // want to set the map center on the last location
            if (counter == $(json).length-1) {
                var zoomLevel = (getCookie(cookieName) == '' ? defaultZoomLevel : parseInt(getCookie(cookieName)));

                mapOptions = {
                    zoom: zoomLevel,
                    //zoomControl: true,
                    //zoomControlOptions: {
                    //    style: google.maps.ZoomControlStyle.LARGE
                    //},
                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                    center: new google.maps.LatLng($(this).attr('latitude'), $(this).attr('longitude'))
                };
                map = new google.maps.Map(document.getElementById('map_div'), mapOptions);
                google.maps.event.addListener(map, 'zoom_changed', function () {
                    var zooooom = map.getZoom();
                    document.cookie = cookieName + zooooom;
                });
                pointList[pointList.length] = new LatLng($(this).attr('latitude'), $(this).attr('longitude'), "/Images/dot_red.png", "Destination Point", popupWindowText, $(this).attr('FlightID'));
                finalLocation = counter;
            }
            else if (counter == 0) {
                startLocation = counter;
                pointList[counter] = new LatLng($(this).attr('latitude'), $(this).attr('longitude'), "/Images/dot_green.png", "Reference Point ", popupWindowText, $(this).attr('FlightID'));
            }
            else pointList[counter] = new LatLng($(this).attr('latitude'), $(this).attr('longitude'), "/Images/flg_blue.png", "Strating Point", popupWindowText, $(this).attr('FlightID'));
            nonUniqueFligts[counter] = new $(this).attr('FlightID');
            counter++;

        });


        pointList.forEach(
        function (p, i) {
            var marker = new google.maps.Marker(
                {   position: p,
                    icon: p.micon
                }
              );
            marker.setMap(map);
            var infowindow = new google.maps.InfoWindow({ content: p.infow });
            google.maps.event.addListener(marker, 'click', function () {
                infowindow.open(map, marker);
            });
        });

        uniqueFligts = array_unique(nonUniqueFligts);
        var Colors = [
        "#0000FF",
        "#00FF00",
        "#8000FF",
        "#808080",
        "#00FFFF",
        "#FF00FF"
        ];
        // draw polyline
        uniqueFligts.forEach(
            function drawPoly(value,j) {
                var flight = value;
                var flightPointList = pointList.filter(function getPoly(val) { if (val.flightid == flight) return true; else false; })
                var color = j<Colors.length?Colors[j]:"#0000FF"
                var flightPath = new google.maps.Polyline({
                    path: flightPointList,
                    strokeColor: color, 
                    strokeOpacity: 0.8,
                    strokeWeight: 2
                });
                flightPath.setMap(map);
            }
            );
    }
}

function array_unique(array) {
    var unique = [];
    for (var i = 0 ; i < array.length ; ++i) {
        if (unique.indexOf(array[i]) == -1)
            unique.push(array[i]);
    }
    return unique;
}

function createMarker(latitude, longitude, speed, direction, distance, locationMethod, phoneNumber, sessionID, accuracy, RowNum, onMapDate, onMapTime, extraInfo, map, finalLocation, startLocation) {
                      
    if (finalLocation) iconUrl = "https://google-maps-icons.googlecode.com/files/aircraft-small.png ";
    else if (startLocation) iconUrl = "https://google-maps-icons.googlecode.com/files/airplane-tourism.png";
    else iconUrl = 'images/dot_cross.png';
    //if (finalLocation) iconUrl = 'images/bullet.png';
    //else if (startLocation) iconUrl = 'images/dot_green.png';
    //else iconUrl = 'images/dot_cross.png';


     //var dot_icon_var = {
    //    path: google.maps.SymbolPath.CIRCLE,
    //    fillColor: 'red',
    //    fillOpacity: .4,
    //    scale: 4.5,
    //    strokeColor: 'white',
    //    strokeWeight: 1
    //};


    //var lastMarker = "</td></tr>";

    //// when a user clicks on last marker, let them know it's final one
    //if (finalLocation) {

    //    lastMarker = "</td></tr><tr><td align=left>&nbsp;</td><td><b>Final location</b></td></tr>";
    //}

    //// convert from meters to feet
    //accuracy = parseInt(accuracy * 3.28);

    var popupWindowText = "<table border=0 style=\"font-size:95%;font-family:arial,helvetica,sans-serif;\">"
        //+ "<tr><td align=right>&nbsp;</td><td>&nbsp;</td><td rowspan=2 align=right>"
        + "<tr><td align=right>Acft:</td><td>" + phoneNumber + "</td><td>&nbsp;</td></tr>"
        + "<tr><td align=right>Point:</td><td colspan=2>" + RowNum + "</td></tr>"
        + "<tr><td align=right>Date:</td><td colspan=2>" + onMapDate + "</td></tr>"
        + "<tr><td align=right>Time:</td><td colspan=2>" + onMapTime + "</td></tr>"
        + "<tr><td align=right>Altitude:</td><td colspan=2>" + extraInfo + "</td></tr>"
        //+ "<img src=images/" + getCompassImage(direction) + ".jpg alt= />" + lastMarker
        + "<tr><td align=right>Speed:</td><td>" + speed + " mph</td></tr>";
        //+ "<tr><td align=right>Distance:</td><td>" + distance + " mi</td><td>&nbsp;</td></tr>";

        //+ "<tr><td align=right>Method:</td><td>" + locationMethod + "</td><td>&nbsp;</td></tr>"
        //+ "<tr><td align=right>Session ID:</td><td>" + sessionID + "</td><td>&nbsp;</td></tr>"
        //+ "<tr><td align=right>Accuracy:</td><td>" + accuracy + " ft</td><td>&nbsp;</td></tr>"
        //+ "<tr><td align=right>Extra Info:</td><td>" + extraInfo + "</td><td>&nbsp;</td></tr></table>";

    ////L.marker(new L.LatLng(latitude, longitude), { icon: markerIcon }).bindPopup(popupWindowText).addTo(map);
    //if (finalLocation) {
    //    L.marker(new L.LatLng(latitude, longitude), { icon: dot_icon }).bindPopup(popupWindowText).addTo(map).openPopup();
    //}
    //else {
    //    L.marker(new L.LatLng(latitude, longitude), { icon: dot_icon }).bindPopup(popupWindowText).addTo(map);
    //}

}

// this chooses the proper image for our litte compass in the popup window
function getCompassImage(azimuth) {
    if ((azimuth >= 337 && azimuth <= 360) || (azimuth >= 0 && azimuth < 23))
            return "compassN";
    if (azimuth >= 23 && azimuth < 68)
            return "compassNE";
    if (azimuth >= 68 && azimuth < 113)
            return "compassE";
    if (azimuth >= 113 && azimuth < 158)
            return "compassSE";
    if (azimuth >= 158 && azimuth < 203)
            return "compassS";
    if (azimuth >= 203 && azimuth < 248)
            return "compassSW";
    if (azimuth >= 248 && azimuth < 293)
            return "compassW";
    if (azimuth >= 293 && azimuth < 337)
            return "compassNW";

    return "";
}

function deleteRoute() {
    if (hasMap()) {
		
		// comment out these two lines to get delete working
		var answer = confirm("Disabled here on test website, this works fine.");
		return false;
		
        var answer = confirm("This will permanently delete this route\n from the database. Do you want to delete?");
        if (answer){
            showWaitImage('Deleting route...');
            var url = 'DeleteRoute.aspx' + routeSelect.options[routeSelect.selectedIndex].value;

            $.ajax({
                   url: url,
                   type: 'GET',
                   success: function() {
                      deleteRouteResponse();
                   }
               });
        }
        else {
            return false;
        }
    }
    else {
        alert("Please select a route before trying to delete.");
    }
}

function deleteRouteResponse() {
    map.innerHTML = '';
    routeSelect.length = 0;

    $.ajax({
           url: 'GetRoutes.aspx',
           type: 'GET',
           success: function(data) {
              loadRoutes(data);
           }
       });
}

// auto refresh the map. there are 3 transitions (shown below). transitions happen when a user
// selects an option in the auto refresh dropdown box. an interval is an amount of time in between
// refreshes of the map. for instance, auto refresh once a minute. in the method below, the 3 numbers
// in the code show where the 3 transitions are handled. setInterval turns on a timer that calls
// the getRouteForMap() method every so many seconds based on the value of newInterval.
// clearInterval turns off the timer. if newInterval is 5, then the value passed to setInterval is
// 5000 milliseconds or 5 seconds.
function autoRefresh() {
    /*
        1) going from off to any interval
        2) going from any interval to off
        3) going from one interval to another
    */

    if (hasMap()) {
        newInterval = refreshSelect.options[refreshSelect.selectedIndex].value;

        if (currentInterval > 0) { // currently running at an interval

            if (newInterval > 0) { // moving to another interval (3)
                clearInterval(intervalID);
                intervalID = setInterval("getRouteForMap();", newInterval * 1000);
                currentInterval = newInterval;
            }
            else { // we are turning off (2)
                clearInterval(intervalID);
                newInterval = 0;
                currentInterval = 0;
            }
        }
        else { // off and going to an interval (1)
            intervalID = setInterval("getRouteForMap();", newInterval * 1000);
            currentInterval = newInterval;
        }

        // show what auto refresh action was taken and after 5 seconds, display the route name again
        showMessage(refreshSelect.options[refreshSelect.selectedIndex].innerHTML);
        setTimeout('showRouteName();', 5000);
    }
    else {
        alert("Please select a route before trying to refresh map.");
        refreshSelect.selectedIndex = 0;
    }
}

function changeZoomLevel() {
    if (hasMap()) {
        zoomLevel = zoomLevelSelect.selectedIndex + 1;

        getRouteForMap();

        // show what zoom level action was taken and after 5 seconds, display the route name again
        showMessage(zoomLevelSelect.options[zoomLevelSelect.selectedIndex].innerHTML);
        setTimeout('showRouteName();', 5000);
    }
    else {
        alert("Please select a route before selecting zoom level.");
        zoomLevelSelect.selectedIndex = zoomLevel - 1;
    }
}

function showMessage(message) {
     messages.innerHTML = 'GpsTracker: <strong>' + message + '</strong>';
}

function showRouteName() {
    showMessage(routeSelect.options[routeSelect.selectedIndex].innerHTML);
}

function showWaitImage(theMessage) {
    map.innerHTML = '<img src="images/ajax-loader.gif" style="position:absolute;top:225px;left:325px;">';
    showMessage(theMessage);
}

function hideWaitImage() {
    map.innerHTML = '';
   // messages.innerHTML = 'GpsTracker';
}

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





