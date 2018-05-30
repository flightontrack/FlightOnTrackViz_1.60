using System.Data.Entity;
using System.Linq;
using FontNameSpace.Models;
using System.Activities.Expressions;
using System.Collections.Generic;
using System;

namespace FontNameSpace.Helpers
{

    public class Queryables : IDisposable
    {

        public Entities db;
        int _acftId;
        int _flightId;
        int _routeId;
        int _topNumber = 100;
        //string _acftNumLocal;

        public Queryables() { db = new Entities(); }

        public Queryables(int Id, string idType = "Flight") {
            db = new Entities();
            if (idType == "Flight") _flightId = Id;
            if (idType == "Route") _routeId = Id;
        }
        public Queryables(Entities dbent) { db = dbent; }

        public int topNumber { set { _topNumber = value; } }
        public int pilotId { get; set; }
        public int acftId
        {
            set { _acftId = value; }
            get { return _acftId; }
        }
        public string acftNumLocal
        { get; set; }
        string _pilotUserName;
        public string pilotUserName {
            set {
                _pilotUserName = value;
                if (_pilotUserName != null) pilotId = db.Pilots.Where(r => r.PilotUserName.Equals(_pilotUserName)).First().PilotID;
            }
        }

        int? _airportId;
        public int? airportId { get; set; }
        string _airportCode;
        public string airportCode
        {
            set
            {
                _airportCode = value;
                if (_airportCode != null) {
                    var arpts = db.AirportCoordinates.Where(r => r.Code.Contains(_airportCode));
                    if (arpts != null && arpts.Count() == 1)
                    {
                        _airportId = arpts.FirstOrDefault().ID;
                        _airportCode = arpts.FirstOrDefault().Code;
                    }
                }
            }
            get { return _airportCode; }
        }

        public int flightId { set { _flightId = value; } }

        public Flight flight {
            get { return db.Flights.Find(_flightId); }
        }
     
        public DimAircraftRemote acftRemote
        {
            get { return db.DimAircraftRemotes.Find(_acftId); }
        }

        public Pilot pilot {
            get { return db.Pilots.Find(pilotId); }
        }

        public int companyIDint { get; set; }

        public IQueryable<Flight> flightsOnlyAll { get { return db.Flights; } }
        public IQueryable<vFlightAcftPilot> flightsAll { get { return db.vFlightAcftPilots.Where(r => r.FlightID > 0).OrderByDescending(r => r.FlightID); } }
        public IQueryable<vRoute> routesAll { get { return db.vRoutes; } }
        public IQueryable<vFlightAcftPilot> flightsGetGarbage { get { return db.vFlightAcftPilots.Where(r => r.IsJunk == true).OrderByDescending(r => r.FlightID); } }
        public IQueryable<vFlightAcftPilot> flightsAllTopNum { get { return db.vFlightAcftPilots.Where(r => r.FlightID > 0).OrderByDescending(row => row.FlightID).Take(_topNumber); } } //.AsNoTracking();
        //var q = db.vFlightAcftPilots.ToList();//.Where(row => row.IsShared == null ? false : (bool)row.IsShared).ToList();//.Where(row => row.IsJunk == false).OrderByDescending(row => row.FlightID);//.Take(TIMESPANFLIGHTS);
        public IQueryable<vPilotLogBook> pilotLogBook { get { return db.vPilotLogBooks.Where(row => row.PilotID == pilotId); } }
        public IQueryable<vVisualPilotLogBook> visualPilotLogBook { get { return db.vVisualPilotLogBook.Where(row => row.PilotID == pilotId); } }

        public IQueryable<vVisualPilotLogDestinations> vVisualPilotLogDestination { get { return db.vVisualPilotLogDestinations.Where(row => row.PilotID == pilotId); } }

        public Pilot pilotEntity { get { return db.Pilots.Find(pilotId); } }

        public IQueryable<GpsLocation> flightGpsLocations { get { return db.GpsLocations.Where(row => row.FlightID == _flightId); } }

        public IQueryable<GpsLocation> flightGpsLocationsOrderDesc { get { return db.GpsLocations.Where(row => row.FlightID == _flightId).OrderByDescending(row => row.GPSLocationID);} }

        public IQueryable<Flight> flightsJunkNotChecked { get { return db.Flights.Where(row => (row.IsAltitudeChecked.Value == null)); } }

        public IQueryable<Flight> flightsByRoute { get { return db.Flights.Where(row => (row.RouteID == _routeId)); } }

        public IQueryable<vFlightAcftPilot> flightsByPilot { get { return db.vFlightAcftPilots.Where(row => row.PilotID == pilotId); } }

        public IQueryable<vAircraftPilot> aircraftsByPilot { get { return db.vAircraftPilots.Where(r => r.PilotID == pilotId); } }
        public IQueryable<vAircraftPilot> aircraftsByAcftNumLocal { get { return db.vAircraftPilots.Where(row => row.AcftNumLocal == acftNumLocal); } }
        public IQueryable<AircraftPilot> aircraftsByCompany { get { return db.AircraftPilots.Where(r => r.CompanyID == companyIDint); } }


        ///Select Lista
        public IQueryable<vAircraftPilot> selList_vAircraftPilot
        {
            get
            { return db.vAircraftPilots.Where(f => f.PilotID == pilotId).OrderBy(row => row.AcftRegNum); }
        }
        public IQueryable<vListAircraft> selList_vAircraftDistinctAllPublic
        {
            get
            { return db.vListAircrafts.OrderBy(r => r.AcftNumLocal); }
        }
        public IQueryable<vListAircraft> selList_vAircraftDistinctWithFlightsPublic
        {
            get
            { return db.vListAircrafts.Where(r=>r.isFilghtExists==1).OrderBy(r => r.AcftNumLocal); }
        }
        public IQueryable<vAircraftPilot> selList_vAircraftPilotAll
        {
            get
            { return db.vAircraftPilots.OrderBy(row => row.AcftRegNum); }
        }

        public IQueryable<Pilot> selList_Pilot
        {
            get
            {
                return db.Pilots.OrderBy(p=>p.PilotUserName);
            }
        }

        public IQueryable<Flight> selList_FlightsByPilot
        {
            get
            {
                return db.Flights.OrderByDescending(f => f.FlightID).Where(f => f.PilotID == pilotId);
            }
        }
        public IQueryable<AirportCoordinates> airportEntity
        {
            get
            {
                return db.AirportCoordinates.Where(r => r.Code.Contains(_airportCode));
            }
        }

        public void Dispose()
        {
            ((IDisposable)db).Dispose();
        }
    }
}