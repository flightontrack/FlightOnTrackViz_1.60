using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_Acft_Track.Helpers;
using System.Data.Entity;
using MVC_Acft_Track.Models;

namespace MVC_Acft_Track.ViewModels
{
    public class vmSearchRequest
    {
        public string flightID;
        public string airportID;
        public string acftNumLocal;
        public string pilotID;
        public string flightDate;
        public string companyID;
        public List<vFlightAcftPilot> flightList; 

        public vmSearchRequest(string p_flightID, string p_airportID, string p_acftNumLocal, string p_pilotID, string p_flightDate, string p_companyID) {
            flightID= p_flightID;
            airportID= p_airportID;
            acftNumLocal= p_acftNumLocal;
            pilotID= p_pilotID;
            flightDate= p_flightDate;
            companyID= p_companyID;

            var q = new qLINQ();
            var f = q.flightsAll;

            if (!string.IsNullOrEmpty(flightID))
            {
                int flightIDint = int.Parse(flightID);
                f = f.Where(row => row.FlightID == flightIDint);
            }
            if (!string.IsNullOrEmpty(flightDate))
            {
                var flightDatedate = DateTime.Parse(flightDate);
                f = f.Where(row => DbFunctions.TruncateTime(row.FlightDate) == flightDatedate);
            }
            //if (!string.IsNullOrEmpty(airportID))
            //{
            //    //flights = flights.Where(row => row.AcftID == int.Parse(aircraftID)).ToList();
            //}
            if (!string.IsNullOrEmpty(acftNumLocal))
            {
                q.acftNumLocal = acftNumLocal;
                var acftids = q.aircraftsByAcftNumLocal.Select(row => row.AcftID).ToList();
                f = f.Where(row => acftids.Contains(row.AcftID.Value));
            }
            if (!string.IsNullOrEmpty(companyID))
            {
                q.companyIDint = int.Parse(companyID);
                var acftids = q.aircraftsByCompany.Select(row => row.AcftID).ToList();
                f = f.Where(row => acftids.Contains(row.AcftID.Value));
            }
            if (!string.IsNullOrEmpty(pilotID))
            {
                var pilotIDint = int.Parse(pilotID);
                f = f.Where(row => row.PilotID == pilotIDint);
            }
            flightList = f.ToList();
        }
    }
}