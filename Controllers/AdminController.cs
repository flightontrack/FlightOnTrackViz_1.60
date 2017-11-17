using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FontBing;
using WebMatrix.WebData;
using FontNameSpace.Models;
using FontNameSpace.Helpers;
using static FontNameSpace.App;
using static FontNameSpace.Finals;
using System.Threading.Tasks;
using System.Collections;
using FontNameSpace.ViewModels;
using System.Web.Helpers;

/// 9784934810.0139
/// 82$befc
/// 79$bec3

namespace FontNameSpace.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        private Entities db = new Entities();
        private Queryables q = new Queryables();

        // GET: Admin
        public ActionResult Index(int? pilotid = null,string error = null)
        {
            //if (Request.IsAuthenticated)
            //{
                ViewBag.PilotSelList = new SelectList(db.Pilots.OrderBy(row => row.PilotUserName), "PilotID", "PilotUserName");
                ViewBag.ErrorMsg = error;

                if (pilotid != null)
                {
                    ViewBag.User = db.Pilots.Where(r => r.PilotID == pilotid).FirstOrDefault().PilotUserName;
                    var user1 = db.Pilots.Where(r => r.PilotID == pilotid).FirstOrDefault().PilotCode;
                    var user2 = db.Pilots.Where(r => r.PilotID == pilotid).FirstOrDefault().SimNumber;
                    var spr = db.get_PilotP(user1, user2).ToList();
                    ViewBag.UserPsw = spr[0];
                }
                return View();
            //}
            //else return RedirectToAction("Login", "Account");
        }
        
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            //if (Request.IsAuthenticated)
            //{
                if (!String.IsNullOrEmpty(form["PilotID"]))
                {
                    var pilotId = Int32.Parse(form["PilotID"]);

                    if (Request["submit"] == "Reset User Psw")
                    {
                        db.update_PilotGuid(pilotId);
                        var user = db.Pilots.Where(r => r.PilotID == pilotId).FirstOrDefault().PilotUserName;
                        try
                        {
                            var pswtoken = WebSecurity.GeneratePasswordResetToken(user);

                            var user1 = db.Pilots.Where(r => r.PilotID == pilotId).FirstOrDefault().PilotCode;
                            var user2 = db.Pilots.Where(r => r.PilotID == pilotId).FirstOrDefault().SimNumber;
                            var spr = db.get_PilotP(user1, user2).ToList();
                            var psw = spr[0];
                            WebSecurity.ResetPassword(pswtoken, psw);
                        }
                        catch (Exception e)
                        {
                            ViewBag.ExceptionErrorMessage = isDebugMode ? e.Message : "Error in Index method";
                            return View("ExceptionPage");
                        }
                    }
                    return RedirectToAction("Index", new { pilotid = pilotId });
                }
                return RedirectToAction("Index");
            //}
            //else return RedirectToAction("Login", "Account");
        }

        public async Task<ActionResult> GetJunkRecordsAsync()
        {
            //Console.WriteLine("before");
            List<Flight> junkFlights = await MarkJunkRecords();
            //Console.WriteLine("after");
            return View(junkFlights);
        }

        async Task<List<Flight>> MarkJunkRecords()
        {
            var flights = q.flightsJunkNotChecked.ToList();
            var junkFlightIds = new List<int>();
            var junkFlights = new List<Flight>();
            foreach (var f in flights)
            {

                q.flightId = f.FlightID;
                var locations = q.flightGpsLocations.ToList();
                if (f.Points <= 1 || locations.Count() == 0)
                {
                    junkFlights.Add(f);
                    junkFlightIds.Add(f.FlightID);
                    continue;
                }
                var highestAltitude = locations.Max(row => row.AltitudeM);
                var locationHighest = locations.Where(l => l.AltitudeM == highestAltitude).Select(l => new { l.latitude, l.longitude }).First();
                string queryString = locationHighest.latitude.ToString() + "," + locationHighest.longitude.ToString();

                BingElevadion.querystring = queryString;
                var elevation = BingElevadion.groundElevation;
                if ((highestAltitude - elevation) < 50)
                {
                    //mark flight as junk
                    junkFlights.Add(f);
                    junkFlightIds.Add(f.FlightID);
                }

            }
            foreach (var f in flights)
            {
                if (ModelState.IsValid)
                {
                    f.IsJunk = true;
                    f.IsAltitudeChecked = true;
                    db.Flights.Attach(f);
                    db.Entry(f).Property(p => p.IsJunk).IsModified = junkFlightIds.Contains(f.FlightID);
                    db.Entry(f).Property(p => p.IsAltitudeChecked).IsModified = true;
                    db.SaveChanges();
                }
            }

            return junkFlights;
        }

        public ActionResult GetFlights(vmSearchRequest searchRequest, string sort = "", string sortdir = "") {
            //if (Request.IsAuthenticated)
            //{
                try
                {
                    var fs = new vmSearchRequestFights(searchRequest, 50,10000, sort, sortdir.Equals("ASC") ? SortDirection.Ascending : SortDirection.Descending);
                    return View(fs);
                }
                catch (Exception e)
                {
                    ViewBag.ExceptionErrorMessage = isDebugMode ? e.Message : "GetFlights() error";
                    return View("ExceptionPage");
                }
            //}
            //else return RedirectToAction("Login", "Account"); ;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetFlights(FormCollection form, vmSearchRequest searchRequest, List<vFlightAcftPilot> flightList)
        {
            //if (Request.IsAuthenticated) { 
                if (ModelState.IsValid)
                {
                    //var dict = searchRequest.Replace("{", string.Empty).Replace("}", string.Empty).Replace(" ", string.Empty).Split(',').Select(r => r.Split('=')).ToDictionary(r => r[0], r => r[1]);
                    var buttonClicked = Request["buttonClicked"];
                    ClassShared.FormHandle(buttonClicked, flightList);
                    searchRequest.isSearchJunk = (Request["buttonClicked"] == "SelectAllGarbage");
                    return RedirectToAction("GetFlights", searchRequest);
                }
                else {
                    var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors }).ToString();
                    LogHelper.onFailureLog("CreateUserPilot()", errors);
                    ViewBag.ExceptionErrorMessage = isDebugMode ? errors : "POST GetFlights() error";
                    return View("ExceptionPage");
                }
            //}
            //else return RedirectToAction("Login", "Account");
        }

        public ActionResult FlightEditAdm(int id = 0, bool? successFlg = null)
        {
            if (successFlg.HasValue) ViewBag.Msg = ((bool)successFlg ? MSG_SAVESUCCESS : MSG_SAVEFAIL);
            ViewBag.successFlg = successFlg;

            Flight flight = db.Flights.Find(id);

            if (flight == null)
            {
                return HttpNotFound();
            }
            int? acftID = flight.AcftID;
            int? selectedPilotID = flight.PilotID;
            int? selectedID = flight.AcftPilotID;
            q.pilotId = flight.PilotID??DEFAULT_PILOTID;

            ViewBag.AircraftsSelList = new SelectList(q.selList_vAircraftPilot.ToList(), "ID", "AcftNumLocal", selectedID);
            ViewBag.PhoneSelList = new SelectList(q.selList_Pilot.ToList(), "PilotID", "PilotUserName", selectedPilotID);
            ViewBag.FlightSelList = new SelectList(q.selList_FlightsByPilot.ToList(), "FlightID", "FlightID", flight.RouteID == null ? flight.FlightID : flight.RouteID);

            ViewBag.selectedPilotID = selectedPilotID;
            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlightEditAdm(Flight flight, string AircraftsSelList,string PhoneSelList, int FlightSelList)
        {
            //bool? successFlg;
            //if (Request.IsAuthenticated)
            //{
                if (ModelState.IsValid)
                {
                    try
                    {

                        db.Flights.Attach(flight);
                        DateTime timeUtcNow = DateTime.UtcNow;
                        flight.Updated = timeUtcNow;
                        flight.RouteID = FlightSelList;
                        int phoneID;
                        int.TryParse(PhoneSelList, out phoneID);
                        if (phoneID > 0)
                        {
                            flight.PilotID = phoneID;
                            db.Entry(flight).Property(f => f.PilotID).IsModified = true;
                        }

                        int acftPilotID;
                        int.TryParse(AircraftsSelList, out acftPilotID);
                        if (acftPilotID > 0)
                        {
                            flight.AcftPilotID = acftPilotID;
                            flight.AcftID = db.AircraftPilots.Find(acftPilotID).AcftID;
                            db.Entry(flight).Property(f => f.AcftID).IsModified = true;
                            db.Entry(flight).Property(f => f.AcftPilotID).IsModified = true;
                        }

                        db.Entry(flight).Property(f => f.FlightName).IsModified = true;
                        db.Entry(flight).Property(f => f.Comments).IsModified = true;
                        db.Entry(flight).Property(p => p.Updated).IsModified = true;
                        db.Entry(flight).Property(f => f.RouteID).IsModified = true;

                        db.SaveChanges();
                        //successFlg = true;
                        return RedirectToAction("FlightEditAdm",  new { id = flight.FlightID,successFlg = true });
                    }
                    catch (Exception e)
                    {
                        //successFlg = false;
                        LogHelper.onFailureLog("FlightEditAdm()", e);
                        return RedirectToAction("FlightEditAdm", new { id = flight.FlightID, successFlg = false });
                    }
                }
                return View(flight);
            //}
            //else return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult SearchByCriteria(string message = "")
        {
            ViewBag.AircraftsSelList = new SelectList(db.vListAircrafts.OrderBy(row => row.AcftNumLocal), "AcftID", "AcftNumLocal");
            ViewBag.PilotSelList = new SelectList(db.vListPilots.OrderBy(row => row.PilotCode), "PilotID", "PilotCode");
            ViewBag.AirportSelList = new SelectList(db.vListAirports.OrderBy(row => row.AirportCode), "AirportID", "AirportCode");
            ViewBag.GroupSelList = new SelectList(db.DimAcftGroups.OrderBy(row => row.GroupName), "GroupID", "GroupName");
            ViewBag.Message = message;
            ViewBag.PublicSearch = false;
            return View("SearchByCriteria");
        }
        [HttpPost]
        public ActionResult SearchByCriteria(vmSearchRequest vmsearchRequest,FormCollection form)
        {
            if (form["submit"] == "Search")
            {
                if (form["FlightID"].Equals("") && form["AcftNumLocal"].Equals("") && form["PilotID"].Equals("") && form["FlightDate"].Equals("") && form["GroupID"].Equals("")) return RedirectToAction("SearchByCriteria", new { message = SELECTSOMTHING });
                return RedirectToAction("GetFlights", vmsearchRequest);
            }
            return View();
        }

    }

}