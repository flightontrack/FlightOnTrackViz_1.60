using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using MVC_Acft_Track.Models;
using MVC_Acft_Track.ViewModels;
using MVC_Acft_Track.Helpers;
using Ionic.Zip;
using PagedList;
using static MVC_Acft_Track.Finals;

namespace MVC_Acft_Track.Controllers
{
    public class MemberController : Controller
    {
        public bool debugmode = true;
        private Entities db;
        private qLINQ q;
        private int pilotid;

        //public IEnumerable<Pilot> u_pilot;
        //private IEnumerable<Pilot> q_pilot;
        //private IEnumerable<vFlightAcftPilot> q_flightsByPilot;
        //private IEnumerable<vFlightAcftPilot> q_flightsByRoute;
        //private IEnumerable<vFlightAcftPilot> q_flightsAll;
        //private IEnumerable<vPilotLogBook> q_flightsLogBook;

        public MemberController()
        {
            db = new Entities();
            q = new qLINQ(db);
//q.pilotUserName = User.Identity.Name;
            //var v = db.Pilots.Where(row => row.PilotUserName == User.Identity.Name);
            //var vv = v.Single();
            //pilotid = db.Pilots.Where(row => row.PilotUserName == User.Identity.Name).First().PilotID;
            ////pilotid = u_pilot.First().PilotID;
            //q.pilotId = pilotid;
            //q_pilot = db.Pilots.Where(row => row.PilotUserName == User.Identity.Name);
            //q_flightsByPilot = db.vFlightAcftPilots.Where(row => row.PilotID == pilotid);
            //q_flightsByRoute = db.vFlightAcftPilots.Where(row => (row.RouteID == routeid));
            //q_flightsAll = db.vFlightAcftPilots;
            //q_flightsLogBook = db.vPilotLogBooks.Where(row => row.PilotID == pilotid).OrderByDescending(row => row.RouteID);
        }

