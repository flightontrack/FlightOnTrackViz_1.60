using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_Acft_Track.Helpers;
using System.Data.Entity;
using MVC_Acft_Track.Models;

namespace MVC_Acft_Track.ViewModels
{
    public class vmSearchRequestFights
    {
        public int topN { get; set; }
        public int rowsPerPage { get; set; }
        public int totalRecordCount { get; set; }
        public vmSearchRequest vmsearchRequest { get; set; }
        public List<vFlightAcftPilot> flightList { get; set; }

        public vmSearchRequestFights()
        {
        }

        public vmSearchRequestFights(vmSearchRequest p_sr, int p_rowsPerPage = 50)
        {

            vmsearchRequest = p_sr;
            rowsPerPage = p_rowsPerPage;
            topN = 50;

            var q = new qLINQ();
            var f = q.flightsAll;

            if (vmsearchRequest.isSearchJunk)
            {
                f = f.Where(r => r.IsJunk == true);
                foreach (vFlightAcftPilot i in f) { i.IsChecked = true; }
                topN = rowsPerPage;
            }
            if (!string.IsNullOrEmpty(vmsearchRequest.flightID))
            {
                int flightIDint = int.Parse(vmsearchRequest.flightID);
                f = f.Where(row => row.FlightID == flightIDint);
            }
            if (!string.IsNullOrEmpty(vmsearchRequest.flightDate))
            {
                var flightDatedate = DateTime.Parse(vmsearchRequest.flightDate);
                f = f.Where(row => DbFunctions.TruncateTime(row.FlightDate) == flightDatedate);
            }
            //if (!string.IsNullOrEmpty(airportID))
            //{
            //    //flights = flights.Where(row => row.AcftID == int.Parse(aircraftID)).ToList();
            //}
            if (!string.IsNullOrEmpty(vmsearchRequest.acftNumLocal))
            {
                q.acftNumLocal = vmsearchRequest.acftNumLocal;
                var acftids = q.aircraftsByAcftNumLocal.Select(row => row.AcftID).ToList();
                f = f.Where(row => acftids.Contains(row.AcftID.Value));
            }
            if (!string.IsNullOrEmpty(vmsearchRequest.companyID))
            {
                q.companyIDint = int.Parse(vmsearchRequest.companyID);
                var acftids = q.aircraftsByCompany.Select(row => row.AcftID).ToList();
                f = f.Where(row => acftids.Contains(row.AcftID.Value));
            }
            if (!string.IsNullOrEmpty(vmsearchRequest.pilotID))
            {
                var pilotIDint = int.Parse(vmsearchRequest.pilotID);
                f = f.Where(row => row.PilotID == pilotIDint);
            }
            if (!string.IsNullOrEmpty(vmsearchRequest.pilotID))
            {
                var pilotIDint = int.Parse(vmsearchRequest.pilotID);
                f = f.Where(row => row.PilotID == pilotIDint);
            }
            totalRecordCount = f.Count();
            flightList = f.Take(topN).ToList();
        }
    }
    public class vmSearchRequest
    {
        public string flightID { get; set; }
        public string airportID { get; set; }
        public string acftNumLocal { get; set; }
        public string pilotID { get; set; }
        public string flightDate { get; set; }
        public string companyID { get; set; }
        public bool isSearchJunk { get; set; }
        public vmSearchRequest()
        {
        }

        public vmSearchRequest(string p_flightID = null, string p_airportID = null, string p_acftNumLocal = null, string p_pilotID = null, string p_flightDate = null, string p_companyID = null,bool p_isSearchJunk = false)
        {
            flightID = p_flightID;
            airportID = p_airportID;
            acftNumLocal = p_acftNumLocal;
            pilotID = p_pilotID;
            flightDate = p_flightDate;
            companyID = p_companyID;
            isSearchJunk = p_isSearchJunk;

    }
}
}