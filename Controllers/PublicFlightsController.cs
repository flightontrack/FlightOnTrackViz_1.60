using FontNameSpace.Helpers;
using FontNameSpace.Models;
using FontNameSpace.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static FontNameSpace.App;
using static FontNameSpace.Finals;

namespace FontNameSpace.Controllers
{
    public class PublicFlightsController : Controller
    {

        private Entities db = new Entities();
        private Queryables q = new Queryables();
        //private IEnumerable<vFlightAcftPilot> q_flightsRecent;
        //private IEnumerable<vFlightAcftPilot> q_flightsActive;
        //private IEnumerable<vFlightAcftPilot> q_acftGroupFlightsActive;
        private IEnumerable<GpsLocation> q_gpslocationsActive;
        //private IEnumerable<DimArea> q_getAreaCenter;
        //private IQueryable<vPilotLogBook> q_flightsLogBook;
        //private IQueryable<vFlightAcftPilot> q_flightsByRoute;
        //private IEnumerable<vFlightAcftPilot> q_isPositionCurrent;
        private int flightId;
        //private int routeid;
        //private int areaID;
        private int top;

        #region Constructor
        public PublicFlightsController()
        {
            //q_flightsRecent = db.vFlightAcftPilots.Where(row => row.IsShared == null ? false : (bool)row.IsShared).Where(row => row.IsJunk == false).OrderByDescending(row => row.FlightID).Take(TIMESPANFLIGHTS);
            //q_flightsLogBook = q.routesAll.OrderByDescending(row => row.RouteID).Take(TIMESPANFLIGHTS);
            //q_flightsLogBook = q.routesAll.Take(TIMESPANFLIGHTS);

            //q_flightsActive = db.vFlightAcftPilots.Where(r => r.isInFlight == TRUE).Where(r => r.Points > 0);

            q_gpslocationsActive = db.GpsLocations.Where(row => row.FlightID == flightId).OrderBy(g => g.FlightID).ThenByDescending(g => g.onSessionPointNum);
            //q_gpslocationsActive = db.GpsLocations.Where(row => row.FlightID.Value == flightId).OrderBy(g => g.FlightID).ThenBy(g => g.onSessionPointNum);
            //q_getAreaCenter = db.DimAreas.Where(r => r.AreaID.Equals(areaID));
            //q_flightsByRoute = q.flightsAll.Where(row => (row.RouteID == routeid&&row.IsJunk==false));
        }
        #endregion
        #region Get Flights Routes
        [HttpGet]

        public ActionResult CheckRouteFlightsCount(int id = 0, string actionBack = "")
        {
            var recordSet = new Queryables(id, "Route").flightsByRoute.ToList();
            var c = recordSet.Count();
            if (c == 1) { id = recordSet[0].FlightID; };
            //bool i = db.Flights.Where(row => (row.RouteID == id)).Count() > 1;
            return RedirectToAction(c>1 ? "GetRouteFlights" : "DisplayFlightData", c>1 ? "PublicFlights" : "Flight", new { id});
        }
        
