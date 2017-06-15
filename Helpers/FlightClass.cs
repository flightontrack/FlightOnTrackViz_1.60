using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_Acft_Track.Models;

namespace MVC_Acft_Track.Helpers
{
    public class FlightClass : Flight
    {
        int flightId;
        Flight flight;
        FlightClass(int id) {
            flight = new Entities().Flights.Find(id);
        }
            //        var isFlightShared = db.Flights.Where(row => row.FlightID == id).First().IsShared;
            //if (Request.IsAuthenticated || (bool) isFlightShared)
            //{
            //    var gpslocations = db.GpsLocations.Where(row => row.FlightID == id).OrderBy(row => row.onSessionPointNum).ToList();
            //    if (gpslocations == null)
            //    {
            //        return HttpNotFound();
            //    }
            //    ViewBag.Flightid = id;
            //    var routeid =db.Flights.Find(id).RouteID;
            //    ViewBag.Routeid = routeid == null ? id : routeid;
            //    ViewBag.Flightname = db.Flights.Find(id).FlightName;
            //    var acftid = db.Flights.Find(id).AcftID;
            //    var pltid = db.Flights.Find(id).PilotID;
            //    var isPilotExists = db.Pilots.Find(pltid);
            //    var ispattern = db.Flights.Find(id).IsPattern.HasValue ?db.Flights.Find(id).IsPattern : false;
            //    ViewBag.FlightAircraft = acftid != null ? db.DimAircraftRemotes.Find(acftid).AcftNum : String.Empty;
            //    ViewBag.FlightPilotCode = pltid != null ? db.Pilots.Find(pltid).PilotCode : String.Empty;
            //    ViewBag.FlightPilotName = pltid != null ? db.Pilots.Find(pltid).PilotName : String.Empty;
            //    ViewBag.FlightIsPattern = (bool)ispattern ? "Yes" : "No";
            //    ViewBag.FlightDuration = db.Flights.Find(id).FlightDurationMin;
            //    ViewBag.TrackFreq = db.Flights.Find(id).FreqSec;
            //    ViewBag.FlightDate = db.Flights.Find(id).FlightDate; // "dummy"; // item.FlightDateUTC.ToString("d",(new System.Globalization.CultureInfo("ja-JP")).DateTimeFormat)
            //    //ViewBag.ActionBack = actionBack;
            //    ViewBag.AcftId = acftid;
    }
}