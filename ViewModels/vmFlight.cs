using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_Acft_Track.Models;

namespace MVC_Acft_Track.ViewModels
{
    public class vmFlight
    {
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

        public vmFlight(int id) {
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
            FlightPltId = Flight.PilotID??0;
            PltId = entities.Pilots.Find(Flight.PilotID ?? 0).PilotID;
            if (PltId != null)
            {
                FlightPilotCode = entities.Pilots.Find(PltId).PilotCode;
                FlightPilotName = entities.Pilots.Find(PltId).PilotName;
            }
            FlightAcftId = Flight.AcftID;
            AcftNum = entities.DimAircraftRemotes.Find(FlightAcftId).AcftNum;
        }
    }
}