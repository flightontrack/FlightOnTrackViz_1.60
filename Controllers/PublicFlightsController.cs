using MVC_Acft_Track.Helpers;
using MVC_Acft_Track.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static MVC_Acft_Track.Finals;

namespace MVC_Acft_Track.Controllers
{
    public class PublicFlightsController : Controller
    {

        private Entities db = new Entities();
        private IEnumerable<vFlightAcftPilot> q_flightsRecent;
        private IEnumerable<vFlightAcftPilot> q_flightsActive;
        private IEnumerable<GpsLocation> q_gpslocationsActive;
        private IEnumerable<DimArea> q_getAreaCenter;
        private IEnumerable<vPilotLogBook> q_flightsLogBook;
        private IEnumerable<vFlightAcftPilot> q_flightsByRoute;
        //private IEnumerable<vFlightAcftPilot> q_isPositionCurrent;
        private int flightId;
        private int routeid;
        private int areaId;
        private int top;

        public PublicFlightsController()
        {
            q_flightsRecent = db.vFlightAcftPilots.Where(row => row.IsShared == null ? false : (bool)row.IsShared).Where(row => row.IsJunk == false).OrderByDescending(row => row.FlightID).Take(TIMESPANFLIGHTS);
            q_flightsLogBook = db.vPilotLogBooks.OrderByDescending(row => row.RouteID).Take(TIMESPANFLIGHTS);

            q_flightsActive = db.vFlightAcftPilots.Where(r => r.isInFlight == TRUE).Where(r => r.Points > 0);
            q_gpslocationsActive = db.GpsLocations.Where(row => row.FlightID == flightId).OrderBy(g => g.FlightID).ThenByDescending(g => g.onSessionPointNum);
            //q_gpslocationsActive = db.GpsLocations.Where(row => row.FlightID.Value == flightId).OrderBy(g => g.FlightID).ThenBy(g => g.onSessionPointNum);
            q_getAreaCenter = db.DimAreas.Where(r => r.AreaID.Equals(areaId));
            q_flightsByRoute = db.vFlightAcftPilots.Where(row => (row.RouteID == routeid&&row.IsJunk==false));
        }

        //public ActionResult Index()
        //{
        //    return View("Index");
        //}