        public ActionResult indexMember(int menuitem=1, bool? buttonEnable = null,bool? successFlg = null, bool? successAirpt =null, string sort= "", string sortdir = "")
        {
            if (Request.IsAuthenticated)
            {
                q.pilotUserName = User.Identity.Name;
                var p = q.pilotEntity;
                if(successFlg.HasValue) ViewBag.Msg = ((bool)successFlg? MSG_SAVESUCCESS: MSG_SAVEFAIL);
                if (successAirpt.HasValue) {
                    q.airportCode = p.BaseAirport;
                    if (q.airportId == null)
                    {
                        var airpts = q.airportEntity.Select(r => new { Code = r.Code }).ToList();
                        if (airpts.Count() > 1) {
                            //ViewBag.AirportList = airpts.ToString();
                            ViewBag.MsgArpt=MSG_ARPTNOTFOUNDMULTIPLE+ airpts.ToString();
                        }
                        else ViewBag.MsgArpt = MSG_ARPTNOTFOUND;
                    }
                }
                ViewBag.successFlg = successFlg;
                ViewBag.SortDir = sortdir;
                switch (menuitem)
                {
                    case 1:
                        return View("MemberPilot", p);
                    case 2:
                        return View("MemberAcft", q.pilotAircrafts.ToList());
                    case 3:

                        //if (p == null) return View("LogBookNotFound");
                        var logBookList = q.pilotLogBook.ToList();

                        var timeLogBook = logBookList.Sum(item => item.FlightDurationMin) / 60;
                        var timeForward = p.TimeForward;// q.pilotTimeForwarded;

                        ViewBag.TimeForward = timeForward;
                        ViewBag.LogBookTimeHours = timeLogBook;
                        ViewBag.TotalTimeHours = timeForward + timeLogBook;

                        var landNumForward = p.LandingsForward;// q.pilotLandingsForwarded;
                        var landNumLogBook = logBookList.Sum(item => item.NoLandings);

                        ViewBag.LandNumForward = landNumForward;
                        ViewBag.LandNumLogBook = landNumLogBook;
                        ViewBag.LandNumTotal = landNumForward + landNumLogBook;

                        ViewBag.VLBReadyToDownload = (buttonEnable == null ? false : true);

                        return View("MemberLogbook", new vmPilotLogBook(logBookList, pilotid, timeForward, landNumForward));
                    case 4:
                        ViewBag.PilotFlightNum = q.flightsByPilot.Count();
                        ViewBag.MsgFightNum = "There are <span class=\"badge\">" + q.flightsByPilot.Count().ToString() + "</span> flights recorded in the history";
                        return View("MemberFlights", q.flightsByPilot.ToList());

                    default:
                        return View("MemberPilot", p);
                }
            }
            else return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult indexMember(FormCollection form, Pilot pilot = null)
        {
            if (Request.IsAuthenticated)
            {
                int i = Int32.Parse(form["menuitem"]);
                //q.pilotUserName = User.Identity.Name;
                //var pid = q.pilotId;
                bool btnEnabl = false;
                bool? successFlg = null;
                bool? successAirpt = null;
                if (ModelState.IsValid)
                {
                    try
                    {
                        switch (form["menuitem"])
                        {
                            case "1":
                                if (pilot.BaseAirport != null)
                                {
                                    if (pilot.BaseAirport.Length < 3)
                                        {
                                            ViewBag.MsgArpt = MSG_ARPTVALIDATION_FAILED ;
                                        successFlg = false;
                                        ViewBag.successFlg = successFlg;
                                        if (successFlg.HasValue) ViewBag.Msg = ((bool)successFlg ? MSG_SAVESUCCESS : MSG_SAVEFAIL);
                                        return View("MemberPilot", pilot);
                                    }
                                    q.airportCode = pilot.BaseAirport;
                                    if (q.airportId == null)
                                    {
                                        successFlg = false;
                                        ViewBag.successFlg = successFlg;
                                        ViewBag.MsgArpt = MSG_ARPTNOTFOUND;
                                        var airpts = q.airportEntity.Select(r => new {Airport=r.Code });
                                        if (airpts.Count() > 1)
                                        {
                                            ViewBag.MsgArpt = MSG_ARPTNOTFOUNDMULTIPLE + String.Join(",  ", airpts);
                                        }
                                        if (successFlg.HasValue) ViewBag.Msg = ((bool)successFlg ? MSG_SAVESUCCESS : MSG_SAVEFAIL);
                                        return View("MemberPilot", pilot);
                                    }
                                }
                                pilot.BaseAirport = q.airportCode;
                                db.Pilots.Attach(pilot);
                                db.Entry(pilot).Property(p => p.NameLast).IsModified = true;
                                db.Entry(pilot).Property(p => p.NameFirst).IsModified = true;
                                db.Entry(pilot).Property(p => p.BaseAirport).IsModified = true;
                                db.Entry(pilot).Property(p => p.IsShared).IsModified = true;
                                //db.Entry(pilot).Property(p => p.CertType).IsModified = true;
                                //db.Entry(pilot).Property(p => p.PilotName).IsModified = true;
                                db.SaveChanges();
                                successFlg = true;
                                break;
                            case "2":
                                break;
                            case "3":
                                //int pid = form["pilotId"] == null ? 0 : Int32.Parse(form["pilotId"]);
                                q.pilotUserName = User.Identity.Name;
                                var pid = q.pilotId;
                                if (!(form["UpdateForwards"] == null))
                                {
                                    int number1, number2;
                                    if (!(Int32.TryParse(form["timeForward"], out number1) && number1 >= 0)) return RedirectToAction("index_member_menu", new { menuitem = 3, pilotId = pid, buttonEnable = btnEnabl });
                                    if (!(Int32.TryParse(form["landForward"], out number2) && number2 >= 0)) return RedirectToAction("index_member_menu", new { menuitem = 3, pilotId = pid, buttonEnable = btnEnabl });
                                    if (ModelState.IsValid)
                                    {
                                        Pilot p =q.pilotEntity;
                                        p.TimeForward = number1;
                                        p.LandingsForward = number2;
                                        db.Pilots.Attach(p);
                                        db.Entry(p).Property(r => r.TimeForward).IsModified = true;
                                        db.Entry(p).Property(r => r.LandingsForward).IsModified = true;
                                        db.SaveChanges();
                                        successFlg = true;
                                    }
                                }
                                else if (!(form["DownloadFlightCSV"] == null))
                                {

                                    return DownloadLogBookCSV(pid);
                                }
                                else if (!(form["RequestVisualLogBook"] == null))
                                {
                                    btnEnabl = true;
                                }
                                else if (!(form["DownloadVLB"] == null))
                                {
                                    btnEnabl = false;
                                    return GenerateVisualLogbook(pid);
                                }
                                break;
                            case "4":
                                FlightUpdate(form);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        successFlg = false;
                    }
                }
                else { successFlg = false; }
                return RedirectToAction("indexMember", new { menuitem = i, successFlg = successFlg, successAirpt= successAirpt, buttonEnable = btnEnabl });
            }
            else return RedirectToAction("Login", "Account");
        }
        public ActionResult AcftEdit(int id = 0, int pilotid = 0)
        {
            var acft = db.vAircraftPilots.Find(id);
            if (acft == null)
            {
                return HttpNotFound();
            }
            return View(acft);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcftEdit(AircraftPilot acft)
        {
            if (Request.IsAuthenticated && ModelState.IsValid)
            {
                //int pid = q_pilot.First().PilotID;
                if (Request["submit"] == "Save Changes")
                {
                    db.AircraftPilots.Attach(acft);
                    db.Entry(acft).Property(p => p.AcftName).IsModified = true;
                    db.Entry(acft).Property(p => p.AcftNumLocal).IsModified = true;
                    db.SaveChanges();
                }
                if (Request["submit"] == "Delete the Aircraft")
                {
                    db.AircraftPilots.Attach(acft);
                    db.AircraftPilots.Remove(acft);
                    db.SaveChanges();
                }
                return RedirectToAction("IndexMember", new { menuitem = 2});

            }
            return View(acft);
        }

        void FlightUpdate(FormCollection form)
        {
            if (Request.IsAuthenticated)
            {
                q.pilotUserName = User.Identity.Name;
                //if (Request["submit"] == "Update page")
                if (form["UpdatePage"] != null)
                {
                    return;
                }
                if (form["DeleteSelectedFlight"] != null)
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
                    return;
                }
                if (form["SaveChanges"] != null)
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
            return;
        }

        public ActionResult FlightsByAcft(int acftId)
        {
            if (Request.IsAuthenticated)
            {
                q.pilotUserName = User.Identity.Name;
                try
                {
                    //pilotid = q_pilot.First().PilotID;
                    string acft = db.DimAircraftRemotes.Find(acftId).AcftNum;
                    ViewBag.ViewTitle = "My " + acft + " Aircraft Flights";
                    ViewBag.ActionBack = "FlightsByAcft";
                    ViewBag.AcftID = acftId;
                    var flights = q.flightsByPilot.Where(row => row.AcftID == acftId).ToList();
                    //ViewBag.MsgFightNum = "There are <span class=\"badge\">" + q.flightsByPilot.Count().ToString() + "</span> flights recorded in the history";
                    ViewBag.MsgFightNum = "There are <span class=\"badge\">" + flights.Count().ToString() + "</span> flights recorded for the aircraft <span class=\"badge\"> " + acft+ "</span>";
                    ViewBag.PilotFlightNum = flights.Count();
                    return View("MemberFlights", flights);
                }
                catch (Exception e)
                {
                    ViewBag.ExceptionErrorMessage = debugmode ? e.Message : "Database Exception";
                    return View("ExceptionPage");
                }
            }
            else return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlightsByAcft(FormCollection form)
        {
            if (Request.IsAuthenticated)
            {
                q.pilotUserName = User.Identity.Name;
                var acftId = int.Parse(form.GetValues("acftId")[0]);
            FlightUpdate(form);
            return RedirectToAction("FlightsByAcft", new { acftId = acftId });
            }
            else return RedirectToAction("Login", "Account");
        }

        public ActionResult AircraftSearchGrid(int id = 0)
        {
            ViewBag.AcftNum = db.DimAircraftRemotes.Find(id).AcftNum;
            //if (Request.IsAuthenticated)
            //{
            try
            {
                Flight flight = db.Flights.Where(row => row.AcftID == id).ToList()[0];
                return View(db.Flights.Where(row => row.AcftID == id && row.IsShared == true).ToList());
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

        //public ActionResult DisplayMyFlightMovingMap(int id = 0, string FlightOrRoute = "Flight", int isAutoUpdate = 0, string linkUp = "Index")
        //{
        //    ViewBag.FlightID = id;
        //    ViewBag.FlightOrRoute = FlightOrRoute;
        //    ViewBag.backUrl = "DisplayFlightData";
        //    ViewBag.linkUp = linkUp;
        //    ViewBag.isAutoUpdate = isAutoUpdate;
        //    return View();
        //}

        //public JsonResult GetFlightDataJson(int id = 0, string idtype = "Flight", int c = 0)
        //{
        //    if (idtype == "Flight")
        //    {
        //        var gpslocations = db.GpsLocations.Where(row => row.FlightID == id).OrderBy(g => g.onSessionPointNum).Select(g => new { g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
        //        return this.Json(gpslocations, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (idtype == "Route")
        //    {
        //        var routeID = db.Flights.Where(row => row.FlightID == id).Select(row => row.RouteID == null ? row.FlightID : row.RouteID).First();
        //        var flightIDs = db.Flights.Where(row => row.RouteID == routeID || row.FlightID == routeID).OrderBy(row => row.FlightID).Select(row => row.FlightID).ToArray();
        //        var gpslocations = db.GpsLocations.Where(row => flightIDs.Contains(row.FlightID)).OrderBy(g => g.FlightID).ThenBy(g => g.onSessionPointNum).Select(g => new { routeID, g.FlightID, g.onSessionPointNum, g.SpeedKnot, g.SpeedKmpH, g.gpsTimeOnly, g.AirportCode, g.AltitudeFt, g.AltitudeM, g.latitude, g.longitude }).ToList();
        //        return this.Json(gpslocations, JsonRequestBehavior.AllowGet);
        //    }
        //    return null;
        //}
        public ActionResult DisplayFlightData(int id = 0, string actionBack = "")
        {
            if (Request.IsAuthenticated) return RedirectToAction("DisplayFlightData", "Flight", new { id = id});
            else return RedirectToAction("Login", "Account");
        }

        //public FileContentResult DownloadFlightCSV(int id = 0)
        //{
        //    var gpsloc = db.GpsLocations.Where(row => row.FlightID == id).OrderBy(row => row.onSessionPointNum).ToList();
        //    string csv = "Num,DateTime,Latitude,Longitude,Altitude_ft,AltitudeM,SpeedMph,SpeedKnot,SpeedKmpH" + Environment.NewLine;
        //    foreach (var loc in gpsloc)
        //    {
        //        csv = csv
        //            + loc.onSessionPointNum.ToString() + ","
        //            + loc.gpsTime.ToString() + ","
        //            + loc.latitude.ToString() + ","
        //            + loc.longitude.ToString() + ","
        //            + loc.AltitudeFt.ToString() + ","
        //            + loc.AltitudeM.ToString() + ","
        //            + loc.SpeedMph.ToString() + ","
        //            + loc.SpeedKnot.ToString() + ","
        //            + loc.SpeedKmpH.ToString()
        //            + Environment.NewLine;
        //    }
        //    return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Flight_" + id + ".csv");
        //}
        //public FileContentResult DownloadKMLStatic(int id = 0)
        //{
        //    var route = db.Flights.Where(r => r.FlightID == id).Select(r => r.RouteID).FirstOrDefault();
        //    if (route == null) route = id; //patch if route is for some reason empty
        //    var flights = db.Flights.Where(r => r.RouteID == route).Select(r => new { r.FlightID }).ToList();

        //    var doc = new Document();
        //    doc.Id = "Route";
        //    doc.Name = "Route";
        //    var folder = new Folder();
        //    folder.Id = "Flights";
        //    folder.Name = "Flights";

        //    foreach (var f in flights)
        //    {
        //        var i = flights.IndexOf(f);
        //        var flightLineStyles = new FlightLineStyles(i);
        //        var docStyle = flightLineStyles.style;
        //        folder.AddStyle(docStyle);

        //        var placemark = new FlightPlacemarkLineString(f.FlightID);
        //        placemark.styleUrlRef = docStyle.Id;
        //        folder.AddFeature(placemark.placemark);
        //    }
        //    doc.AddFeature(folder);

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

        //public FileContentResult DownloadKMLTimeLine(int id = 0)
        //{
        //    var route = db.Flights.Where(r => r.FlightID == id).Select(r => r.RouteID).FirstOrDefault();
        //    if (route == null) route = id; //patch if route is for some reason empty
        //    var flights = db.Flights.Where(r => r.RouteID == route).Select(r => new { r.FlightID }).ToList();

        //    var doc = new Document();
        //    doc.Id = "Route" + id;
        //    doc.Name = "Route " + id;

        //    foreach (var f in flights)
        //    {
        //        var folder = new Folder();
        //        folder.Id = f.FlightID.ToString();
        //        folder.Name = "Flight " + f.FlightID;
        //        var i = flights.IndexOf(f);

        //        var flightPointStylesHide = new FlightPointStyles(i, 0);
        //        var flightPointStylesShow = new FlightPointStyles(i, 1);
        //        //var docStyle = flightPointStylesHide.style;
        //        folder.AddStyle(flightPointStylesHide.style);
        //        folder.AddStyle(flightPointStylesShow.style);
        //        var styleMap = new StyleMap(flightPointStylesHide.style, flightPointStylesShow.style).styleMap;
        //        folder.AddStyle(styleMap);

        //        var flightLineStyles = new FlightLineStyles(i);
        //        var docStyleLine = flightLineStyles.style;
        //        folder.AddStyle(docStyleLine);

        //        var placemarkSet = new FlightPlacemarkPoint();
        //        placemarkSet.styleUrlRef = styleMap.Id;
        //        placemarkSet.getFlightPlacemarkPoints(f.FlightID);
        //        foreach (var p in placemarkSet.placemarks)
        //        {
        //            folder.AddFeature(p);
        //        }

        //        var placemarkLineString = new FlightPlacemarkLineString(f.FlightID);
        //        placemarkLineString.styleUrlRef = docStyleLine.Id;
        //        folder.AddFeature(placemarkLineString.placemark);

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

        //public ActionResult Details(int id = 0)
        //{
        //    Flight flight = db.Flights.Find(id);
        //    if (flight == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(flight);
        //}

        public ActionResult FlightEdit(int id = 0, bool? successFlg = null)
        {
            if (Request.IsAuthenticated)
            {
            q.pilotUserName = User.Identity.Name;
            Flight flight = db.Flights.Find(id);
            if (successFlg.HasValue) ViewBag.Msg = ((bool)successFlg ? MSG_SAVESUCCESS : MSG_SAVEFAIL);
            ViewBag.successFlg = successFlg;

            if (flight == null)
            {
                return HttpNotFound();
            }
            var acftID = flight.AcftID;
            var pilotID = flight.PilotID;
            //int selectedID = db.vAircraftPilots.Where(row=>(row.AcftID==acftID && row.PilotID==pilotID)).Select(row => row.ID).First();
            int? selectedID = flight.AcftPilotID;

            ViewBag.AircraftsSelList = new SelectList(db.vAircraftPilots.Where(row => (row.PilotID == pilotID)).OrderBy(row => row.AcftNumLocal), "ID", "AcftNumLocal", selectedID);
            //if (User.Identity.Name.Contains("9784295693")) ViewBag.AircraftsSelList = new SelectList(db.vAircraftPilots.OrderBy(row => row.AcftRegNum), "AcftID", "AcftNumLocal", acftID); ;
            return View(flight);
            }
            else return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlightEdit(Flight flight, string AircraftsSelList)
        {
            bool? successFlg;
            if (Request.IsAuthenticated)
            {
            q.pilotUserName = User.Identity.Name;
            if (ModelState.IsValid)
            {
                try
                {
                    //db.Entry(flight.FlightName).State = EntityState.Modified;
                    var acftPilotID = int.Parse(AircraftsSelList);
                    flight.AcftPilotID = acftPilotID;
                    //var acftId = db.AircraftPilots.Where(row => row.ID == int.Parse(AircraftsSelList)).First().AcftID;
                    flight.AcftID = db.AircraftPilots.Where(row => (row.ID == acftPilotID)).First().AcftID;
                    db.Flights.Attach(flight);
                    db.Entry(flight).Property(f => f.AcftID).IsModified = true;
                    db.Entry(flight).Property(f => f.AcftPilotID).IsModified = true;
                    db.Entry(flight).Property(f => f.FlightName).IsModified = true;
                    db.Entry(flight).Property(f => f.Comments).IsModified = true;
                    db.SaveChanges();
                    successFlg = true;
                    return RedirectToAction("FlightsByAcft", new { acftId = flight.AcftID });
                }
                catch(Exception e)
                {
                    successFlg = false;
                    return RedirectToAction("FlightEdit", new { successFlg = successFlg });
                }
            }
            return View(flight);
            }
            else return RedirectToAction("Login", "Account");
        }

        public FileContentResult DownloadLogBookCSV(FormCollection form)
        {
            q.pilotId = form["pilotId"] == null ? 0 : Int32.Parse(form["pilotId"]);
            var pilotLogBook = q.pilotLogBook.ToList();
            string csv = "Date,Acft MMS,Acft,Airports,Landings,Minutes,Comments" + Environment.NewLine;
            foreach (var rec in pilotLogBook)
            {
                csv = csv
                    + '"' + rec.FlightDateOnly.ToString() + '"' + ","
                    + '"' + rec.AcftMMS.ToString() + '"' + ","
                    + '"' + rec.Acft.ToString() + '"' + ","
                    + '"' + rec.RouteName.ToString() + '"' + ","
                    + '"' + rec.NoLandings.ToString() + '"' + ","
                    + '"' + rec.FlightDurationMin.ToString() + '"' + ","
                    + (rec.Comments == null ? "" : rec.Comments.ToString())
                    + Environment.NewLine;
            }
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "PilotLogBook_" + ".csv");
        }
        public FileContentResult DownloadLogBookCSV(int pid)
        {
            q.pilotId = pid;
            var pilotLogBook = q.pilotLogBook.ToList();
            string csv = "Date,Acft MMS,Acft,Airports,Landings,Minutes,Comments" + Environment.NewLine;
            foreach (var rec in pilotLogBook)
            {
                csv = csv
                    + '"' + rec.FlightDateOnly.ToString() + '"' + ","
                    + '"' + rec.AcftMMS.ToString() + '"' + ","
                    + '"' + rec.Acft.ToString() + '"' + ","
                    + '"' + rec.RouteName.ToString() + '"' + ","
                    + '"' + rec.NoLandings.ToString() + '"' + ","
                    + '"' + rec.FlightDurationMin.ToString() + '"' + ","
                    + (rec.Comments == null ? "" : rec.Comments.ToString())
                    + Environment.NewLine;
            }
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "PilotLogBook_" + ".csv");
        }

        public FileContentResult GenerateVisualLogbook(int pid)
        {
            q.pilotId = pid;

            //var pilotLogBook = q.visualPilotLogBook.ToList();
            var pilotLogBook = db.fVisualPilotLogBook(pid).ToList();
            var pilotLogDestinations = db.fVisualPilotLogDestinations(pid).ToList();
            int? routeID = 0;

            string csvVisualPilotLogBook = "RouteID,FlightDate,AcftMMS,Acft,AcftRegNum,PilotID,RouteDurationMin,RouteName,NoLandings,FlightID,FlightName,FlightDurationMin,Comments,Points,GPSLocationID,order_id,AirportCode,longitude,latitude" + Environment.NewLine;
            try
            {
                foreach (var rec in pilotLogBook)
                {
                    routeID = rec.RouteID;
                    csvVisualPilotLogBook = csvVisualPilotLogBook
                        + '"' + rec.RouteID.ToString() + '"' + ","
                        + '"' + rec.FlightDate.ToString() + '"' + ","
                        + '"' + rec.AcftMMS.ToString() + '"' + ","
                        + '"' + rec.Acft.ToString() + '"' + ","
                        + '"' + rec.AcftRegNum.ToString() + '"' + ","
                        + '"' + rec.PilotID.ToString() + '"' + ","
                        + '"' + rec.RouteDurationMin.ToString() + '"' + ","
                        + '"' + (rec.RouteName == null ? "" : rec.RouteName) + '"' + ","
                        + '"' + rec.NoLandings.ToString() + '"' + ","
                        + '"' + rec.FlightID.ToString() + '"' + ","
                        + '"' + (rec.FlightName.ToString() == null ? "" : rec.FlightName) + '"' + ","
                        + '"' + (rec.FlightDurationMin == null ? "" : rec.FlightDurationMin.ToString()) + '"' + ","
                        + '"' + (rec.Comments == null ? "" : rec.Comments.ToString()) + '"' + ","
                        + '"' + rec.Points.ToString() + '"' + ","
                        + '"' + rec.GPSLocationID.ToString() + '"' + ","
                        + '"' + rec.order_id.ToString() + '"' + ","
                        + '"' + rec.AirportCode.ToString() + '"' + ","
                        + '"' + rec.longitude.ToString() + '"' + ","
                        + '"' + rec.latitude.ToString() + '"'
                        + Environment.NewLine;
                }
            }
            catch (Exception e)
            {
                int? r = routeID;
                csvVisualPilotLogBook = csvVisualPilotLogBook + Environment.NewLine + "LogBook generation failed on RouteID = " + r + Environment.NewLine + "Some fields are null";
            }
            string csvPilotLogDestinations = "PilotID,FlightID,flightN,longitude,latitude,dest_order_id,AirportCode,flightweight" + Environment.NewLine;
            foreach (var rec in pilotLogDestinations)
            {
                csvPilotLogDestinations = csvPilotLogDestinations
                    + '"' + rec.PilotID.ToString() + '"' + ","
                    + '"' + rec.FlightID.ToString() + '"' + ","
                    + '"' + rec.flightN.ToString() + '"' + ","
                    + '"' + rec.longitude.ToString() + '"' + ","
                    + '"' + rec.latitude.ToString() + '"' + ","
                    + '"' + rec.dest_order_id.ToString() + '"' + ","
                    + '"' + rec.AirportCode.ToString() + '"' + ","
                    + '"' + rec.flightweight.ToString() + '"' + ","
                    + Environment.NewLine;
            }
            using (var outputStream = new System.IO.MemoryStream())
            {
                using (var zip = new ZipFile())
                {
                    zip.AddEntry("Data/vVisualPilotLogBook.csv", csvVisualPilotLogBook);
                    zip.AddEntry("Data/vVisualPilotLogDestinations.csv", csvPilotLogDestinations);
                    zip.AddDirectory(Server.MapPath("~/Tableau/"));

                    zip.Save(outputStream);
                    outputStream.Position = 0;
                    return File(outputStream.ToArray(), "application/zip", "VisualLogBook.twbxz");
                }
            };

        }

        [HttpGet]
        public ActionResult PilotLogBookMobile(string pilotUserName)
        {
            ViewBag.PilotUserName = pilotUserName;
            q.pilotUserName = pilotUserName;
            //var p = db.Pilots.Where(r => r.PilotUserName.Equals(pilotUserName)).FirstOrDefault();
            var p = q.pilotEntity;
            if (p == null) return View("LogBookNotFound");

            //pilotid = p.PilotID;
            //ViewBag.PilotId = p.PilotID;
            ViewBag.ViewTitle = "My LogBook";
            ViewBag.ActionBack = "Index";
            //var logBookList = q_flightsLogBook.ToList();

            var logBookList = q.pilotLogBook.ToList();
            var timeLogBook = logBookList.Sum(item => item.FlightDurationMin) / 60;
            //var timeForward = db.Pilots.Where(r => r.PilotID == pilotid).FirstOrDefault().TimeForward;
            var timeForward = p.TimeForward;// q.pilotTimeForwarded;
            ViewBag.TimeForward = timeForward;
            ViewBag.LogBookTimeHours = timeLogBook;
            ViewBag.TotalTimeHours = timeForward + timeLogBook;

            //var landNumForward = db.Pilots.Where(r => r.PilotID == pilotid).FirstOrDefault().LandingsForward; ;
            var landNumForward = p.LandingsForward;// q.pilotLandingsForwarded;
            var landNumLogBook = logBookList.Sum(item => item.NoLandings);

            ViewBag.LandNumForward = landNumForward;
            ViewBag.LandNumLogBook = landNumLogBook;
            ViewBag.LandNumTotal = landNumForward + landNumLogBook;

            //List<vPilotLogBook> logBookRecords = q_flightsLogBook.ToList();

            var vmpilotlogbook = new vmPilotLogBook(logBookList, q.pilotId, timeForward, landNumForward);

            return View(vmpilotlogbook);
        }
        [HttpPost]
        public ActionResult PilotLogBookMobile(FormCollection form)
        {
            string pilotUserName = "";
            int pid = 0;
            if (!(form["submit"] == null))
            {
                pid = Int32.Parse(form["pilotId"]);

                Pilot pilot = db.Pilots.Find(pid);
                pilotUserName = pilot.PilotUserName;
                int number1, number2;
                if (!(Int32.TryParse(form["timeForward"], out number1) && number1 >= 0)) return RedirectToAction("PilotLogBookMobile", new { pilotUserName = pilotUserName });
                if (!(Int32.TryParse(form["landForward"], out number2) && number2 >= 0)) return RedirectToAction("PilotLogBookMobile", new { pilotUserName = pilotUserName });
                if (ModelState.IsValid)
                {
                    pilot.TimeForward = number1;
                    pilot.LandingsForward = number2;
                    db.Pilots.Attach(pilot);
                    db.Entry(pilot).Property(p => p.TimeForward).IsModified = true;
                    db.Entry(pilot).Property(p => p.LandingsForward).IsModified = true;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("PilotLogBookMobile", new { pilotUserName = pilotUserName });
        }

        public ActionResult http()
        {
            ViewBag.ErrorMessage = "Default Error message";
            return View();
        }
    }
}