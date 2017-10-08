using MVC_Acft_Track.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Acft_Track.Helpers
{
    public static class ClassShared
    {
        private static Entities db = new Entities();
        public static void FormHandle(string buttonClicked, List<vFlightAcftPilot> flightList)
        {
            if (buttonClicked.Equals("DeleteSelectedFlights"))
            {
                foreach (var f in flightList)
                {
                    if (f.IsChecked ?? false)
                    {
                        Flight flight = db.Flights.Find(f.FlightID);
                        db.Flights.Remove(flight);
                        db.SaveChanges();
                    }
                }
                return;
            }
            if (buttonClicked.Equals("MergeSelectedFlights"))
            {
                ArrayList flightsToMerge = new ArrayList();
                foreach (var f in flightList)
                {
                    if (f.IsChecked ?? false)
                    {
                        flightsToMerge.Add((string)f.FlightID.ToString());
                    }
                }
                flightsToMerge.Sort();
                string flightsToMergeString = String.Join(",", (string[])flightsToMerge.ToArray(typeof(string)));
                db.merge_Flights(flightsToMergeString);
                return;
            }
            if (buttonClicked.Equals("SaveChanges"))
            {
                DateTime timeUtcNow = DateTime.UtcNow;
                foreach (var f in flightList)
                {
                    Flight flight = db.Flights.Find(f.FlightID);
                    if ((f.IsShared ?? false) ^ (flight.IsShared ?? false))
                    {
                        flight.IsShared = (f.IsShared ?? false);
                        flight.Updated = timeUtcNow;
                        db.Flights.Attach(flight);
                        db.Entry(flight).Property(p => p.IsShared).IsModified = true;
                        db.Entry(flight).Property(p => p.Updated).IsModified = true;
                        db.SaveChanges();
                    };

                    if (f.IsJunk ^ (flight.IsJunk ?? false))
                    {
                        flight.IsJunk = f.IsJunk;
                        db.Flights.Attach(flight);
                        db.Entry(flight).Property(p => p.IsJunk).IsModified = true;
                        db.SaveChanges();
                    };

                    if (f.RouteID != (flight.RouteID.HasValue ? flight.RouteID : flight.FlightID))
                    {
                        flight.RouteID = f.RouteID;
                        flight.Updated = timeUtcNow;
                        db.Flights.Attach(flight);
                        db.Entry(flight).Property(p => p.RouteID).IsModified = true;
                        db.Entry(flight).Property(p => p.Updated).IsModified = true;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}