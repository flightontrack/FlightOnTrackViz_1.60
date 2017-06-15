using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_Acft_Track.Models;

namespace MVC_Acft_Track.ViewModels
{
    public class vmFlightObjects
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
        public int? FlightAcftId;
        public string AcftNum = String.Empty;

        public vmFlightObjects(int id) {
            Flightid = id;
            var entities = new Entities();
            Flight = entities.Flights.Find(id);
            FlightName = Flight.FlightName;
            RouteId = Flight.RouteID??id;
            IsPattern = Flight.IsPattern.HasValue ? "Yes" : "No";
            FlightDuration = Flight.FlightDurationMin??0;
            TrackFreq = Flight.FreqSec;
            FlightDate = Flight.FlightDate;
            IsShared = Flight.IsShared;
            GpsLocations = entities.GpsLocations.Where(row => row.FlightID == id).OrderBy(row => row.onSessionPointNum).ToList();
            var pilot = new vmPilot(Flight.PilotID ?? 0);
            if (pilot.PilotId != null)
            {
                PltId = pilot.PilotId;
                FlightPilotCode = pilot.PilotCode;
                FlightPilotName = pilot.PilotName;
            }
            FlightAcftId = Flight.AcftID;
            AcftNum = entities.DimAircraftRemotes.Find(FlightAcftId).AcftNum;
        }
    }

    public class vmPilot
    {
        public int? PilotId;
        public Pilot thePilot;
        public string PilotCode;
        public string PilotName;

        public vmPilot(int id)
        {
            var entities = new Entities();
            var p = entities.Pilots.Find(id);
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