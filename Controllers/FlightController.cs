using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using kmlStyles;
using kmlFlightPlacemarkPoint;
using kmlFlightPlacemarkLineString;
using MVC_Acft_Track.Models;
using MVC_Acft_Track.ViewModels;
using MVC_Acft_Track.Helpers;
using Ionic.Zip;

namespace MVC_Acft_Track.Controllers
{ 

    public class FlightController : Controller
    {
        public bool debugmode = true;
        private qLINQ q = new qLINQ { db = new Entities()};

        private Entities db = new Entities();
        private int pilotid;
        //private int routeid;
        private IEnumerable<Pilot> q_pilot;
        private IEnumerable<vFlightAcftPilot> q_flightsByPilot;
        //private IEnumerable<vFlightAcftPilot> q_flightsByRoute;
        private IEnumerable<vFlightAcftPilot> q_flightsAll;
        private IEnumerable<vPilotLogBook> q_flightsLogBook;

        public FlightController() {
            q_pilot = db.Pilots.Where(row => row.PilotUserName == User.Identity.Name);
            q_flightsByPilot = db.vFlightAcftPilots.Where(row => row.PilotID == pilotid);
            //q_flightsByRoute = db.vFlightAcftPilots.Where(row => (row.RouteID == routeid));
            q_flightsAll = db.vFlightAcftPilots;
            q_flightsLogBook = db.vPilotLogBooks.Where(row => row.PilotID == pilotid).OrderByDescending(row => row.RouteID);
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated) {
                try {
                    pilotid = q_pilot.First().PilotID;
                    ViewBag.ViewTitle = "All My Flights";
                    ViewBag.ActionBack = "Index";
                    return View(q_flightsByPilot.ToList());
                }
                catch (Exception e) {
                    ViewBag.ExceptionErrorMessage = debugmode?e.Message:"Database Exception";
                    return View("ErrorPage");
                }
            }
            else return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            if (Request.IsAuthenticated)
            {
                if (Request["submit"] == "Update page")
                {
                    return RedirectToAction("Index");
                }
                if (Request["submit"] == "Delete selected flights")
                {
                    foreach (string id in form)
                    {
                        if (id.Contains("IsDelete") && form.GetValues(id).Contains("true"))
                        {
                            string rowid = id.Remove(0, 8);
                            Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
                            db.Flights.Remove(flight);
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index");
                }
                if (Request["submit"] == "Save changes")
                {
                    foreach (string id in form)
                    {
                        if (id.Contains("IsShare"))
                        {
                            string rowid = id.Remove(0, id.IndexOf(':') + 1);
                            if (ModelState.IsValid)
                            {
                                Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
                                if (form.GetValues(id).Contains("true") ^ (flight.IsShared.HasValue?flight.IsShared.Value:false))
                                {
                                    flight.IsShared = form.GetValues(id).Contains("true");
                                    db.Flights.Attach(flight);
                                    db.Entry(flight).Property(p => p.IsShared).IsModified = true;
                                    db.SaveChanges();
                                };
                            }
                        }
                        if (id.Contains("IsJunk"))
                        {
                            
                            string rowid = id.Remove(0, id.IndexOf(':') + 1);

                            if (ModelState.IsValid)
                            {
                                Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
                                if (form.GetValues(id).Contains("true") ^ (flight.IsJunk.HasValue ? flight.IsJunk.Value : false))
                                {
                                    flight.IsJunk = form.GetValues(id).Contains("true");
                                    db.Flights.Attach(flight);
                                    db.Entry(flight).Property(p => p.IsJunk).IsModified = true;
                                    db.SaveChanges();
                                };
                            }
                        }
                        if (id.Contains("FlightIdDropDown"))
                        {
                            string rowid = id.Remove(0,id.IndexOf(':')+1);
                            if (ModelState.IsValid)
                            {
                                Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
                                Int32 val = Int32.Parse(form.GetValues(id)[0]);
                                if (val != (flight.RouteID.HasValue ? flight.RouteID : flight.FlightID))
                                {
                                    flight.RouteID = val;
                                    db.Flights.Attach(flight);
                                    db.Entry(flight).Property(p => p.RouteID).IsModified = true;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult FlightsByAcft(int acftId)
        {
            if (Request.IsAuthenticated)
            {
                try {
                    pilotid = q_pilot.First().PilotID;
                    string acft = db.DimAircraftRemotes.Find(acftId).AcftNum;
                    ViewBag.ViewTitle = "My "+ acft+" Aircraft Flights";
                    ViewBag.ActionBack = "FlightsByAcft";
                    ViewBag.AcftID = acftId;
                    var flights = q_flightsByPilot.Where(row => row.AcftID == acftId).ToList();
                    return View("Index", flights);
                }
                catch (Exception e)
                {
                    ViewBag.ExceptionErrorMessage = debugmode ? e.Message : "Database Exception";
                    return View("ErrorPage");
                }
            }
            else return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlightsByAcft(FormCollection form)
        {
            var acftId = int.Parse(form.GetValues("acftId")[0]);
            //if (Request.IsAuthenticated)
            //{
            //    try
            //    {
            //        return RedirectToAction("Index");
            //    }
            //    catch (Exception e)
            //    {
            //        ViewBag.ExceptionErrorMessage = debugmode ? e.Message : "Database Exception";
            //        return View("ErrorPage");
            //    }
            //}
            //else return RedirectToAction("Login", "Account");
            if (Request.IsAuthenticated)
            {
                if (Request["submit"] == "Update page")
                {
                    return RedirectToAction("FlightsByAcft", new { acftId = acftId });
                }
                if (Request["submit"] == "Delete selected flights")
                {
                    foreach (string id in form)
                    {
                        if (id.Contains("IsDelete") && form.GetValues(id).Contains("true"))
                        {
                            string rowid = id.Remove(0, 8);
                            Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
                            db.Flights.Remove(flight);
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("FlightsByAcft", new { acftId = acftId });
                }
                if (Request["submit"] == "Save changes")
                {
                    foreach (string id in form)
                    {
                        if (id.Contains("IsShare"))
                        {
                            string rowid = id.Remove(0, id.IndexOf(':') + 1);
                            if (ModelState.IsValid)
                            {
                                Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
                                if (form.GetValues(id).Contains("true") ^ (flight.IsShared.HasValue ? flight.IsShared.Value : false))
                                {
                                    flight.IsShared = form.GetValues(id).Contains("true");
                                    db.Flights.Attach(flight);
                                    db.Entry(flight).Property(p => p.IsShared).IsModified = true;
                                    db.SaveChanges();
                                };
                            }
                        }
                        if (id.Contains("FlightIdDropDown"))
                        {
                            string rowid = id.Remove(0, id.IndexOf(':') + 1);
                            if (ModelState.IsValid)
                            {
                                Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
                                Int32 val = Int32.Parse(form.GetValues(id)[0]);
                                if (val != (flight.RouteID.HasValue ? flight.RouteID : flight.FlightID))
                                {
                                    flight.RouteID = val;
                                    db.Flights.Attach(flight);
                                    db.Entry(flight).Property(p => p.RouteID).IsModified = true;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("FlightsByAcft", new { acftId = acftId });
        }

        public ActionResult AircraftSearchGrid(int id =0)
        {
            ViewBag.AcftNum  = db.DimAircraftRemotes.Find(id).AcftNum;
            //if (Request.IsAuthenticated)
            //{
                try
                {
                    Flight flight = db.Flights.Where(row => row.AcftID == id).ToList()[0];
                    return View(db.Flights.Where(row => row.AcftID==id&&row.IsShared==true).ToList());
                }
                catch (Exception e)
                {
                    //ViewBag.ErrorMessage = "Can not connect to the database";
                    ViewBag.ExceptionErrorMessage = e.Message;
                    return View("ErrorPage");
                };
            //}
            //else return RedirectToAction("Login", "Account"); ;
        }

        public ActionResult DisplayMyFlightMovingMap(int id = 0, string FlightOrRoute = "Flight", int isAutoUpdate = 0, string linkUp = "Index")
        {
            ViewBag.FlightID = id;
            ViewBag.FlightOrRoute = FlightOrRoute;
            ViewBag.backUrl = "DisplayFlightData";
            ViewBag.linkUp = linkUp;
            ViewBag.isAutoUpdate = isAutoUpdate;
            return View();
        }

        public JsonResult GetFlightDataJson(int id = 0,string idtype = "Flight",int c = 0)
        {
            if (idtype == "Flight")
            {
                var gpslocations = db.GpsLocations.Where(row => row.FlightID == id).OrderBy(g => g.onSessionPointNum).Select(g => new { g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
                return this.Json(gpslocations, JsonRequestBehavior.AllowGet);
            }
            else if (idtype == "Route")
            {
                var routeID = db.Flights.Where(row => row.FlightID == id).Select(row => row.RouteID == null ? row.FlightID : row.RouteID).First();
                var flightIDs = db.Flights.Where(row => row.RouteID == routeID || row.FlightID == routeID).OrderBy(row => row.FlightID).Select(row => row.FlightID).ToArray();
                var gpslocations = db.GpsLocations.Where(row => flightIDs.Contains(row.FlightID)).OrderBy(g => g.FlightID).ThenBy(g => g.onSessionPointNum).Select(g => new { routeID, g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
                return this.Json(gpslocations, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        //public ActionResult CheckRouteFlightsCount(int id = 0, string actionBack = "")
        //{
        //    bool i = db.Flights.Where(row => (row.RouteID == id)).Count()>1;
        //    //return RedirectToAction("GetRouteFights", new { id, actionBack });

        //    return RedirectToAction(i?"GetRouteFlights":"DisplayFlightData",new {id,actionBack});
        //}

        //public ActionResult GetRouteFlights(int id, string actionBack)
        //{
        //    // route is shared if base flight is shared
        //    var isRouteShared = db.Flights.Where(row => (row.FlightID == id)).First().IsShared;
        //    if (Request.IsAuthenticated || (bool)isRouteShared)
        //    {

        //        try
        //        {
        //            ViewBag.ViewTitle = "Route " + id + " Flights";
        //            ViewBag.ActionBack = actionBack;
        //            ViewBag.isRouteShared = true;
        //            routeid = id;
        //            var q = q_flightsByRoute.ToList();
        //            return View("Index", q_flightsByRoute.ToList());
        //        }
        //        catch (Exception e)
        //        {
        //            ViewBag.ExceptionErrorMessage = debugmode ? e.Message : "Database Exception";
        //            return View("ErrorPage");
        //        }
        //    }
        //    else return RedirectToAction("Login", "Account");
        //}
        public ActionResult DisplayFlightData(int id = 0)
        {
            var isFlightShared = db.Flights.Where(row => row.FlightID == id).First().IsShared;
            if (Request.IsAuthenticated || (bool) isFlightShared)
            {
                var gpslocations = db.GpsLocations.Where(row => row.FlightID == id).OrderBy(row => row.onSessionPointNum).ToList();
                if (gpslocations == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Flightid = id;
                var routeid =db.Flights.Find(id).RouteID;
                ViewBag.Routeid = routeid == null ? id : routeid;
                ViewBag.Flightname = db.Flights.Find(id).FlightName;
                var acftid = db.Flights.Find(id).AcftID;
                var pltid = db.Flights.Find(id).PilotID;
                var ispattern = db.Flights.Find(id).IsPattern.HasValue ?db.Flights.Find(id).IsPattern : false;
                ViewBag.FlightAircraft = acftid != null ? db.DimAircraftRemotes.Find(acftid).AcftNum : String.Empty;
                ViewBag.FlightPilotCode = pltid != null ? db.Pilots.Find(pltid).PilotCode : String.Empty;
                ViewBag.FlightPilotName = pltid != null ? db.Pilots.Find(pltid).PilotName : String.Empty;
                ViewBag.FlightIsPattern = (bool)ispattern ? "Yes" : "No";
                ViewBag.FlightDuration = db.Flights.Find(id).FlightDurationMin;
                ViewBag.TrackFreq = db.Flights.Find(id).FreqSec;
                ViewBag.FlightDate = db.Flights.Find(id).FlightDate; // "dummy"; // item.FlightDateUTC.ToString("d",(new System.Globalization.CultureInfo("ja-JP")).DateTimeFormat)
                //ViewBag.ActionBack = actionBack;
                ViewBag.AcftId = acftid;
                return View(gpslocations);
            }
            else return RedirectToAction("Login", "Account"); ;
        }

        public FileContentResult DownloadFlightCSV(int id = 0)
        {
            var gpsloc = db.GpsLocations.Where(row => row.FlightID == id).OrderBy(row => row.onSessionPointNum).ToList();
            string csv = "Num,DateTime,Latitude,Longitude,Altitude_ft,AltitudeM,SpeedMph,SpeedKnot,SpeedKmpH" + Environment.NewLine;
            foreach (var loc in gpsloc)
            {
                csv = csv
                    + loc.onSessionPointNum.ToString() + ","
                    + loc.gpsTime.ToString() + ","
                    + loc.latitude.ToString() + ","
                    + loc.longitude.ToString() + ","
                    + loc.AltitudeFt.ToString() + ","
                    + loc.AltitudeM.ToString() + ","
                    + loc.SpeedMph.ToString() + ","
                    + loc.SpeedKnot.ToString() + ","
                    + loc.SpeedKmpH.ToString() 
                    + Environment.NewLine;
            }
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Flight_"+id+".csv");
        }
        public FileContentResult DownloadKMLStatic(int id = 0)
        {
            var route = db.Flights.Where(r => r.FlightID == id).Select(r => r.RouteID).FirstOrDefault();
            if (route == null) route = id; //patch if route is for some reason empty
            var flights = db.Flights.Where(r => r.RouteID == route).Select(r => new { r.FlightID }).ToList();

            var doc = new Document();
            doc.Id = "Route";
            doc.Name = "Route";
            var folder = new Folder();
            folder.Id = "Flights";
            folder.Name = "Flights";

            foreach (var f in flights)
            {
                var i = flights.IndexOf(f);
                var flightLineStyles = new FlightLineStyles(i);
                var docStyle = flightLineStyles.style;
                folder.AddStyle(docStyle);

                var placemark = new FlightPlacemarkLineString(f.FlightID);
                placemark.styleUrlRef = docStyle.Id;
                folder.AddFeature(placemark.placemark);
            }
            doc.AddFeature(folder);

            var kml = new Kml();                   
            kml.Feature = doc;
            KmlFile kmlFile = KmlFile.Create(kml, true);

            //using (var stream = System.IO.File.OpenWrite("C:/temp/kmlfile.kml"))
            //{
            //    kmlFile.Save(stream);

            //};

            using (var stream = new System.IO.MemoryStream())
            {
                kmlFile.Save(stream);
                var kmlFileName = "Flight_" + id + ".kml";
                var fileBytes = new System.Text.UTF8Encoding().GetBytes(new System.Text.UTF8Encoding().GetString(stream.ToArray()));
                return File(fileBytes, "application/vnd.google-earth.kml+xml", kmlFileName);
            };
        }

        //public FileContentResult DownloadKMLTimeLine2(int id = 0)
        //{
        //    var route = db.Flights.Where(r => r.FlightID == id).Select(r => r.RouteID).FirstOrDefault();
        //    if (route == null) route = id; //patch if route is for some reason empty
        //    var flights = db.Flights.Where(r => r.RouteID == route).Select(r => new { r.FlightID }).ToList();

        //    var doc = new Document();
        //    doc.Id = "Route";
        //    doc.Name = "Route";

        //    foreach (var f in flights)
        //    {
        //        var folder = new Folder();
        //        folder.Id = f.FlightID.ToString();
        //        folder.Name = "Flight " + f.FlightID;
        //        var i = flights.IndexOf(f);
        //        var flightPointStylesHide = new FlightPointStyles(i,0);
        //        var flightPointStylesShow = new FlightPointStyles(i,1);
        //        //var docStyle = flightPointStylesHide.style;
        //        folder.AddStyle(flightPointStylesHide.style);
        //        folder.AddStyle(flightPointStylesShow.style);
        //        var styleMap = new StyleMap(flightPointStylesHide.style, flightPointStylesShow.style).styleMap;
        //        folder.AddStyle(styleMap);
        //        var placemarkSet = new FlightPlacemarkPoint();
        //        placemarkSet.styleUrlRef = styleMap.Id;
        //        placemarkSet.getFlightPlacemarkPoints(f.FlightID);
        //        foreach (var p in placemarkSet.placemarks)
        //        {
        //            folder.AddFeature(p);
        //        }
        //        doc.AddFeature(folder);
        //    }

        //    var kml = new Kml();
        //    kml.Feature = doc;
        //    KmlFile kmlFile = KmlFile.Create(kml, true);

        //    //using (var stream = System.IO.File.OpenWrite("C:/temp/kmlfile.kml"))
        //    //{
        //    //    kmlFile.Save(stream);

        //    //};

        //    using (var stream = new System.IO.MemoryStream())
        //    {
        //        kmlFile.Save(stream);
        //        var kmlFileName = "Flight_" + id + ".kml";
        //        var fileBytes = new System.Text.UTF8Encoding().GetBytes(new System.Text.UTF8Encoding().GetString(stream.ToArray()));
        //        return File(fileBytes, "application/vnd.google-earth.kml+xml", kmlFileName);
        //    };
        //}
        public FileContentResult DownloadKMLTimeLine(int id = 0)
        {
            var route = db.Flights.Where(r => r.FlightID == id).Select(r => r.RouteID).FirstOrDefault();
            if (route == null) route = id; //patch if route is for some reason empty
            var flights = db.Flights.Where(r => r.RouteID == route).Select(r => new { r.FlightID }).ToList();

            var doc = new Document();
            doc.Id = "Route" + id;
            doc.Name = "Route " + id;

            foreach (var f in flights)
            {
                var folder = new Folder();
                folder.Id = f.FlightID.ToString();
                folder.Name = "Flight " + f.FlightID;
                var i = flights.IndexOf(f);

                var flightPointStylesHide = new FlightPointStyles(i, 0);
                var flightPointStylesShow = new FlightPointStyles(i, 1);
                //var docStyle = flightPointStylesHide.style;
                folder.AddStyle(flightPointStylesHide.style);
                folder.AddStyle(flightPointStylesShow.style);
                var styleMap = new StyleMap(flightPointStylesHide.style, flightPointStylesShow.style).styleMap;
                folder.AddStyle(styleMap);

                var flightLineStyles = new FlightLineStyles(i);
                var docStyleLine = flightLineStyles.style;
                folder.AddStyle(docStyleLine);

                var placemarkSet = new FlightPlacemarkPoint();
                placemarkSet.styleUrlRef = styleMap.Id;
                placemarkSet.getFlightPlacemarkPoints(f.FlightID);
                foreach (var p in placemarkSet.placemarks)
                {
                    folder.AddFeature(p);
                }

                var placemarkLineString = new FlightPlacemarkLineString(f.FlightID);
                placemarkLineString.styleUrlRef = docStyleLine.Id;
                folder.AddFeature(placemarkLineString.placemark);

                doc.AddFeature(folder);
            }

            var kml = new Kml();
            kml.Feature = doc;
            KmlFile kmlFile = KmlFile.Create(kml, true);

            //using (var stream = System.IO.File.OpenWrite("C:/temp/kmlfile.kml"))
            //{
            //    kmlFile.Save(stream);

            //};

            using (var stream = new System.IO.MemoryStream())
            {
                kmlFile.Save(stream);
                var kmlFileName = "Flight_" + id + ".kml";
                var fileBytes = new System.Text.UTF8Encoding().GetBytes(new System.Text.UTF8Encoding().GetString(stream.ToArray()));
                return File(fileBytes, "application/vnd.google-earth.kml+xml", kmlFileName);
            };
        }
        public ActionResult http()
        {
            ViewBag.ErrorMessage = "Default Error message";
            return View();
        }

        //public ActionResult Details(int id = 0)
        //{
        //    Flight flight = db.Flights.Find(id);
        //    if (flight == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(flight);
        //}

        public ActionResult GetRouteFlights(string flightIds)
        {
            var fids = flightIds.Split(',');
            //var gpslocations = db.GpsLocations.Where(row => fids.Containsrow.FlightID) //).OrderBy(g => g.FlightID).ThenByDescending(g => g.onSessionPointNum).Select(g => new { g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
            var gpslocations = db.GpsLocations.Where(row => fids.Contains(row.FlightID.ToString())).OrderBy(g => g.FlightID).ThenBy(g => g.onSessionPointNum).Select(g => new { g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.FlightsJsonData = serializer.Serialize(gpslocations);
            ViewBag.AreaCenterLat = gpslocations.FirstOrDefault().latitude;
            ViewBag.AreaCenterLong = gpslocations.FirstOrDefault().longitude;
            //ViewBag.ActionBack = "GetRouteFlights";
            return View("DisplayLatestFlightsStaticMap");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}