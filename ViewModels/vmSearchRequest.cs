using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_Acft_Track.Helpers;
using System.Data.Entity;
using MVC_Acft_Track.Models;
using System.Web.Helpers;
using static MVC_Acft_Track.Finals;

namespace MVC_Acft_Track.ViewModels
{
    public class vmSearchRequestFights
    {
        public bool isRouteListRequest { get; set; } = false;
        public int topN { get; set; } = DEFAULT_TOPN;
        public int rowsPerPage { get; set; } = DEFAULT_ROWPERPAGE;
        public int totalRecordCount { get; set; } = 0;
        public string sortCol { get; set; } = "RouteID";
        public SortDirection sortDir { get; set; } = SortDirection.Descending;
        public vmSearchRequest vmsearchRequest { get; set; }
        public List<vFlightAcftPilot> flightList { get; set; }
        public List<vRoute> routeList { get; set; }

        static qLINQ q = new qLINQ();
        IQueryable<vFlightAcftPilot> f = q.flightsAll;
        IQueryable<vRoute> rt = q.routesAll;

        public vmSearchRequestFights() { }
        public vmSearchRequestFights Search()
        {
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
            if (!(vmsearchRequest.routeID == null))
            {
                f = f.Where(row => row.RouteID == vmsearchRequest.routeID);
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
            //if (!string.IsNullOrEmpty(vmsearchRequest.pilotID))
            //{
            //    var pilotIDint = int.Parse(vmsearchRequest.pilotID);
            //    f = f.Where(row => row.PilotID == pilotIDint);
            //}
            if (!(vmsearchRequest.pilotID == null))
            {
                f = f.Where(row => row.PilotID == vmsearchRequest.pilotID);
            }
            if (!(vmsearchRequest.aicrftID == null))
            {
                f = f.Where(row => row.AcftID == vmsearchRequest.aicrftID);
            }
            if (vmsearchRequest.isSearchJunk)
            {
                f = f.Where(row => row.IsJunk == true);
            }
            if (vmsearchRequest.isNoJunk)
            {
                f = f.Where(row => row.IsJunk == false);
            }
            if (isRouteListRequest)
            {
                var routeIds = f.Select(r => r.RouteID).ToArray();
                routeList = rt.Where(row => routeIds.Contains(row.RouteID)).ToList();
                totalRecordCount = routeList.Count();
                routeList = routeList.Take(topN).ToList();
            }
            else
            {
                flightList = f.ToList();
                totalRecordCount = flightList.Count();
                flightList = flightList.Take(topN).ToList();
            }

            return this;
        }

        public vmSearchRequestFights(vmSearchRequest p_sr, int p_rowsPerPage=50, int p_topN = 50, string p_sortCol= "RouteID", SortDirection p_sortdir = SortDirection.Ascending)
        {
            vmsearchRequest = p_sr;
            rowsPerPage = p_rowsPerPage;
            sortCol = p_sortCol;
            sortDir = p_sortdir;
            topN = p_topN;

            q = new qLINQ();
            f = q.flightsAll;
            rt = q.routesAll;

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
            if (!(vmsearchRequest.routeID==null))
            {
                f = f.Where(row => row.RouteID == vmsearchRequest.routeID);
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
            //if (!string.IsNullOrEmpty(vmsearchRequest.pilotID))
            //{
            //    var pilotIDint = int.Parse(vmsearchRequest.pilotID);
            //    f = f.Where(row => row.PilotID == pilotIDint);
            //}
            if (!(vmsearchRequest.pilotID==null))
            {
                f = f.Where(row => row.PilotID == vmsearchRequest.pilotID);
            }
            if (!(vmsearchRequest.aicrftID == null))
            {
                f = f.Where(row => row.AcftID == vmsearchRequest.aicrftID);
            }
            if (vmsearchRequest.isSearchJunk)
            {
                f = f.Where(row => row.IsJunk == true);
            }
            if (vmsearchRequest.isNoJunk)
            {
                f = f.Where(row => row.IsJunk == false);
            }
            if (isRouteListRequest)
            {
                var routeIds = f.Select(r => r.RouteID);
                routeList = rt.Where(row => routeIds.Contains(row.RouteID)).ToList();
                totalRecordCount = routeList.Count();
            }
            flightList = f.ToList();
            totalRecordCount = flightList.Count();
            flightList = flightList.Take(topN).ToList();
        }
    }
    public class vmSearchRequest
    {
        public string flightID { get; set; }
        public int? routeID { get; set; }
        public string airportID { get; set; }
        public int? aicrftID { get; set; }
        public string acftNumLocal { get; set; }
        public int? pilotID { get; set; }
        public string flightDate { get; set; }
        public string companyID { get; set; }
        public bool isSearchJunk { get; set; }
        public bool isNoJunk { get; set; }
        public vmSearchRequest()
        {
        }

}
}