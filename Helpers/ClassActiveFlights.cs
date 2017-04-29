using System.Data.Entity;
using System.Linq;
using MVC_Acft_Track.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static MVC_Acft_Track.Finals;

namespace MVC_Acft_Track.Helpers
{
    public class ClassActiveFlights
    {
        public Entities db;
        int _groupId;
        int _areaId;
        List<int?> acftList;
        List<int> _flightIdList;

        public List<int> flightIdList
        {
            set { _flightIdList = value; }
            get { return _flightIdList; }
        }

        public ClassActiveFlights() {
            db = new Entities();
        }
        public ClassActiveFlights(Entities dbent) { db = dbent; }
        public int groupId
        {
            set { _groupId = value;
                acftList = db.AircraftGroups.Where(r => r.GroupID == _groupId).Select(r=>r.AcftID).ToList();
            }
            get { return _groupId; }
        }
        public int areaId
        {
            set
            {
                _areaId = value;
            }
            get { return _areaId; }
        }
        public IQueryable<vFlightAcftPilot> flightsFlights
        {
            get
            { return db.vFlightAcftPilots.Where(r => flightIdList.Contains(r.FlightID)); }
        }

        public IQueryable<vFlightAcftPilot> acftGroupFlightsActive {
            get
            { return db.vFlightAcftPilots.Where(r => r.isInFlight == TRUE).Where(r => acftList.Contains(r.AcftID)).Where(r => r.Points > 0); }
        }
        public Array acftGroupFlightIDsActive
        {
            get
            { return db.vFlightAcftPilots.Where(r => r.isInFlight == TRUE).Where(r => acftList.Contains(r.AcftID)).Where(r => r.Points > 0).Select(r=>r.FlightID).ToArray(); }
        }
        public object areaCenter() {
            return db.DimAreas.Where(r => r.AreaID.Equals(_areaId)).Select(r => new { r.CenterLat, r.CenterLong }); 
        }

        public Location getGroupCenter()
        {
            /// centering area on latest updated flight
            var fs = db.vFlightAcftPilots.Where(r => r.isInFlight == TRUE).Where(r => acftList.Contains(r.AcftID)).Where(r => r.Points > 0);
            //var loc = new Location();
            if (fs.Count() == 0) { return new Location(); }
            int? delay = 10000;
            int fId = 0;
            foreach (var f in fs)
            {
                if (f.UpdateDelay < delay) { delay = f.UpdateDelay;fId = f.FlightID; }
            }
            var q = new qLINQ (db);
            q.flightId = fId;
            var loc = q.flightGpsLocationsOrderDesc.ToList()[0];
            return new Location { Lat = loc.latitude, Long = loc.longitude};
        }

        public Location getFlightsCenter()
        {
            if (flightsFlights.Count() == 0) { return new Location(); }
            int ? delay = 10000;
            int fId = 0;
            foreach (var f in flightsFlights)
            {
                if (f.UpdateDelay < delay) { delay = f.UpdateDelay; fId = f.FlightID; }
            }
            var q = new qLINQ(db);
            q.flightId = fId;
            var loc = q.flightGpsLocationsOrderDesc.ToList()[0];
            return new Location { Lat = loc.latitude, Long = loc.longitude };
        }

    }
    public class Location
    {
        public decimal Lat;
        public decimal Long;
    }
}