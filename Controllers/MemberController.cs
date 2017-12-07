using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using FontNameSpace.Models;
using FontNameSpace.ViewModels;
using FontNameSpace.Helpers;
using Ionic.Zip;
using PagedList;
using static FontNameSpace.Finals;
using static FontNameSpace.App;
using System.Web.Helpers;

namespace FontNameSpace.Controllers
{
    //[RequireHttps]
    [Authorize]
    public class MemberController : Controller
    {
        //public bool isDebugMode = true;
        private Entities db;
        private Queryables q;
        private int pilotid;

        public MemberController()
        {
            db = new Entities();
            q = new Queryables(db);
        }

        public ActionResult indexMember(bool isSearchJunk = false, int menuitem = 1, int? acftId = null, bool? buttonEnable = null,bool? successFlg = null, string sort= "", string sortdir = "",int page=1)
        {
            //if (Request.IsAuthenticated)
            //{
                q.pilotUserName = User.Identity.Name;
                var p = q.pilotEntity;
                if(successFlg.HasValue) ViewBag.Msg = ((bool)successFlg? MSG_SAVESUCCESS: MSG_SAVEFAIL);
                //if (successAirpt.HasValue) {
                //    q.airportCode = p.BaseAirport;
                //    if (q.airportId == null)
                //    {
                //        var airpts = q.airportEntity.Select(r => new { Code = r.Code }).ToList();
                //        if (airpts.Count() > 1) {
                //            //ViewBag.AirportList = airpts.ToString();
                //            ViewBag.MsgArpt=MSG_ARPTNOTFOUNDMULTIPLE+ airpts.ToString();
                //        }
                //        else ViewBag.MsgArpt = MSG_ARPTNOTFOUND;
                //    }
                //}
                ViewBag.successFlg = successFlg;
                ViewBag.SortDir = sortdir;
                switch (menuitem)
                {
                    case 1:
                        return View("MemberPilot", p);
                    case 2:
                        return View("MemberAcft", q.aircraftsByPilot.ToList());
                    case 3:

                        //if (p == null) return View("LogBookNotFound");
                        var logBookList = q.pilotLogBook.ToList();
                        var minLogBookDate = System.Convert.ToDateTime(logBookList.Min(item => item.FlightDateOnly));

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

                        return View("MemberLogbook", new vmPilotLogBook(logBookList, pilotid, minLogBookDate, timeForward, landNumForward));
                    case 4:
                        return View("MemberFlights", new vmSearchRequestFights(new vmSearchRequest { pilotID = p.PilotID, aicrftID = acftId, isSearchJunk = isSearchJunk },15,10000, sort, sortdir.Equals("ASC") ? SortDirection.Ascending : SortDirection.Descending));
                    default:
                        return View("MemberPilot", p);
                }
            //}
            //else return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult indexMember(FormCollection form, vmSearchRequest searchRequest, List<vFlightAcftPilot> flightList, Pilot pilot = null)
        {
            //if (Request.IsAuthenticated)
            //{
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
                                        successFlg = false;
                                        ViewBag.MsgArpt = MSG_ARPTVALIDATION_FAILED ;
                                        //ViewBag.successFlg = successFlg;
                                        //if (successFlg.HasValue) ViewBag.Msg = ((bool)successFlg ? MSG_SAVESUCCESS : MSG_SAVEFAIL);
                                        //return View("MemberPilot", pilot);
                                    }
                                    else
                                    {
                                        q.airportCode = pilot.BaseAirport;
                                        if (q.airportId == null)
                                        {
                                            successFlg = false;
                                            //ViewBag.successFlg = successFlg;                                            
                                            var airpts = q.airportEntity.Select(r => new {Airport=r.Code });
                                            if (airpts.Count() > 1)
                                            {
                                                ViewBag.MsgArpt = MSG_ARPTNOTFOUNDMULTIPLE + String.Join(",  ", airpts);
                                            }
                                            else ViewBag.MsgArpt = MSG_ARPTNOTFOUND;
                                        }
                                    }
                                    if (successFlg.HasValue)
                                    {
                                        ViewBag.Msg = MSG_SAVEFAIL;
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
                                if (ModelState.IsValid)
                                {
                                    var buttonClicked = Request["buttonClicked"];
                                    ClassShared.FormHandle(buttonClicked, flightList);
                                    searchRequest.isSearchJunk = (Request["buttonClicked"] == "SelectAllGarbage");
                                    //return RedirectToAction("indexMember", searchRequest);
                                }
                                else
                                {
                                    var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors }).ToString();
                                    LogHelper.onFailureLog("CreateUserPilot()", errors);
                                    ViewBag.ExceptionErrorMessage = isDebugMode ? errors : "POST GetFlights() error";
                                    return View("ExceptionPage");
                                }                                
                                //FlightUpdate(form);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        successFlg = false;
                    }
                }
                else { successFlg = false; }
                return RedirectToAction("indexMember", new { isSearchJunk = searchRequest.isSearchJunk, menuitem = i, successFlg = successFlg, buttonEnable = btnEnabl });
            //}
            //else return RedirectToAction("Login", "Account");
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
            //if (Request.IsAuthenticated && ModelState.IsValid)
            if (ModelState.IsValid)
            {
                //int pid = q_pilot.First().PilotID;
                if (Request["submit"] == "Save Changes")
                {
                    db.AircraftPilots.Attach(acft);
                    db.Entry(acft).Property(p => p.AcftName).IsModified = true;
                    db.Entry(acft).Property(p => p.AcftNumLocal).IsModified = true;
                    db.Entry(acft).Property(p => p.URLPictures).IsModified = true;
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

        //void FlightUpdate(FormCollection form)
        //{
        //    if (Request.IsAuthenticated)
        //    {
        //        q.pilotUserName = User.Identity.Name;
        //        //if (Request["submit"] == "Update page")
        //        if (form["UpdatePage"] != null)
        //        {
        //            return;
        //        }
        //        if (form["DeleteSelectedFlight"] != null)
        //        {
        //            foreach (string id in form)
        //            {
        //                if (id.Contains("IsDelete") && form.GetValues(id).Contains("true"))
        //                {
        //                    string rowid = id.Remove(0, 8);
        //                    Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
        //                    db.Flights.Remove(flight);
        //                    db.SaveChanges();
        //                }
        //            }
        //            return;
        //        }
        //        if (form["SaveChanges"] != null)
        //        {
        //            foreach (string id in form)
        //            {
        //                if (id.Contains("IsShare"))
        //                {
        //                    string rowid = id.Remove(0, id.IndexOf(':') + 1);
        //                    if (ModelState.IsValid)
        //                    {
        //                        Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
        //                        if (form.GetValues(id).Contains("true") ^ (flight.IsShared.HasValue ? flight.IsShared.Value : false))
        //                        {
        //                            flight.IsShared = form.GetValues(id).Contains("true");
        //                            db.Flights.Attach(flight);
        //                            db.Entry(flight).Property(p => p.IsShared).IsModified = true;
        //                            db.SaveChanges();
        //                        };
        //                    }
        //                }
        //                if (id.Contains("IsJunk"))
        //                {
        //                    string rowid = id.Remove(0, id.IndexOf(':') + 1);

        //                    if (ModelState.IsValid)
        //                    {
        //                        Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
        //                        if (form.GetValues(id).Contains("true") ^ (flight.IsJunk.HasValue ? flight.IsJunk.Value : false))
        //                        {
        //                            flight.IsJunk = form.GetValues(id).Contains("true");
        //                            db.Flights.Attach(flight);
        //                            db.Entry(flight).Property(p => p.IsJunk).IsModified = true;
        //                            db.SaveChanges();
        //                        };
        //                    }
        //                }
        //                if (id.Contains("FlightIdDropDown"))
        //                {
        //                    string rowid = id.Remove(0, id.IndexOf(':') + 1);
        //                    if (ModelState.IsValid)
        //                    {
        //                        Flight flight = db.Flights.Find(Convert.ToInt32(rowid));
        //                        Int32 val = Int32.Parse(form.GetValues(id)[0]);
        //                        if (val != (flight.RouteID.HasValue ? flight.RouteID : flight.FlightID))
        //                        {
        //                            flight.RouteID = val;
        //                            db.Flights.Attach(flight);
        //                            db.Entry(flight).Property(p => p.RouteID).IsModified = true;
        //                            db.SaveChanges();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return;
        //}

        public ActionResult FlightsByAcft(int acftId)
        {
            //if (Request.IsAuthenticated)
            //{
                q.pilotUserName = User.Identity.Name;
                try
                {
                    string acft = db.DimAircraftRemotes.Find(acftId).AcftNum;
                    ViewBag.ViewTitle = "My " + acft + " Aircraft Flights";
                    ViewBag.ActionBack = "FlightsByAcft";
                    ViewBag.AcftID = acftId;
                    var flights = q.flightsByPilot.Where(row => row.AcftID == acftId).ToList();
                    //ViewBag.MsgFightNum = "There are <span class=\"badge\">" + q.flightsByPilot.Count().ToString() + "</span> flights recorded in the history";
                    ViewBag.MsgFightNum = "There are <span class=\"badge\">" + flights.Count().ToString() + "</span> flights recorded for the aircraft <span class=\"badge\"> " + acft+ "</span>";
                    ViewBag.PilotFlightNum = flights.Count();
                //return View("MemberFlights", new vmSearchRequestFights(new vmSearchRequest { pilotID = q.pilotId, aicrftID = acftId}, 15, 10000));
                return RedirectToAction("indexMember", new { menuitem = 4, acftId = acftId });

            }
            catch (Exception e)
                {
                    ViewBag.ExceptionErrorMessage = isDebugMode ? e.Message : "Database Exception";
                    return View("ExceptionPage");
                }
            //}
            //else return RedirectToAction("Login", "Account");
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult FlightsByAcft(FormCollection form)
        //{
        //    //if (Request.IsAuthenticated)
        //    //{
        //    q.pilotUserName = User.Identity.Name;
        //    var acftId = int.Parse(form.GetValues("acftId")[0]);
        //    FlightUpdate(form);
        //    return RedirectToAction("FlightsByAcft", new { acftId = acftId });
        //    //return RedirectToAction("indexMember", new { menuitem = 4, acftId = acftId });
        //    //}
        //    //else return RedirectToAction("Login", "Account");
        //}

        //public ActionResult AircraftSearchGrid(int id = 0)
        //{
        //    ViewBag.AcftNum = db.DimAircraftRemotes.Find(id).AcftNum;
        //    //if (Request.IsAuthenticated)
        //    //{
        //    try
        //    {
        //        Flight flight = db.Flights.Where(row => row.AcftID == id).ToList()[0];
        //        return View(db.Flights.Where(row => row.AcftID == id && row.IsShared == true).ToList());
        //    }
        //    catch (Exception e)
        //    {
        //        //ViewBag.ErrorMessage = "Can not connect to the database";
        //        ViewBag.ExceptionErrorMessage = e.Message;
        //        return View("ErrorPage");
        //    };
        //    //}
        //    //else return RedirectToAction("Login", "Account"); ;
        //}

        [AllowAnonymous]
        public ActionResult DisplayFlightData(int id = 0, string actionBack = "")
        {
            if (Request.IsAuthenticated) return RedirectToAction("DisplayFlightData", "Flight", new { id = id});
            else return RedirectToAction("Login", "Account");
        }

        public ActionResult FlightEdit(int id = 0, bool? successFlg = null, int page=1)
        {
            //if (Request.IsAuthenticated)
            //{
            q.pilotUserName = User.Identity.Name;
            Flight flight = db.Flights.Find(id);
            if (successFlg.HasValue) ViewBag.Msg = ((bool)successFlg ? MSG_SAVESUCCESS : MSG_SAVEFAIL);
            ViewBag.successFlg = successFlg;

            if (flight == null)
            {
                return HttpNotFound();
            }
                var pilotID = flight.PilotID;
                int? selectedAcftPilotID = flight.AcftPilotID;

                ViewBag.AircraftsSelList = new SelectList(db.vAircraftPilots.Where(row => (row.PilotID == pilotID)).OrderBy(row => row.AcftNumLocal), "ID", "AcftNumLocal", selectedAcftPilotID);
                ViewBag.FlightSelList = new SelectList(db.Flights.Where(row => (row.PilotID == pilotID)).OrderByDescending(row => row.FlightID), "FlightID", "FlightID", flight.RouteID == null ? flight.FlightID : flight.RouteID);
                ViewBag.Page = page;
                return View(flight);
            //}
            //else return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlightEdit(Flight flight, string AircraftsSelList,int FlightSelList,int page)
        {
            //bool? successFlg;
            //if (Request.IsAuthenticated)
            //{
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
                    flight.RouteID = FlightSelList;
                    db.Flights.Attach(flight);
                    db.Entry(flight).Property(f => f.AcftID).IsModified = true;
                    db.Entry(flight).Property(f => f.AcftPilotID).IsModified = true;
                    db.Entry(flight).Property(f => f.FlightName).IsModified = true;
                    db.Entry(flight).Property(f => f.Comments).IsModified = true;
                    db.Entry(flight).Property(f => f.RouteID).IsModified = true;
                    db.SaveChanges();
                    //successFlg = true;
                    return RedirectToAction("indexMember", new { menuitem = 4, page=page });
                }
                catch(Exception e)
                {
                    //successFlg = false;
                    return RedirectToAction("FlightEdit", new { id = flight.FlightID, successFlg = false });
                }
            }
            return View(flight);
            //}
            //else return RedirectToAction("Login", "Account");
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
            string csv = "Date,Acft MMS,Acft ,From/To,Remarks,Landings,Duration (h)" + Environment.NewLine + Environment.NewLine;
            int i = 0;
            int? nlsum = 0;
            int? nlpp = 0;
            float? hsumpp = 0;
            float? hsum = 0;
            foreach (var rec in pilotLogBook)
            {
                i++;
                nlpp += rec.NoLandings;
                var h = rec.FlightDurationMin / 60.0f;
                hsumpp += h;
                csv = csv
                    + '"' + rec.FlightDateOnly.ToString() + '"' + ","
                    + '"' + rec.AcftMMS.ToString() + '"' + ","
                    + '"' + rec.Acft.ToString() + '"' + ","
                    + '"' + rec.RouteName.ToString() + '"' + ","
                    + '"' + (rec.Comments == null ? "" : rec.Comments.ToString())+ '"' + ","
                    + '"' + rec.NoLandings.ToString() + '"' + ","
                    + '"' + String.Format("{0:F2}", h) + '"' + ","
                    + '"' + rec.RouteID.ToString() + '"' + ","
                    + Environment.NewLine;
                if (i % 7 == 0) {
                    hsum += hsumpp;
                    nlsum += nlpp;
                    csv = csv
                    + Environment.NewLine
                    + '"' + '"' + "," + '"' + '"' + "," + '"' + '"' + "," + '"' + '"' + "," + "Page Total" + "," + nlpp.ToString() + "," + String.Format("{0:F2}", hsumpp) + Environment.NewLine
                    + '"' + '"' + "," + '"' + '"' + "," + '"' + '"' + "," + '"' + '"' + "," + "Amt. Forward" + Environment.NewLine
                    + '"' + '"' + "," + '"' + '"' + "," + '"' + '"' + "," + '"' + '"' + "," + "Total To Date" + "," + nlsum.ToString() + "," + String.Format("{0:F2}", hsum)
                    + Environment.NewLine + Environment.NewLine;
                    nlpp = 0;
                    hsumpp = 0;
                }
            }
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "PilotLogBook_" + ".csv");
        }

        public FileContentResult GenerateVisualLogbook(int pid)
        {
            q.pilotId = pid;

            //var pilotLogBook = q.visualPilotLogBook.ToList();
            var pilotLogBook = q.visualPilotLogBook.OrderByDescending(r => r.RouteID).ToList();
            var pilotLogDestinations = db.vVisualPilotLogDestinations.Where(r => r.PilotID == pid).OrderBy(r=>r.RouteID).ToList();
            int? routeID = 0;

            string csvVisualPilotLogBook = "RouteID,FlightDate,AcftMMS,AcftRegNum,PilotID,RouteDurationMin,RouteName,NoLandings,AirportCode" + Environment.NewLine;
            try
            {
                foreach (var rec in pilotLogBook)
                {
                    routeID = rec.RouteID;
                    csvVisualPilotLogBook = csvVisualPilotLogBook
                        + '"' + rec.RouteID.ToString() + '"' + ","
                        + '"' + rec.FlightDate.ToString() + '"' + ","
                        + '"' + rec.AcftMMS.ToString() + '"' + ","
                        //+ '"' + rec.Acft.ToString() + '"' + ","
                        + '"' + rec.AcftRegNum.ToString() + '"' + ","
                        + '"' + rec.PilotID.ToString() + '"' + ","
                        + '"' + rec.RouteDurationMin.ToString() + '"' + ","
                        + '"' + (rec.RouteName == null ? "" : rec.RouteName) + '"' + ","
                        + '"' + rec.RouteNoLandings.ToString() + '"' + ","
                        //+ '"' + rec.FlightID.ToString() + '"' + ","
                        //+ '"' + (rec.FlightName.ToString() == null ? "" : rec.FlightName) + '"' + ","
                        //+ '"' + (rec.FlightDurationMin == null ? "" : rec.FlightDurationMin.ToString()) + '"' + ","
                        //+ '"' + (rec.Comments == null ? "" : rec.Comments.ToString()) + '"' + ","
                        //+ '"' + rec.Points.ToString() + '"' + ","
                        //+ '"' + rec.GPSLocationID.ToString() + '"' + ","
                        //+ '"' + rec.order_id.ToString() + '"' + ","
                        + '"' + rec.AirportCode.ToString() + '"' + ","
                        //+ '"' + rec.longitude.ToString() + '"' + ","
                        //+ '"' + rec.latitude.ToString() + '"'
                        + Environment.NewLine;
                }
                //return File(new System.Text.UTF8Encoding().GetBytes(csvVisualPilotLogBook), "text/csv", "VisualPilotLogBook_" + ".csv");
            }
            catch (Exception e)
            {
                int? r = routeID;
                csvVisualPilotLogBook = csvVisualPilotLogBook + Environment.NewLine + "LogBook generation failed on RouteID = " + r + Environment.NewLine + "Some fields are null";
            }
            string csvPilotLogDestinations = "PilotID,RouteID,LineN,RouteName,longitude,latitude,AirportCode,flightweight" + Environment.NewLine;
            try
            {
                foreach (var rec in pilotLogDestinations)
                {
                    routeID = rec.RouteID;
                    csvPilotLogDestinations = csvPilotLogDestinations
                        + '"' + rec.PilotID.ToString() + '"' + ","
                        + '"' + rec.RouteID.ToString() + '"' + ","
                        + '"' + rec.LineN.ToString() + '"' + ","
                        + '"' + (rec.RouteName ?? "").ToString() + '"' + ","
                        //+ '"' + rec.FlightID.ToString() + '"' + ","
                        + '"' + rec.longitude.ToString() + '"' + ","
                        + '"' + rec.latitude.ToString() + '"' + ","
                        //+ '"' + rec.dest_order_id.ToString() + '"' + ","
                        + '"' + rec.AirportCode.ToString() + '"' + ","
                        + '"' + rec.flightweight.ToString() + '"' + ","
                        + Environment.NewLine;
                }
            }
            catch (Exception e)
            {
                int? r = routeID;
                csvVisualPilotLogBook = csvVisualPilotLogBook + Environment.NewLine + "LogBook Destination generation failed on RouteID = " + r + Environment.NewLine + "Some fields are null";
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
                    return File(outputStream.ToArray(), "application/zip", "VisualLogBook.twbx");
                }
            };

        }

        [HttpGet]
        [AllowAnonymous]
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
            var minLogBookDate = System.Convert.ToDateTime(logBookList.Min(item => item.FlightDateOnly));
            //List<vPilotLogBook> logBookRecords = q_flightsLogBook.ToList();

            var vmpilotlogbook = new vmPilotLogBook(logBookList, q.pilotId, minLogBookDate, timeForward, landNumForward);

            return View(vmpilotlogbook);
        }
        [HttpPost]
        [AllowAnonymous]
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