        [HttpGet]
        public ActionResult GetLatestRoutes()
        {
            try
            {
                var theDate = DateTime.Today.Add(new System.TimeSpan(TIMESPANDAYS, 0, 0, 0));
                //var q = db.vFlightAcftPilots.ToList();//.Where(row => row.IsShared == null ? false : (bool)row.IsShared).ToList();//.Where(row => row.IsJunk == false).OrderByDescending(row => row.FlightID);//.Take(TIMESPANFLIGHTS);
                var flights = q_flightsLogBook.ToList();
                ViewBag.ViewTitle = "Recent Public Flights";
                //ViewBag.ActionBack = "IndexFlightsPublic";
                //ViewBag.ActionBack = "GetLatestRoutes";
                return View("GetLatestRoutes", flights);
            }
            catch (Exception e)
            {
                ViewBag.eMessage = "GetLatestRoutes() " + e.Message;
                return View("Error");
            };
        }      
        [HttpPost]
        public ActionResult GetLatestRoutes(FormCollection form)
        {
            var routeIds = new List<int>();
            var c = form.Count;
            foreach (string id in form)
            {
                if (form[id] == "submit") continue;
                if (form.GetValues(id).Contains("true"))
                {
                    routeIds.Add(int.Parse(id));
                }
            }
            var flightIds = db.Flights.Where(f=>routeIds.Contains(f.RouteID.Value)).Select(f=>f.FlightID).ToList();
            if (routeIds.Count == 0) return RedirectToAction("GetLatestRoutes");
            //var gpslocations = db.GpsLocations.Where(row => flightIds.Contains(row.FlightID)).OrderBy(g => g.FlightID).ThenByDescending(g => g.onSessionPointNum).Select(g => new { g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
            var gpslocations = db.GpsLocations.Where(row => flightIds.Contains(row.FlightID)).OrderBy(g => g.FlightID).ThenBy(g => g.onSessionPointNum).Select(g => new { g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.FlightsJsonData = serializer.Serialize(gpslocations);
            ViewBag.AreaCenterLat = gpslocations.FirstOrDefault().latitude;
            ViewBag.AreaCenterLong = gpslocations.FirstOrDefault().longitude;
            //ViewBag.ActionBack = "GetLatestRoutes";
            //return View("DisplayLatestFlightsStaticMap");
            var flightIdsString = string.Join(",", flightIds);
            return RedirectToAction("GetRouteFlights", "Flight", new { flightIds = flightIdsString });

        }
        
        public ActionResult CheckRouteFlightsCount(int id = 0, string actionBack = "")
        {
            bool i = db.Flights.Where(row => (row.RouteID == id)).Count() > 1;
            return RedirectToAction(i ? "GetRouteFlights" : "DisplayFlightData", i ? "PublicFlights" : "Flight", new { id, actionBack });
        }
        
        [HttpGet]
        public ActionResult GetRouteFlights(int id, string actionBack, string message="")
        {
            // route is shared if base flight is shared
            //var isRouteShared = db.Flights.Where(row => (row.FlightID == id)).First().IsShared;
                try
                {
                    ViewBag.ViewTitle = "Route " + id + " Flights";
                    ViewBag.ActionBack = "GetLatestRoutes";
                    ViewBag.Message = message;
                    //ViewBag.Caller = "GetRouteFlights";
                    routeid = id;
                    var q = q_flightsByRoute.ToList();
                    return View("IndexFlightsPublic", q);
                }
                catch (Exception e)
                {
                    ViewBag.ExceptionErrorMessage = e.Message ;
                    return View("ErrorPage");
                }
            }
        [HttpPost]
        public ActionResult GetRouteFlights(FormCollection form)
        {
            var flightIds = new List<string>();
            var c = form.Count;
            foreach (string id in form)
            {
                if (form[id] == "submit") continue;
                if (form.GetValues(id).Contains("true"))
                {
                    //flightIds.Add(int.Parse(id));
                    flightIds.Add(id);
                }
            }
            if (flightIds.Count == 0) return RedirectToAction("GetRouteFlights",new { message = SELECTSOMTHING });
            var flightIdsString=string.Join(",", flightIds);
            return RedirectToAction("GetRouteFlights", "Flight", new { flightIds = flightIdsString });
            //return View("DisplayLatestFlightsStaticMap");
        }
        
        [HttpGet]
        public ActionResult SearchByCriteria(string message = "")
        {
            ViewBag.AircraftsSelList = new SelectList(db.vListAircrafts.OrderBy(row => row.AcftNumLocal), "AcftID", "AcftNumLocal");
            ViewBag.PilotSelList = new SelectList(db.vListPilots.OrderBy(row => row.PilotCode), "PilotID", "PilotCode");
            ViewBag.AirportSelList = new SelectList(db.vListAirports.OrderBy(row => row.AirportCode), "AirportID", "AirportCode");
            ViewBag.CompanySelList = new SelectList(db.DimCompany.OrderBy(row => row.Name), "CompanyID", "Name");
            ViewBag.Message = message;
            return View("SearchByCriteria");
        }
        [HttpPost]
        public ActionResult SearchByCriteria(FormCollection form)
        {
            if (form["submit"] == "Search")
            {
                if (form["FlightID"].Equals("") && form["AcftNumLocal"].Equals("") && form["PilotID"].Equals("") && form["FlightDate"].Equals("") && form["CompanyID"].Equals("")) return RedirectToAction("SearchByCriteria", new { message = SELECTSOMTHING });
                var flightID = form["FlightID"];
                var airportID = form["AirportID"];
                var acftNumLocal = form["AcftNumLocal"];
                var pilotID = form["PilotID"];
                var flightDate = form["FlightDate"];
                var companyID = form["CompanyID"];
                return RedirectToAction("SearchByCriteriaResult", new { flightID = flightID, airportID = airportID, acftNumLocal = acftNumLocal, pilotID = pilotID, flightDate = flightDate,companyID= companyID });
            }
            return View();
        }

        [HttpGet]
        public ActionResult SearchByCriteriaResult(string flightID, string airportID, string acftNumLocal, string pilotID, string flightDate, string companyID)
        {
            List<vFlightAcftPilot> flights = new List<vFlightAcftPilot>();
            var f = db.vFlightAcftPilots.Where(row => 1==1);

            if (string.IsNullOrEmpty(flightID + acftNumLocal + pilotID + flightDate+ companyID))
            {
                return RedirectToAction("SearchByCriteria");
            }
            else
            {
                if (!string.IsNullOrEmpty(flightID))
                {
                    int flightIDint = int.Parse(flightID);
                    f = f.Where(row => row.FlightID == flightIDint);
                }
                if (!string.IsNullOrEmpty(flightDate))
                {
                    var flightDatedate = DateTime.Parse(flightDate);
                    f = f.Where(row => DbFunctions.TruncateTime(row.FlightDate)== flightDatedate);
                }
                //if (!string.IsNullOrEmpty(airportID))
                //{
                //    //flights = flights.Where(row => row.AcftID == int.Parse(aircraftID)).ToList();
                //}
                if (!string.IsNullOrEmpty(acftNumLocal))
                {
                    var acftids = db.AircraftPilots.Where(row => row.AcftNumLocal == acftNumLocal).Select(row => row.AcftID).ToList();
                    f = f.Where(row => acftids.Contains(row.AcftID.Value));
                }
                if (!string.IsNullOrEmpty(companyID))
                {
                    var companyIDint = int.Parse(companyID);
                    var acftids = db.AircraftPilots.Where(row => row.CompanyID == companyIDint).Select(row => row.AcftID).ToList();
                    f = f.Where(row => acftids.Contains(row.AcftID.Value));
                }
                if (!string.IsNullOrEmpty(pilotID))
                {
                    var pilotIDint = int.Parse(pilotID);
                    f = f.Where(row => row.PilotID == pilotIDint);
                }
                flights = f.Where(row=>!row.IsJunk).Where(row => row.IsShared == null ? false : (bool)row.IsShared).ToList();
            }

            return View("IndexFlightsPublic", flights);
        }
        [HttpPost]
        public ActionResult SearchByCriteriaResult(FormCollection form)
        {
            var flightIds = new List<int>();
            var c = form.Count;
            foreach (string id in form)
            {
                if (form[id] == "Compare") continue;
                if (form.GetValues(id).Contains("true"))
                {
                    flightIds.Add(int.Parse(id));
                }
            }
            if (flightIds.Count == 0) return View();
            var gpslocations = db.GpsLocations.Where(row => flightIds.Contains(row.FlightID)).OrderBy(g => g.FlightID).ThenBy(g => g.onSessionPointNum).Select(g => new { g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.FlightsJsonData = serializer.Serialize(gpslocations);
            ViewBag.AreaCenterLat = gpslocations.FirstOrDefault().latitude;
            ViewBag.AreaCenterLong = gpslocations.FirstOrDefault().longitude;
            return View("DisplayLatestFlightsStaticMap");
        }

        [HttpGet]
        public ActionResult SearchByArea(int? id = null, string message = "")
        {
            ViewBag.AreaSelList = new SelectList(db.DimAreas.OrderBy(row => row.AreaName), "AreaID", "AreaName", id);
            ViewBag.Message = message;
            return View();
        }
        [HttpPost]
        public ActionResult SearchByArea(FormCollection form)
        {
            if (form["AreaID"].Equals("")) return RedirectToAction("SearchByArea", new { message = SELECTSOMTHING });
            var areaId = int.Parse(form["AreaID"]);
            if (form["submit_map"] == "Display Map")
            {
                return RedirectToAction("DisplayAreaMovingMap", new { aId = areaId });
            }
            if (form["submit_list"] == "Display List")
            {
                return RedirectToAction("GetAreaActiveFlights", new { areaId = areaId });
            }
            return RedirectToAction("SearchByArea");
        }

        [HttpGet]
        public ActionResult DisplayAreaMovingMap(int aId)
        {
            areaId = aId;
            var areaCenterLat = q_getAreaCenter.Select(r => new { r.CenterLat, r.CenterLong }).ToList();
            //var areaCenterLat = q_getAreaCenter.Select(r => new { r.CenterLat, r.CenterLong });
            ViewBag.areaId = areaId;
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //ViewBag.AreaCenter = this.Json(areaCenter, JsonRequestBehavior.AllowGet);
            ViewBag.AreaCenterLat = areaCenterLat.FirstOrDefault().CenterLat;
            ViewBag.AreaCenterLong = areaCenterLat.FirstOrDefault().CenterLong;
            ViewBag.RadiusSelList = ListsDD.getRadius();
            return View();
        }

        [HttpGet]
        public ActionResult GetAreaActiveFlights(int areaId, string message = "")
        {
            ViewBag.Message = message;
            top = TRACKPOINTS;
            try
            {
                var gpslocationsActive = new List<GpsLocation>();
                var flightsActive = q_flightsActive.ToList().Select(r => r.FlightID);
                foreach (int flight in flightsActive)
                {
                    flightId = flight;
                    gpslocationsActive.InsertRange(gpslocationsActive.Count(), q_gpslocationsActive.Take(top).ToList());
                    //gpslocationsActive.InsertRange(gpslocationsActive.Count(), q_gpslocationsActive.Take(top).OrderByDescending(g => g.onSessionPointNum).ToList());
                }
                var flightsInArea = (from l in gpslocationsActive
                                     where db.get_isArea(l.GPSLocationID, areaId).FirstOrDefault().GetValueOrDefault()
                                     select l.FlightID).ToList();
                var flightsActiveInArea = q_flightsActive.Where(r => flightsInArea.Contains(r.FlightID));
                ViewBag.areaId = areaId;
                ViewBag.areaName = db.DimAreas.Where(r => r.AreaID.Equals(areaId)).FirstOrDefault().AreaName;
                return View((IEnumerable<vFlightAcftPilot>)flightsActiveInArea);
            }
            catch (Exception e)
            {
                ViewBag.eMessage = "GetAreaActiveFlights()" + e.Message;
                return View("Error");
            };
        }
        [HttpPost]
        public ActionResult GetAreaActiveFlights(FormCollection form)
        {
            areaId = int.Parse(form.GetValues("areaId")[0]);
            if (form["submit"] == "Update Page") return RedirectToAction("GetAreaActiveFlights", new { areaId = areaId });
            var flightIds = new List<int>();
            var c = form.Count;
            foreach (string id in form)
            {
                if (form.GetValues(id).Contains("true"))
                {
                    flightIds.Add(int.Parse(id));
                }
            }
            if (flightIds.Count == 0) return RedirectToAction("GetAreaActiveFlights", new { areaId = areaId, message = SELECTSOMTHING });

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var flightsSer = serializer.Serialize(flightIds);
            return RedirectToAction("DisplayAreaSelFlightsMovingMap", new { aId = areaId, fs = flightsSer });
        }

        public ActionResult DisplayAreaSelFlightsMovingMap(int aId, string fs, int isAutoUpdate = 0)
        {
            areaId = aId;
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //int[] flights = serializer.Deserialize<int[]>(fs);
            var areaCenterLat = q_getAreaCenter.Select(r => new { r.CenterLat, r.CenterLong }).ToList();
            //ViewBag.Flights = flights;
            ViewBag.Flights = fs;
            ViewBag.AreaCenterLat = areaCenterLat.FirstOrDefault().CenterLat;
            ViewBag.AreaCenterLong = areaCenterLat.FirstOrDefault().CenterLong;
            ViewBag.backUrl = "GetAreaActiveFlights";
            ViewBag.areaId = areaId;
            ViewBag.isAutoUpdate = isAutoUpdate;
            return View();
        }

        public JsonResult GetAreaFlightsJson(int areaId, int c)
        {
            //top = c;
            top = TRACKPOINTS;
            var gpslocationsActive = new List<GpsLocation>();
            var flightsActive = q_flightsActive.ToList().Select(r => new { r.FlightID, r.isPositionCurrent, r.Acft });
            foreach (var flight in flightsActive)
            {
                flightId = flight.FlightID;
                gpslocationsActive.InsertRange(gpslocationsActive.Count(), q_gpslocationsActive.Take(top).ToList());
                //gpslocationsActive.InsertRange(gpslocationsActive.Count(), q_gpslocationsActive.Take(top).OrderByDescending(g => g.onSessionPointNum).ToList());
            }
            var activeLocInArea = (from l in gpslocationsActive
                                   where db.get_isArea(l.GPSLocationID, areaId).FirstOrDefault().GetValueOrDefault()
                                   select l).ToList();
            var gpslocations = from l in activeLocInArea
                               join f in flightsActive on l.FlightID equals f.FlightID
                               select new { l.GPSLocationID, l.FlightID, l.SpeedKnot, l.SpeedKmpH, l.gpsTimeOnly, l.AltitudeFt, l.AltitudeM, l.latitude, l.longitude, f.isPositionCurrent, f.Acft };

            //var a = this.Json(gpslocations, JsonRequestBehavior.AllowGet);
            return this.Json(gpslocations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultipleFlightsDataJson(string fs, int c = 0)
        {
            top = TRACKPOINTS;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            int[] flightIds = serializer.Deserialize<int[]>(fs);
            var flightsActive = q_flightsActive.Where(row => flightIds.Contains(row.FlightID)).ToList().Select(r => new { r.FlightID, r.isPositionCurrent, r.Acft });
            var gpslocationsActive = new List<GpsLocation>();
            foreach (var flight in flightsActive)
            {
                flightId = flight.FlightID;
                //gpslocationsActive.InsertRange(gpslocationsActive.Count(), q_gpslocationsActive.Take(top).ToList());
                gpslocationsActive.InsertRange(gpslocationsActive.Count(), q_gpslocationsActive.Take(top).OrderByDescending(g => g.onSessionPointNum).ToList());
            }
            var gpslocations = from l in gpslocationsActive
                               join f in flightsActive on l.FlightID equals f.FlightID
                               select new { l.GPSLocationID, l.FlightID, l.SpeedKnot, l.SpeedKmpH, l.gpsTimeOnly, l.AltitudeFt, l.AltitudeM, l.latitude, l.longitude, f.isPositionCurrent, f.Acft };

            return this.Json(gpslocations, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SearchByGroup(int? id = null, string message = "")
        {
            //ViewBag.AreaSelList = new SelectList(db.DimAreas.OrderBy(row => row.AreaName), "AreaID", "AreaName", id);
            //ViewBag.Message = message;
            return View();
        }
    }

}
