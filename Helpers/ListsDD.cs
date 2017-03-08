using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;
using MVC_Acft_Track.Models;

namespace MVC_Acft_Track.Helpers
{
    public class ListsDD
    {
        private enum EPilotCertificates { Student_Pilot, Sport_Pilot, Recreational_Pilot, Private_Pilot, Commercial_Pilot, Airline_Transport_Pilot, Other };
        private Entities db = new Entities();
        public IEnumerable<SelectListItem> getAllAcftList()
        {
            var values = db.vListAircrafts.ToList();
            IEnumerable<SelectListItem> listItems =
                                            from value in values
                                            select new SelectListItem
                                            {
                                                Text = value.AcftNumLocal,
                                                Value = value.AcftID.ToString(),
                                                Selected = false
                                            };
            return listItems;
        }
        public IEnumerable<SelectListItem> getAllPilotList()
        {
            var values = db.vListPilots.ToList();
            IEnumerable<SelectListItem> listItems =
                                            from value in values
                                            select new SelectListItem
                                            {
                                                Text = value.PilotCode,
                                                Value = value.PilotID.ToString(),
                                                Selected = false
                                            };
            return listItems;
        }
        public IEnumerable<SelectListItem> getAllAirportList()
        {
            //var values = db.GpsLocations.Where(r => r.AirportCode != null).Select(r => new {r.AirportCode }).Distinct().ToList();
            var values =  db.vListAirports.ToList();

            IEnumerable<SelectListItem> listItems =
                                            from value in values
                                            select new SelectListItem
                                            {
                                                Text = value.AirportCode,
                                                Value = value.AirportID.ToString(),
                                                Selected = false
                                            };
            return listItems;
        }

        public IEnumerable<SelectListItem> getAllAcftListL()
        {
            var values = db.vListAircrafts.ToList();
            IEnumerable<SelectListItem> listItems =
                                            from value in values
                                            select new SelectListItem
                                            {
                                                Text = value.AcftNumLocal,
                                                Value = value.AcftID.ToString(),
                                                Selected = false
                                            };
            return listItems;
        }
        public static IEnumerable<SelectListItem> getCertificates(string selectedCert)
        {
            IEnumerable<EPilotCertificates> values = Enum.GetValues(typeof(EPilotCertificates)).Cast<EPilotCertificates>();

            IEnumerable<SelectListItem> listItems =
                from value in values
                select new SelectListItem
                {
                    Text = value.ToString().Replace("_", " "),
                    Value = value.ToString(),
                    Selected = value.ToString().Replace("_", " ").Equals(selectedCert),
                };
            return listItems;
            //ViewBag.Certif = items;

        }

    }
}