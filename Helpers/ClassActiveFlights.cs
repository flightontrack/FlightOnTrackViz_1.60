using System.Data.Entity;
using System.Linq;
using MVC_Acft_Track.Models;
using System;
using System.Collections.Generic;
using System.Web;
using static MVC_Acft_Track.Finals;

namespace MVC_Acft_Track.Helpers
{
    public class ClassActiveFlights
    {
        public Entities db;
        int _groupId;
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

        public IQueryable<vFlightAcftPilot> inflightFlights
        {
            get
            { return db.vFlightAcftPilots.Where(r => r.isInFlight == TRUE); }
        }

        public IQueryable<vFlightAcftPilot> flightsFlights
        {
            get
            { return inflightFlights.Where(r => flightIdList.Contains(r.FlightID)); }
        }
        public IQueryable<vFlightAcftPilot> acftGroupFlightsActive
        {
            get
            { return inflightFlights.Where(r => acftList.Contains(r.AcftID)); }
        }

        public ClassArea.AreaCircle getAcftGroupMapCircle()
        {
            /// centering area on latest updated flight
            var fs = inflightFlights.Where(r => acftList.Contains(r.AcftID)).ToList();
            //var loc = new Location();
            if (fs.Count() == 0) { return new ClassArea.AreaCircle(); }
            int? delay = 10000;
            int fId = 0;
            foreach (var f in fs)
            {
                if (f.UpdateDelay < delay) { delay = f.UpdateDelay;fId = f.FlightID; }
            }
            var q = new qLINQ (db);
            q.flightId = fId;
            var loc = q.flightGpsLocationsOrderDesc.ToList()[0];
            return new ClassArea.AreaCircle { Lat = loc.latitude, Long = loc.longitude, Radius = DEFAULT_AREARADIUS };
        }

        public ClassArea.AreaCircle getFlightsMapCircle()
        {
            if (flightsFlights.Count() == 0) { return new ClassArea.AreaCircle(); }
            int ? delay = 10000;
            int fId = 0;
            foreach (var f in flightsFlights)
            {
                if (f.UpdateDelay < delay) { delay = f.UpdateDelay; fId = f.FlightID; }
            }
            var q = new qLINQ(db);
            q.flightId = fId;
            var loc = q.flightGpsLocationsOrderDesc.ToList()[0];
            return new ClassArea.AreaCircle { Lat = loc.latitude, Long = loc.longitude, Radius = DEFAULT_AREARADIUS  };
        }

    }
}