        [HttpGet]
        public ActionResult GetRouteFlights(int id, string actionBack, string message="")
        {
            // route is shared if base flight is shared
            //var isRouteShared = db.Flights.Where(row => (row.FlightID == id)).First().IsShared;
                try
                {
                    ViewBag.ViewTitle = "Route " + id + " Flights";
                    //ViewBag.ActionBack = "GetLatestRoutes";
                    ViewBag.Message = message;
                    ViewBag.DisplayChBoxChecked = true;
                    //var sr = new vmSearchRequest{ routeID= id,isNoJunk = true};
                    var fs = new vmSearchRequestFights { vmsearchRequest=new vmSearchRequest { routeID = id, isNoJunk = true }}.Search();
                    //return View(fs);
                //routeid = id;
                    //var q = q_flightsByRoute.ToList();
                    return View(fs);
                }
                catch (Exception e)
                {
                    ViewBag.ExceptionErrorMessage = e.Message ;
                    return View("ExceptionPage");
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
        //[HttpGet]
        //public ActionResult FlightsPublicByCriteria(vmSearchRequest searchRequest, string sort = "", string sortdir = "")
        //{
        //    try
        //    {
        //        var fs = new vmSearchRequestFights(searchRequest, 50, 10000, sort, sortdir.Equals("ASC") ? SortDirection.Ascending : SortDirection.Descending);
        //        return View(fs);
        //    }
        //    catch (Exception e)
        //    {
        //        ViewBag.ExceptionErrorMessage = isDebugMode ? e.Message : "FlightsPublicByCriteria() error";
        //        return View("ExceptionPage");
        //    }
        //}

        //[HttpPost]
        //public ActionResult FlightsPublicByCriteria(FormCollection form)
        //{
        //    if (form["buttonClicked"] == "DisplayOnMap")
        //    {
        //        var flightIds = new List<int>();
        //        var c = form.Count;
        //        foreach (string id in form)
        //        {
        //            if (form[id] == "Compare") continue;
        //            if (form.GetValues(id).Contains("true"))
        //            {
        //                flightIds.Add(int.Parse(id));
        //            }
        //        }
        //        if (flightIds.Count == 0) return View();
        //        var gpslocations = db.GpsLocations.Where(row => flightIds.Contains(row.FlightID)).OrderBy(g => g.FlightID).ThenBy(g => g.onSessionPointNum).Select(g => new { g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
        //        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //        ViewBag.FlightsJsonData = serializer.Serialize(gpslocations);
        //        ViewBag.AreaCenterLat = gpslocations.FirstOrDefault().latitude;
        //        ViewBag.AreaCenterLong = gpslocations.FirstOrDefault().longitude;
        //        return View("DisplayLatestFlightsStaticMap");
        //    }
        //    return View();
        //}
        [HttpGet]
        //public ActionResult RoutesPublicByCriteria(vmSearchRequest searchRequest, string sort = "", string sortdir = "")
        public ActionResult GetRoutes(vmSearchRequest searchRequest, string sort = "RouteID", string sortdir = "")
        {
            try
            {
                vmSearchRequestFights rts = new vmSearchRequestFights
                {
                    vmsearchRequest = searchRequest??new vmSearchRequest(),
                    isRouteListRequest = true,
                    topN = searchRequest.vmSearchRequestTopN,
                    sortDir = sortdir.Equals("ASC") ? SortDirection.Ascending : SortDirection.Descending,
                    sortCol = sort
                }
                .Search();
                ViewBag.ViewTitle = searchRequest.vmSearchRequestTitle;
                return View(rts);
            }
            catch (Exception e)
            {
                ViewBag.ExceptionErrorMessage = isDebugMode ? e.InnerException.Message : "GetRoutes() error";
                return View("ExceptionPage");
            }
        }

        [HttpPost]
        public ActionResult GetRoutes(FormCollection form)
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
            var flightIds = q.flightsOnlyAll.Where(f => routeIds.Contains(f.RouteID.Value)).Select(f => f.FlightID).ToArray();
            if (routeIds.Count == 0) return RedirectToAction("GetRoutes");
            var gpslocations = db.GpsLocations.Where(row => flightIds.Contains(row.FlightID)).OrderBy(g => g.FlightID).ThenBy(g => g.onSessionPointNum).Select(g => new { g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.FlightsJsonData = serializer.Serialize(gpslocations);
            ViewBag.AreaCenterLat = gpslocations.FirstOrDefault().latitude;
            ViewBag.AreaCenterLong = gpslocations.FirstOrDefault().longitude;
            var flightIdsString = string.Join(",", flightIds);
            return RedirectToAction("GetRouteFlights", "Flight", new { flightIds = flightIdsString });

        }
        public ActionResult GetLatestRoutes()
        {
            var theDate = DateTime.Today.Add(new System.TimeSpan(TIMESPANDAYS, 0, 0, 0));
            var vmsearchRequest = new vmSearchRequest { vmSearchRequestTopN = TIMESPANFLIGHTS, vmSearchRequestTitle = "Recent Public Flights" };
            return RedirectToAction("GetRoutes", vmsearchRequest);
        }
        #endregion
        #region Search methods
        [HttpGet]
        public ActionResult SearchByCriteria(string message = "")
        {
            ViewBag.AircraftsSelList = new SelectList(q.selList_vAircraftDistinctWithFlightsPublic.ToList(), "AcftID", "AcftNumLocal");
            //new SelectList(db.vListAircrafts.OrderBy(row => row.AcftNumLocal), "AcftID", "AcftNumLocal");
            ViewBag.PilotSelList = new SelectList(db.vListPilots.OrderBy(row => row.PilotCode), "PilotID", "PilotCode");
            ViewBag.AirportSelList = new SelectList(db.vListAirports.OrderBy(row => row.AirportCode), "AirportID", "AirportCode");
            ViewBag.GroupSelList = new SelectList(db.EntityGroups.OrderBy(row => row.GroupName), "GroupID", "GroupName");
            ViewBag.PublicSearch = true;
            ViewBag.Message = message;
            return View("SearchByCriteria");
        }

        [HttpPost]
        public ActionResult SearchByCriteria(vmSearchRequest vmsearchRequest, FormCollection form)
        {
            if (form["submit"] == "Search")
            {
                if (form["FlightID"].Equals("") && form["AcftNumLocal"].Equals("") && form["PilotID"].Equals("") && form["FlightDate"].Equals("") && form["GroupID"].Equals("")) return RedirectToAction("SearchByCriteria", new { message = SELECTSOMTHING });
                vmsearchRequest.vmSearchRequestTopN = DEFAULT_TOPN;
                vmsearchRequest.vmSearchRequestTitle = "Public Flights Search Result";
                return RedirectToAction("GetRoutes", vmsearchRequest);
                //return RedirectToAction("GetRoutes", new { searchRequest = new vmSearchRequest(), topN = DEFAULT_TOPN, title = "Public Flights Search Result" } );
            }
            return View();
        }

        #endregion
        #region Area Flights
        [HttpGet]
        public ActionResult SearchByArea(int? areaId = null, string message = "")
        {
            ViewBag.AreaSelList = new SelectList(db.DimAreas.OrderBy(row => row.AreaName), "AreaID", "AreaName", areaId);
            ViewBag.Message = message;
            return View();
        }
        [HttpPost]
        public ActionResult SearchByArea(FormCollection form)
        {
            if (form["AreaID"].Equals("")) return RedirectToAction("SearchByArea", new { message = SELECTSOMTHING });
            var areaID = int.Parse(form["AreaID"]);

            if (form["submit_map"] == "Display Map")
            {
                var area = new ClassArea(areaID, DEFAULT_AREARADIUS);
                return RedirectToAction("DisplayAreaMovingMap", new { aId = areaID, areaCenterLat = area.circle.Lat, areaCenterLong = area.circle.Long });
            }
            if (form["submit_list"] == "Display Flights")
            {
                return RedirectToAction("GetListAreaActiveFlights", new { areaId = areaID });
            }
            return RedirectToAction("SearchByArea");
        }
        [HttpGet]
        public ActionResult GetListAreaActiveFlights(int areaId, string areaRadius= DEFAULT_AREARADIUS, string message = "")
        {
            ViewBag.Message = message;
            top = TRACKPOINTS;
            var top1 = 1;
            try
            {
                var classActiveFlights = new ClassActiveFlights();
                var gpslocationsActive = new List<GpsLocation>();
                /// get all currently active flights
                var flightsActive = classActiveFlights.inflightFlights.Select(r=>r.FlightID).ToList();
                foreach (int flight in flightsActive)
                {
                    flightId = flight;
                    gpslocationsActive.InsertRange(gpslocationsActive.Count(), q_gpslocationsActive.Take(top1).ToList());
                }
                /// filter active flights in area out of all active flights
                classActiveFlights.flightIdList = (from l in gpslocationsActive
                                     where db.get_isArea(l.GPSLocationID, areaId, areaRadius).FirstOrDefault().GetValueOrDefault()
                                     select l.FlightID).ToList();
                var flightsActiveInArea = classActiveFlights.flightsFlights.ToList();
                ViewBag.areaId = areaId;
                ViewBag.DisplayChBoxChecked = true;
                var a = new ClassArea(areaId, DEFAULT_AREARADIUS);
                ViewBag.areaName = a.area.FirstOrDefault().AreaName;
                return View("GetListActiveFlights",flightsActiveInArea);
            }
            catch (Exception e)
            {
                ViewBag.eMessage = "GetListAreaActiveFlights()" + e.Message;
                return View("Error");
            };
        }

        [HttpPost]
        public ActionResult GetListAreaActiveFlights(FormCollection form)
        {
            var areaID = int.Parse(form["AreaID"]);
            if (form["submit"] == "Update Page") return RedirectToAction("GetListAreaActiveFlights", new { areaId = areaID });
            var flightIds = new List<int>();
            var c = form.Count;
            foreach (string id in form)
            {
                if (form.GetValues(id).Contains("true"))
                {
                    flightIds.Add(int.Parse(id));
                }
            }
            if (flightIds.Count == 0) return RedirectToAction("GetListAreaActiveFlights", new { areaId = areaID, message = SELECTSOMTHING });

            var flightsSer = new JavaScriptSerializer().Serialize(flightIds);
            var area = new ClassArea(areaID, DEFAULT_AREARADIUS);
            return RedirectToAction("DisplayAreaMovingMap", new { aId = areaID, fs = flightsSer, areaCenterLat = area.circle.Lat, areaCenterLong = area.circle.Long });
        }
        #endregion
        #region Group Flights
        [HttpGet]
        public ActionResult SearchByGroup(int? id = null, string message = "")
        {
            ViewBag.GroupSelList = new SelectList(db.EntityGroups.OrderBy(row => row.GroupName), "GroupID", "GroupName", id);
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public ActionResult SearchByGroup(FormCollection form)
        {
            if (form["GroupID"].Equals("")) return RedirectToAction("SearchByGroup", new { message = SELECTSOMTHING });
            var groupId = int.Parse(form["GroupID"]);
            //var classActiveFlights = new ClassActiveFlights();
            //classActiveFlights.groupId = groupId;
            //var flightIds = classActiveFlights.acftGroupFlightsActive.Select(r => r.FlightID).ToList();
            //if (flightIds.Count() == 0)
            //{
            //    return RedirectToAction("SearchByGroup", new { aId = groupId, message = MSG_NOGROUPACTIVACFTS });
            //}
            //var flightsSer = new JavaScriptSerializer().Serialize(flightIds);

            //if (form["submit_map"] == "Display Map")
            //{
            //    var groupCenter = classActiveFlights.getAcftGroupMapCircle();
            //    return RedirectToAction("DisplayAreaMovingMap", new { fs = flightsSer, areaCenterLat = groupCenter.Lat, areaCenterLong = groupCenter.Long });
            //}
            if (form["submit_list"] == "Display Flights")
            {
                //return RedirectToAction("GetListActiveFlights", new { flightsSer = flightsSer, groupId = groupId });
                return RedirectToAction("GetListActiveFlights", new {groupId = groupId });
            }
            return RedirectToAction("SearchByGroup");
        }
        [HttpGet]
        public ActionResult GetListActiveFlights(int groupId, string message = "")
        //public ActionResult GetListActiveFlights(string flightsSer, int groupId, string message = "")
        {
            var classActiveFlights = new ClassActiveFlights();
            classActiveFlights.groupId = groupId;
            var flightIds = classActiveFlights.acftGroupFlightsActive.Select(r => r.FlightID).ToList();
            if (flightIds.Count() == 0)
            {
                return RedirectToAction("SearchByGroup", new { id = groupId, message = MSG_NOGROUPACTIVACFTS });
            }

            ViewBag.Message = message;
            ViewBag.groupId = groupId;
            ViewBag.DisplayChBoxChecked = true;
            //var flightIds = new JavaScriptSerializer().Deserialize<int[]>(flightsSer);
            //var classActiveFlights = new ClassActiveFlights();
            classActiveFlights.flightIdList = flightIds;
            var flights=classActiveFlights.flightsFlights.ToList();
            return View(flights);
        }
        [HttpPost]
        public ActionResult GetListActiveFlights(FormCollection form)
        {
            if (form["groupId"].Equals("")) return RedirectToAction("SearchByGroup", new { message = SELECTSOMTHING });
            var groupId = int.Parse(form["groupId"]);
            var classActiveFlights = new ClassActiveFlights();
            classActiveFlights.groupId = groupId;
            var flightIds = classActiveFlights.acftGroupFlightsActive.Select(r => r.FlightID).ToList();
            //if (flightIds.Count() == 0)
            //{
            //    return RedirectToAction("SearchByGroup", new { aId = groupId, message = MSG_NOGROUPACTIVACFTS });
            //}
            var flightsSer = new JavaScriptSerializer().Serialize(flightIds);
            if (form["submit"] == "Update Page")
            {
                return RedirectToAction("GetListActiveFlights", new { flightsSer = flightsSer, groupId = groupId });
            }
            if (form["submit"] == "Display On Map")
            {
                flightIds.Clear();
                foreach (string id in form)
                {
                    if (form.GetValues(id).Contains("true"))
                    {
                        flightIds.Add(int.Parse(id));
                    }
                }
                if (flightIds.Count == 0) return RedirectToAction("GetListActiveFlights", new { flightsSer = flightsSer, groupId = groupId, message = SELECTSOMTHING });

                var flightsSelSer = new JavaScriptSerializer().Serialize(flightIds);
                classActiveFlights.flightIdList = flightIds;
                var mapCircle = classActiveFlights.getFlightsMapCircle();
                return RedirectToAction("DisplayAreaMovingMap", new { gId = groupId, fs = flightsSer, areaCenterLat = mapCircle.Lat, areaCenterLong = mapCircle.Long });
            }
            return View();
        }
        #endregion
        [HttpGet]
        public ActionResult DisplayAreaMovingMap(string fs,int? gId=null, int? aId=null, string aRadius = DEFAULT_AREARADIUS, decimal? areaCenterLat = null, decimal? areaCenterLong = null)
        {
            //areaId = aId;
            //var areaCenter = q_getAreaCenter.Select(r => new { r.CenterLat, r.CenterLong }).ToList()[0];
            ViewBag.areaId = aId;
            ViewBag.groupId = gId;
            ViewBag.Flights = fs;
            ViewBag.AreaCenterLat = areaCenterLat;
            ViewBag.AreaCenterLong = areaCenterLong;
            ViewBag.RadiusSelList = ListsDD.getRadius(aRadius);
            ViewBag.ARadius = Int32.Parse(aRadius);
            ViewBag.isFlightList = (aId==null);

            ViewBag.model = new
            {
                areaId = aId,
                groupId = gId,
                Flights = fs,
                AreaCenterLat = areaCenterLat,
                AreaCenterLong = areaCenterLong,
                RadiusSelList = ListsDD.getRadius(aRadius),
                ARadius = Int32.Parse(aRadius),
                isFlightList = (aId == null)
            };
            //ViewBag.backUrl = 
            return View();
        }
        #region JSON methods
        /// <summary>
        /// difference between the two JSON method:
        /// in area the number of aircraft/flights in the circle is dynamic whereas the area center is static
        /// in group of flights the number of aircraft/flights is static but the circle should move depending on position of the aircrafts
        /// </summary>
        public JsonResult GetAreaFlightsJson(int areaId, string areaRadius, int c)
        {
            //areaID = areaId;
            top = TRACKPOINTS;
            var gpslocationsActive = new List<GpsLocation>();
            var classActiveFlights = new ClassActiveFlights();
            /// get all currently active flights
            var flightsActive = classActiveFlights.inflightFlights.Select(r => new { r.FlightID, r.isPositionCurrent, r.AcftNumLocal }).ToList();
                //q_flightsActive.ToList().Select(r => new { r.FlightID, r.isPositionCurrent, r.AcftNumLocal });
            foreach (var flight in flightsActive)
            {
                flightId = flight.FlightID;
                gpslocationsActive.InsertRange(gpslocationsActive.Count(), q_gpslocationsActive.Take(top).ToList());
            }
            /// filter active gps locations in area out of all active flights
            var activeLocInArea = (from l in gpslocationsActive
                                   where db.get_isArea(l.GPSLocationID, areaId, areaRadius).FirstOrDefault().GetValueOrDefault()
                                   select l).ToList();
            /// join back to active flights to get acft and if position current
            var gpslocations = from l in activeLocInArea
                               join f in flightsActive on l.FlightID equals f.FlightID
                               select new { l.GPSLocationID, l.FlightID, l.SpeedKnot, l.SpeedKmpH, l.gpsTimeOnly, l.AltitudeFt, l.AltitudeM, l.latitude, l.longitude, f.isPositionCurrent, f.AcftNumLocal };
            var area = new ClassArea(areaId, areaRadius);
            var jsonObj = new { centerRadius = area.circle, gpslocations = gpslocations };
            return this.Json(jsonObj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMultipleFlightsDataJson(string fs, int c = 0)
        {
            top = TRACKPOINTS;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var classActiveFlights = new ClassActiveFlights();
            classActiveFlights.flightIdList = serializer.Deserialize<int[]>(fs).ToList();
            var flightsActive = classActiveFlights.flightsFlights.Select(r => new { r.FlightID, r.isPositionCurrent, r.AcftNumLocal, r.FlightDurationMin }).ToList();
            var gpslocationsActive = new List<GpsLocation>();
            foreach (var flight in flightsActive)
            {
                flightId = flight.FlightID;
                gpslocationsActive.InsertRange(gpslocationsActive.Count(), q_gpslocationsActive.Take(top).OrderByDescending(g => g.onSessionPointNum).ToList());
            }
            var gpslocations = from l in gpslocationsActive
                               join f in flightsActive on l.FlightID equals f.FlightID
                               select new { l.GPSLocationID, l.FlightID, l.SpeedKnot, l.SpeedKmpH, l.gpsTimeOnly, l.AltitudeFt, l.AltitudeM, l.latitude, l.longitude, f.isPositionCurrent, f.AcftNumLocal,f.FlightDurationMin,
                                   durHhMm = string.Format("{0}:{1}", f.FlightDurationMin / 60, f.FlightDurationMin % 60)
                               };
            var groupCenterRadius = classActiveFlights.getFlightsMapCircle();
            var jsonObj = new { centerRadius = groupCenterRadius, gpslocations = gpslocations };
            return this.Json(jsonObj, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }

}
