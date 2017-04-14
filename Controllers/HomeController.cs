using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Acft_Track.Models;
using MVC_Acft_Track.ViewModels;
//using System.Diagnostics;
using System.Web.Security;
using MVC_Acft_Track.Helpers;
using static MVC_Acft_Track.Finals;
using Ionic.Zip;

namespace MVC_Acft_Track.Controllers
{
    public class HomeController : Controller
    {

        //public enum EPilotCertificates { Student_Pilot, Sport_Pilot, Recreational_Pilot, Private_Pilot, Commercial_Pilot, Airline_Transport_Pilot, Other };
        private qLINQ q = new qLINQ { db = new Entities() };
        private Entities db = new Entities();
        private int pilotid;
        private IEnumerable<vFlightAcftPilot> q_flightsByPilot;

        public HomeController()
        {
            q_flightsByPilot = db.vFlightAcftPilots.Where(row => row.PilotID == pilotid).Where(row => row.Points > 0);
        }

        public ActionResult Index()
        {
            //ViewBag.FeaturedMessage = "The aircraft tracking system records the GPS locations of your phone while on a flight ";
            ViewBag.AppTitle = APP_NAME;
            ViewBag.AppTitleModifier = APP_MODIFIER;
            //if ( Roles.IsUserInRole("admin")) {
            //}
            //var password = user.GetPassword();           
            if (Request.IsAuthenticated)
            {

                try
                {
                    ViewBag.Message = MSG_LOGGED;
                    //Trace.WriteLine("Point0");
                    var pilotUserName = User.Identity.Name;
                    var result = db.Pilots.Where(p => p.PilotUserName == pilotUserName).FirstOrDefault();
                    if (result == null) { return View("AuthError"); }
                    //pilotid = result.PilotID;
                    return RedirectToAction("IndexMember","Member", new { menuitem = 1});

                    //Trace.WriteLine(pilotid);
                }
                catch (Exception e)
                {
                    ViewBag.eMessage = e.Message;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.Message = MSG_LOGINHINT;
                //var dd = new ListsDD();
                //ViewBag.AircraftsSelList = dd.getAllAcftList();
                //ViewBag.PilotSelList = dd.getAllPilotList();
                //ViewBag.AirportSelList = dd.getAllAirportList();
                return View(Request.Browser.IsMobileDevice? "Index" : "Index");
                //return View(Request.Browser.IsMobileDevice ? "IndexMobile" : "Index");
            }
        }
        //POST: 
        [HttpPost]
        public ActionResult Index(string searchString = "")
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                if (db.DimAircraftRemotes.Where(row => row.AcftNum.Equals(searchString)).Count() == 0) { ViewBag.NotFoundMessage = "No aircraft with this number '" + searchString + "' found"; return View(); }
                int AcftID = db.DimAircraftRemotes.Where(row => row.AcftNum.Equals(searchString)).First().AcftID;
                return RedirectToAction("AircraftSearchGrid", "Flight", new { id = AcftID });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Help()
        {
            ViewBag.Message = "Help";
            return View();
        }

        public ActionResult HelpBeTester()
        {
            ViewBag.Message = "Alpha testing";
            return View();
        }
        public ActionResult HelpTracking()
        {
            ViewBag.Message = "Using FlightOnTrack Android appllication";
            return View();
        }
        public ActionResult HelpWebSite()
        {
            ViewBag.Message = "FAQ";
            return View();
        }
        public ActionResult HelpiPhoneUser()
        {
            ViewBag.Message = "iPhone, Windows Phone users";
            return View();
        }
        public ActionResult HelpTableau()
        {
            ViewBag.Message = "HelpTableau";
            return View();
        }
        public ActionResult ScreenshotVideo()
        {
            ViewBag.Message = "Screenshots and Videos";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us";
            return View();
        }
        //private void SetViewBagCertType(EPilotCertificates pilotCertificates)
        //{
        //    IEnumerable<EPilotCertificates> values = Enum.GetValues(typeof(EPilotCertificates)).Cast<EPilotCertificates>();
        //    IEnumerable<SelectListItem> items =

        //        from value in values
        //        select new SelectListItem
        //        {
        //            Text = value.ToString().Replace("_"," "),
        //            Value = value.ToString(),
        //            Selected = value == pilotCertificates,
        //        };

        //    ViewBag.Certif = items;

        //}
        // GET:
        public ActionResult PilotEdit(int id = 0)
        {
            Pilot pilot = db.Pilots.Find(id);

            //EPilotCertificates enumIndexSelected;
            //Enum.TryParse(pilot.CertType, out enumIndexSelected);
            //List<SelectListItem> items = new List<SelectListItem>();

            //items.Add(new SelectListItem { Text = "Student", Value = "0" });
            //items.Add(new SelectListItem { Text = "Sport Pilot", Value = "1" });
            //items.Add(new SelectListItem { Text = "Recreational Pilot", Value = "2" });
            //items.Add(new SelectListItem { Text = "Private Pilot", Value = "3", Selected = true });
            //items.Add(new SelectListItem { Text = "Commercial Pilot", Value = "4" });
            //items.Add(new SelectListItem { Text = "Flight Instructor", Value = "5", Selected = true });
            //items.Add(new SelectListItem { Text = "Airline Transport Pilot", Value = "6" });
            //ViewBag.Cert = items;

            //SetViewBagCertType(enumIndexSelected);
            ViewBag.Certif = ListsDD.getCertificates(pilot.CertType);
            if (pilot == null)
            {
                return HttpNotFound();
            }
            ViewBag.Pilotphone = pilot.PilotCode;
            ViewBag.PilotNickname = pilot.PilotName;
            ViewBag.Certif = ListsDD.getCertificates(pilot.CertType);
            return View(pilot);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PilotEdit(Pilot pilot, string Certif)
        {
            if (ModelState.IsValid)
            {
                pilot.CertType = Certif.Replace("_", " ");
                db.Pilots.Attach(pilot);
                db.Entry(pilot).Property(p => p.NameLast).IsModified = true;
                db.Entry(pilot).Property(p => p.NameFirst).IsModified = true;
                db.Entry(pilot).Property(p => p.BaseAirport).IsModified = true;
                db.Entry(pilot).Property(p => p.CertType).IsModified = true;
                //db.Entry(pilot).Property(p => p.PilotName).IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pilot);
        }

        public ActionResult IndexLogBook()
        {
            //ViewBag.FeaturedMessage = "The aircraft tracking system records the GPS locations of your phone while on a flight ";
            ViewBag.AppTitle = APP_NAME;
            ViewBag.AppTitleModifier = APP_MODIFIER;
            //if ( Roles.IsUserInRole("admin")) {
            //}
            //var password = user.GetPassword();           
            if (Request.IsAuthenticated)
            {
                try
                {
                    ViewBag.Message = MSG_LOGGED;
                    //Trace.WriteLine("Point0");
                    var pilotUserName = User.Identity.Name;
                    var result = db.Pilots.Where(p => p.PilotUserName == pilotUserName).FirstOrDefault();
                    if (result == null) { return View("AuthError"); }
                    pilotid = result.PilotID;
                    //Trace.WriteLine(pilotid);
                    ViewBag.PilotCode = db.Pilots.Where(p => p.PilotID == pilotid).FirstOrDefault().PilotCode;
                    ViewBag.NameLast = db.Pilots.Where(p => p.PilotID == pilotid).FirstOrDefault().NameLast;
                    ViewBag.NameFirst = db.Pilots.Where(p => p.PilotID == pilotid).FirstOrDefault().NameFirst;
                    string c = db.Pilots.Where(p => p.PilotID == pilotid).FirstOrDefault().CertType;
                    ViewBag.PilotCert = c == null ? c : c.Replace('_', ' ');
                    //Trace.WriteLine("Point10");
                    ViewBag.Raiting = "";
                    ViewBag.BaseAirport = db.Pilots.Where(p => p.PilotID == pilotid).FirstOrDefault().BaseAirport;
                    ViewBag.PilotName = db.Pilots.Where(p => p.PilotID == pilotid).FirstOrDefault().PilotName;
                    ViewBag.PilotID = pilotid;
                    ViewBag.PilotFlightNum = q_flightsByPilot.Count();
                    //db.Flights.Where(p => p.PilotID == pilotid).Count();
                    //var aircraftids = db.Flights.Where(p => p.PilotID == pilotid).Select(p => p.AcftID).Distinct().ToList();
                    //var aircrafts = db.vAircraftPilots.Where(a => aircraftids.Contains(a.AcftID)).ToList();
                    var aircrafts = db.vAircraftPilots.Where(p => p.PilotID == pilotid).ToList();
                    return View("Index_full", aircrafts);
                }
                catch (Exception e)
                {
                    ViewBag.eMessage = e.Message;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.Message = MSG_NOTLOGGED;
                var dd = new ListsDD();
                ViewBag.AircraftsSelList = dd.getAllAcftList();
                ViewBag.PilotSelList = dd.getAllPilotList();
                ViewBag.AirportSelList = dd.getAllAirportList();
                return View();
            }
        }

        public ActionResult index_member_menu(int menuitem, string msg = null, int pilotid = 0, bool? buttonEnable = null)
        {
            if (Request.IsAuthenticated)
            {
                q.pilotId = pilotid;
                var p = q.pilotEntity;
                ViewBag.Msg = msg;
                switch (menuitem)
                {
                    case 1:
                        return View("Index_member_pilot", p);
                    case 2:
                        return View("Index_member_acft", db.vAircraftPilots.Where(r => r.PilotID == pilotid).ToList());
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

                        ViewBag.VLBReadyToDownload = (buttonEnable==null?false:true);

                        //var vmpilotlogbook = new vmPilotLogBook(logBookList, pilotid, timeForward, landNumForward);
                        return View("Index_member_logbook", new vmPilotLogBook(logBookList, pilotid, timeForward, landNumForward));
                    case 4:
                        ViewBag.PilotFlightNum = q_flightsByPilot.Count();
                        return View("Index_member_flights", p);

                    default:
                        return View("Index_member_pilot", db.Pilots.Find(pilotid));
                }
                    //break;
            }
            else return RedirectToAction("Login", "Account");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult index_member_menu(FormCollection form, Pilot pilot = null)
        //{
        //    if (Request.IsAuthenticated)
        //    {
        //        int i = Int32.Parse(form["menuitem"]);
        //        var pids = form["pilotId"];
        //        int pid = form["pilotId"] == null ? 0 : Int32.Parse(form["pilotId"]);
        //        bool btnEnabl = false;
        //        //var aircrafts = db.vAircraftPilots.Where(p => p.PilotID == pilotid).ToList();
        //        string msg = null;
        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                switch (form["menuitem"])
        //                {
        //                    case "1":
        //                        db.Pilots.Attach(pilot);
        //                        db.Entry(pilot).Property(p => p.NameLast).IsModified = true;
        //                        db.Entry(pilot).Property(p => p.NameFirst).IsModified = true;
        //                        db.Entry(pilot).Property(p => p.BaseAirport).IsModified = true;
        //                        db.Entry(pilot).Property(p => p.CertType).IsModified = true;
        //                        //db.Entry(pilot).Property(p => p.PilotName).IsModified = true;
        //                        db.SaveChanges();
        //                        msg = "The changes have been saved successfully.";
        //                        //returnview = "Index_member_pilot";
        //                        break;
        //                    case "2":
        //                        //returnview = "Index_member_acft";
        //                        break;
        //                    case "3":
        //                        if (!(form["UpdateForwards"] == null))
        //                        {
        //                            int number1, number2;
        //                            if (!(Int32.TryParse(form["timeForward"], out number1) && number1 >= 0)) return RedirectToAction("index_member_menu", new { menuitem = 3, pilotId = pid, buttonEnable = btnEnabl });
        //                            if (!(Int32.TryParse(form["landForward"], out number2) && number2 >= 0)) return RedirectToAction("index_member_menu", new { menuitem = 3, pilotId = pid, buttonEnable = btnEnabl });
        //                            if (ModelState.IsValid)
        //                            {
        //                                Pilot p = db.Pilots.Find(pid);
        //                                p.TimeForward = number1;
        //                                p.LandingsForward = number2;
        //                                db.Pilots.Attach(p);
        //                                db.Entry(p).Property(r => r.TimeForward).IsModified = true;
        //                                db.Entry(p).Property(r => r.LandingsForward).IsModified = true;
        //                                db.SaveChanges();
        //                                msg = "The changes have been saved successfully.";
        //                            }
        //                        }
        //                        else if (!(form["DownloadFlightCSV"] == null))
        //                        {
        //                            return DownloadLogBookCSV(pid);
        //                        }
        //                        else if (!(form["RequestVisualLogBook"] == null))
        //                        {
        //                            btnEnabl = true;
        //                        }
        //                        else if (!(form["DownloadVLB"] == null))
        //                        {
        //                            btnEnabl = false;
        //                            return GenerateVisualLogbook(pid);
        //                        }
        //                        //PilotLogBookUpdate(form, pid, ref btnEnabl);
        //                        break;
        //                    case "4":
        //                        //returnview = "Index_member_pilot";
        //                        break;
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                msg = "Failed to save changes.";
        //            }
        //        }
        //        //var m = db.Pilots.Where(p => p.PilotID == pilotid).FirstOrDefault();
        //        return RedirectToAction("index_member_menu", new { menuitem = i, msg = msg, pilotid = pid, buttonEnable = btnEnabl });
        //    }
        //    else return RedirectToAction("Login", "Account");
        //}
        //public ActionResult PilotLogBookUpdate(FormCollection form,int pid, ref bool btnEnabl)
        //{

        //    if (!(form["UpdateForwards"] == null))
        //        {
        //        int number1, number2;
        //        if (!(Int32.TryParse(form["timeForward"], out number1) && number1 >= 0)) return RedirectToAction("index_member_menu", new { menuitem = 3, pilotId = pid, buttonEnable = btnEnabl });
        //        if (!(Int32.TryParse(form["landForward"], out number2) && number2 >= 0)) return RedirectToAction("index_member_menu", new { menuitem = 3, pilotId = pid, buttonEnable = btnEnabl });
        //        if (ModelState.IsValid)
        //            {
        //                Pilot pilot = db.Pilots.Find(pid);
        //                pilot.TimeForward = number1;
        //                pilot.LandingsForward = number2;
        //                db.Pilots.Attach(pilot);
        //                db.Entry(pilot).Property(p => p.TimeForward).IsModified = true;
        //                db.Entry(pilot).Property(p => p.LandingsForward).IsModified = true;
        //                db.SaveChanges();
        //            }
        //        }
        //        else if (!(form["DownloadFlightCSV"] == null))
        //        {
        //            return DownloadLogBookCSV(pid);
        //        }
        //        else if (!(form["RequestVisualLogBook"] == null))
        //        {
        //            btnEnabl = true;
        //        }
        //        else if (!(form["DownloadVLB"] == null))
        //        {
        //            btnEnabl = false;
        //            return GenerateVisualLogbook(pid);
        //        }
        //    //return RedirectToAction("index_member_menu", new { menuitem = 3, pilotId = pid, buttonEnable = btnEnabl });
        //}

    }
}
