using MVC_Acft_Track.ListsNS;
using MVC_Acft_Track.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Acft_Track.Controllers
{
    public class PublicFlightsController : Controller
    {
        private Entities db = new Entities();
        [HttpGet]
        public ActionResult IndexFlightSearch()
        {
//            var dd = new ListsDD();

            ViewBag.AircraftsSelList = new SelectList(db.vListAircrafts, "AcftID", "AcftRegNum");
            ViewBag.PilotSelList = new SelectList(db.vListPilots, "PilotID","PilotCode");
            ViewBag.AirportSelList = new SelectList(db.vListAirports, "AirportID", "AirportCode");

            return View();
        }
        ////POST: 
        //[HttpPost]
        //public ActionResult IndexFlightSearch()
        //{
        //    //            var dd = new ListsDD();

        //    ViewBag.AircraftsSelList = new SelectList(db.vListAircrafts, "AcftID", "AcftRegNum");
        //    ViewBag.PilotSelList = new SelectList(db.vListPilots, "PilotID", "PilotCode");
        //    ViewBag.AirportSelList = new SelectList(db.vListAirports, "AirportID", "AirportCode");

        //    return View();
        //}
    }
}
