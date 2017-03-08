using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MVC_Acft_Track.Models;

namespace MVC_Acft_Track.ViewModels
{
    public class vmPilotLogBook
    {
        public int pilotId;
        public List<vPilotLogBook> flightsLogBook { get; set; }
        [Range(0,99999)] /// the validation is not working for some reason
        public int timeForward  { get; set; }
        [Range(0, 99999)]
        public int landForward  { get; set; }

        public vmPilotLogBook(List<vPilotLogBook> lb,int pid,int tf=0,int lf=0){
            timeForward=tf;
            landForward=lf;
            flightsLogBook=lb;
            pilotId = pid;
        }
    }

    public class vmPilotLogBookCombined
    {
        public int pilotId;
        public List<vPilotLogBook> flightsLogBook { get; set; }
        [Range(0, 99999)] /// the validation is not working for some reason
        public int timeForward { get; set; }
        [Range(0, 99999)]
        public int landForward { get; set; }

        public List<vAircraftPilot> aircrafts { get; set; }

        public vmPilotLogBookCombined(List<vPilotLogBook> lb, List<vAircraftPilot> a, int pid, int tf = 0, int lf = 0)
        {
            timeForward = tf;
            landForward = lf;
            flightsLogBook = lb;
            pilotId = pid;
            aircrafts = a;
        }
    }
}