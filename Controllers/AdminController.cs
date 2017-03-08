using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MVC_Acft_Track.Models;
using MVC_Acft_Track.Helpers;
using FontBing;
using WebMatrix.WebData;
using static MVC_Acft_Track.App;

/// 9784934810.0139
/// 82$befc
/// 73974

namespace MVC_Acft_Track.Controllers
{
    public class AdminController : Controller
    {

        private Entities db = new Entities();
        private qLINQ q = new qLINQ();

        // GET: Admin
        public ActionResult Index(int? pilotid = null,string error = null)
        {
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
        }
        
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            if (Request.IsAuthenticated)
            {
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
                            return View("ErrorPage");

                        }
                    }
                    return RedirectToAction("Index", new { pilotid = pilotId });
                }
                return RedirectToAction("Index");
            }
            else return RedirectToAction("Login", "Account");
        }

        public ActionResult MarkJunkRecords()
        {
            var flights = q.flightsJunkNotChecked.ToList();
            var junkFlightIds = new List<int>();
            var junkFlights = new List<Flight>();
            foreach(var f in flights){
                
                q.flightId = f.FlightID;
                var locations = q.flightGpsLocations.ToList();
                if (f.Points <= 1 || locations.Count() == 0) { 
                    junkFlights.Add(f);
                    junkFlightIds.Add(f.FlightID);
                    continue; 
                }
                var highestAltitude = locations.Max(row => row.AltitudeM);
                var locationHighest = locations.Where(l => l.AltitudeM == highestAltitude).Select(l => new { l.latitude,l.longitude}).First();
                string queryString = locationHighest.latitude.ToString() + "," + locationHighest.longitude.ToString();

                BingElevadion.querystring = queryString;
                var elevation = BingElevadion.groundElevation;
                if ((highestAltitude - elevation)<50){
                //mark flight as junk
                    junkFlights.Add(f);
                    junkFlightIds.Add(f.FlightID); 
                }

            }   
            foreach(var f in flights){
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
            return View(junkFlights);
        }

        public ActionResult GetAllFligts() {
            if (Request.IsAuthenticated)
            {
                try
                {
                    ViewBag.ViewTitle = "All Recent Flights";
                    ViewBag.ActionBack = "GetAllFligts";
                    q.topNumber = 50;
                    var fs = q.flightsAll.ToList();
                    return View(fs);
                }
                catch (Exception e)
                {
                    ViewBag.ExceptionErrorMessage = isDebugMode ? e.Message : "GetAllFligts() error";
                    return View("ErrorPage");
                }
            }
            else return RedirectToAction("Login", "Account"); ;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetAllFligts(FormCollection form)
        {
            if (Request.IsAuthenticated)
            {
                if (Request["submit"] == "Update page")
                {
                    return RedirectToAction("GetAllFligts");
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
                    return RedirectToAction("GetAllFligts");
                }
                if (Request["submit"] == "Save changes")
                {
                    DateTime timeUtcNow = DateTime.UtcNow;
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
                                    flight.Updated = timeUtcNow;
                                    db.Flights.Attach(flight);
                                    db.Entry(flight).Property(p => p.IsShared).IsModified = true;
                                    db.Entry(flight).Property(p => p.Updated).IsModified = true;
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
                                int val = Int32.Parse(form.GetValues(id)[0]);
                                if (val != (flight.RouteID.HasValue ? flight.RouteID : flight.FlightID))
                                {

                                    flight.RouteID = val;
                                    flight.Updated = timeUtcNow;
                                    db.Flights.Attach(flight);
                                    db.Entry(flight).Property(p => p.RouteID).IsModified = true;
                                    db.Entry(flight).Property(p => p.Updated).IsModified = true;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            return RedirectToAction("GetAllFligts");
            }
            else return RedirectToAction("Login", "Account");
        }
        public ActionResult Edit(int id = 0)
        {
            Flight flight = db.Flights.Find(id);

            if (flight == null)
            {
                return HttpNotFound();
            }
            int? acftID = flight.AcftID;
            int? selectedPilotID = flight.PilotID;
            int? selectedID = flight.AcftPilotID;

            ViewBag.AircraftsSelList = new SelectList(q.selList_vAircraftPilot, "ID", "AcftNumLocal", selectedID);
            ViewBag.PhoneSelList = new SelectList(q.selList_Pilot.ToList(), "PilotID", "PilotUserName", selectedPilotID);
            ViewBag.selectedPilotID = selectedPilotID;
            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Flight flight, string AircraftsSelList,string PhoneSelList)
        {
            if (ModelState.IsValid)
            {
                db.Flights.Attach(flight);

                DateTime timeUtcNow = DateTime.UtcNow;
                flight.Updated = timeUtcNow;

                int phoneID;
                int.TryParse(PhoneSelList, out phoneID);
                if (phoneID > 0) {
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

                db.SaveChanges();
                return RedirectToAction("GetAllFligts");
            }
            return View(flight);
        }

    }

}