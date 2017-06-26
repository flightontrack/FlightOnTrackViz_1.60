﻿using System.Data.Entity;
using System.Linq;
using MVC_Acft_Track.Models;
using System.Activities.Expressions;
using System.Collections.Generic;

namespace MVC_Acft_Track.Helpers
{

    public class qLINQ
    {

        public Entities db;
        int _pilotId;
        int _acftId;
        int _flightId;
        int _topNumber = 100;
        //Flight flight;

        public qLINQ() { db = new Entities(); }

        public qLINQ(int flightId) { _flightId = flightId; }
        public qLINQ(Entities dbent) { db = dbent; }

        public int topNumber { set { _topNumber = value; } }
        public int pilotId {
            set { _pilotId = value; }
            get { return _pilotId; }
        }

        public int acftId
        {
            set { _acftId = value; }
            get { return _acftId; }
        }
        string _pilotUserName;
        public string pilotUserName {
            set {
                _pilotUserName = value;
                if (_pilotUserName != null) pilotId = db.Pilots.Where(r => r.PilotUserName.Equals(_pilotUserName)).First().PilotID;
            }
        }

        int? _airportId;
        public int? airportId
        {
            set { _airportId = value; }
            get { return _airportId; }
        }
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
            get { return db.Pilots.Find(_pilotId); }
        }

        public IQueryable<vFlightAcftPilot> flightsAll { get { return db.vFlightAcftPilots.Where(r => r.FlightID > 0).OrderByDescending(r => r.FlightID); } }
        public IQueryable<vFlightAcftPilot> flightsAllTopNum { get { return db.vFlightAcftPilots.Where(r => r.FlightID > 0).OrderByDescending(row => row.FlightID).Take(_topNumber); } } //.AsNoTracking();
        //var q = db.vFlightAcftPilots.ToList();//.Where(row => row.IsShared == null ? false : (bool)row.IsShared).ToList();//.Where(row => row.IsJunk == false).OrderByDescending(row => row.FlightID);//.Take(TIMESPANFLIGHTS);
        public IQueryable<vPilotLogBook> pilotLogBook { get { return db.vPilotLogBooks.Where(row => row.PilotID == _pilotId).OrderByDescending(row => row.RouteID); } }
        public IQueryable<vVisualPilotLogBook> visualPilotLogBook { get { return db.vVisualPilotLogBook.Where(row => row.PilotID == _pilotId).OrderByDescending(row => row.RouteID); } }

        public IQueryable<vVisualPilotLogDestinations> vVisualPilotLogDestinations { get { return db.vVisualPilotLogDestinations.Where(row => row.PilotID == _pilotId).OrderBy(row => row.FlightID); } }

        public Pilot pilotEntity { get { return db.Pilots.Find(_pilotId); } }

        public IQueryable<GpsLocation> flightGpsLocations { get { return db.GpsLocations.Where(row => row.FlightID == _flightId); } }

        public IQueryable<GpsLocation> flightGpsLocationsOrderDesc { get { return db.GpsLocations.Where(row => row.FlightID == _flightId).OrderByDescending(row => row.GPSLocationID);} }

        public IQueryable<Flight> flightsJunkNotChecked { get { return db.Flights.Where(row => (row.IsAltitudeChecked.Value == null)); } }

        public IQueryable<vFlightAcftPilot> flightsByPilot { get { return db.vFlightAcftPilots.Where(row => row.PilotID == pilotId); } }

        public IQueryable<vAircraftPilot> pilotAircrafts { get { return db.vAircraftPilots.Where(r => r.PilotID == pilotId); } }


        ///Select Lista
        public IQueryable<vAircraftPilot> selList_vAircraftPilot
        {
            get
            { return db.vAircraftPilots.OrderBy(row => row.AcftRegNum); }
        }

        //public List<Pilot> selList_Pilot
        //{
        //    get
        //    {
        //        return db.Pilots.AsEnumerable().Select(p => new Pilot {PilotID=p.PilotID, PilotUserName=p.PilotUserName }).ToList();
        //    }
        //}

        public IQueryable<Pilot> selList_Pilot
        {
            get
            {
                return db.Pilots.OrderBy(p=>p.PilotUserName);
            }
        }
        public IQueryable<AirportCoordinates> airportEntity
        {
            get
            {
                return db.AirportCoordinates.Where(r => r.Code.Contains(_airportCode));
            }
        }
    }
}