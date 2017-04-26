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
        int _areaId;
        int _groupId;
        int _pilotId;
        int _flightId;
        List<int?> acftList;

        public ClassActiveFlights() {
            db = new Entities();
        }
        public ClassActiveFlights(Entities dbent) { db = dbent; }
        public int groupId
        {
            set { _groupId = value;
                //acftList = new List<int?>();
                acftList = db.AircraftGroups.Where(r => r.GroupID == _groupId).Select(r=>r.AcftID).ToList();
            }
            get { return _groupId; }
        }

        public IQueryable<vFlightAcftPilot> acftGroupFlightsActive {
            get { return db.vFlightAcftPilots.Where(r => r.isInFlight == TRUE).Where(r => acftList.Contains(r.AcftID)).Where(r => r.Points > 0); }
        }
        public IQueryable<GpsLocation> flightLastGpsLocation { get { return db.GpsLocations.Where(row => row.FlightID == _flightId); } }
        public object getAreaCenter()
        {
            var fs = db.vFlightAcftPilots.Where(r => r.isInFlight == TRUE).Where(r => acftList.Contains(r.AcftID)).Where(r => r.Points > 0);
            int? delay = 10000;
            int fId = 0;
            foreach (var f in fs)
            {
                if (f.UpdateDelay < delay) { delay = f.UpdateDelay;fId = f.FlightID; }
            }
            //var f = fs.Where(r=>r.UpdateDelay=)
            return new {lat =1,lng =2 };
        }
    }

}