//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_Acft_Track.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class vFlightAcftPilot
    {
        public int FlightID { get; set; }
        public Nullable<int> RouteID { get; set; }
        public string FlightName { get; set; }
        public Nullable<System.DateTime> FlightDate { get; set; }
        public string FlightDateOnly { get; set; }
        public Nullable<System.DateTime> FlightTimeStart { get; set; }
        public Nullable<int> FlightDurationMin { get; set; }
        public Nullable<bool> IsShared { get; set; }
        public Nullable<int> Points { get; set; }
        public Nullable<bool> IsChecked { get; set; }
        public bool IsJunk { get; set; }
        public string Comments { get; set; }
        public string AcftRegNum { get; set; }
        public string Acft { get; set; }
        public Nullable<int> AcftID { get; set; }
        public Nullable<int> PilotID { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public string PilotCode { get; set; }
        public string AcftNumLocal { get; set; }
        public string AcftName { get; set; }
        public int isPositionCurrent { get; set; }
        public int isInFlight { get; set; }
        public Nullable<int> UpdateDelay { get; set; }
        public Nullable<int> AltitudeFt { get; set; }
        public string AcftMMS { get; set; }
        public string AcftNum { get; set; }
        public string Pilot { get; set; }
    }
}
