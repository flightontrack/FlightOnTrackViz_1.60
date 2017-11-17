using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_Acft_Track.Models;
using MVC_Acft_Track.Helpers;

namespace MVC_Acft_Track.ViewModels
{
    public class vmFlight
    {
        public int Flightid;
        public Flight Flight;
        public List<GpsLocation> GpsLocations;
        public int RouteId;
        public string FlightName;
        public string IsPattern;
        public int FlightDuration;
        public int? TrackFreq;
        public DateTime? FlightDate;
        public int? FlightPltId;
        public bool? IsShared;
        public int? PltId;
        public string FlightPilotCode = String.Empty;
        public string FlightPilotName = String.Empty;
        //public int? FlightAcftId;
        public string AcftNum = String.Empty;

        public vmFlight(int id) {
            Flightid = id;
            //var entities = new Entities();
            var q = new Queryables(id);
            Flight = q.flight;
            FlightName = Flight.FlightName;
            RouteId = Flight.RouteID ?? id;
            IsPattern = Flight.IsPattern.HasValue ? "Yes" : "No";
            FlightDuration = Flight.FlightDurationMin ?? 0;
            TrackFreq = Flight.FreqSec;
            FlightDate = Flight.FlightDate;
            IsShared = Flight.IsShared;
            //GpsLocations = entities.GpsLocations.Where(row => row.FlightID == id).OrderBy(row => row.onSessionPointNum).ToList();
            GpsLocations = q.flightGpsLocations.ToList();
            var pilot = new vmPilot(Flight.PilotID ?? 0);
            if (pilot.PilotId != null)
            {
                PltId = pilot.PilotId;
                FlightPilotCode = pilot.PilotCode;
                FlightPilotName = pilot.PilotName;
            }
            q.acftId = Flight.AcftID ?? 0;
            //FlightAcftId = Flight.AcftID;
            AcftNum = q.acftRemote.AcftNum;
        }
    }

    //public class vmFlightList
    //{
    //    public List<vmFlight> Flights;

    //} 
    public class vmPilot
    {
        public int? PilotId;
        public Pilot thePilot;
        public string PilotCode;
        public string PilotName;

        public vmPilot(int id)
        {
            //var entities = new Entities();
            var q = new Queryables();
            q.pilotId = id;
            var p = q.pilot;
            if (p != null)
            {
                thePilot = p;
                PilotId = p.PilotID;
                PilotCode = thePilot.PilotCode;
                PilotName = thePilot.PilotName;
            }
        }
    }
}