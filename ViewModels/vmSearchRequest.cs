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
        public int  topN = 100;
        public int  rowsPerPage;
        public int  totalRecordCount;
        public List<vFlightAcftPilot> flightList; 

        public vmSearchRequest(string p_flightID = null, string p_airportID = null, string p_acftNumLocal = null, string p_pilotID = null, string p_flightDate = null, string p_companyID = null,bool p_isSearchJunk = false, int p_rowsPerPage = 50) {
            flightID= p_flightID;
            airportID= p_airportID;
            acftNumLocal= p_acftNumLocal;
            pilotID= p_pilotID;
            flightDate= p_flightDate;
            companyID= p_companyID;
            rowsPerPage = p_rowsPerPage;

            var q = new qLINQ();
            var f = q.flightsAll;

            if (p_isSearchJunk)
            {
                f = f.Where(r => r.IsJunk == true);
                foreach (vFlightAcftPilot i in f) { i.IsChecked = true; }
                topN = rowsPerPage;
            }
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
            totalRecordCount = f.Count();
            flightList = f.Take(topN).ToList();
        }
    }
}