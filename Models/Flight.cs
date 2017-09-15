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
    
    public partial class Flight
    {
        public int FlightID { get; set; }
        public string FlightCode { get; set; }
        public string FlightName { get; set; }
        public Nullable<System.DateTime> FlightDate { get; set; }
        public System.DateTime CreatedUTC { get; set; }
        public Nullable<System.DateTime> FlightTimeStart { get; set; }
        public Nullable<int> FlightDurationMin { get; set; }
        public Nullable<int> AcftID { get; set; }
        public Nullable<int> PilotID { get; set; }
        public Nullable<int> AcftPilotID { get; set; }
        public Nullable<bool> IsPattern { get; set; }
        public Nullable<bool> IsShared { get; set; }
        public Nullable<int> Last_GPSLocationID { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<int> Points { get; set; }
        public Nullable<int> FlightStateID { get; set; }
        public Nullable<int> FlightControllerID { get; set; }
        public Nullable<bool> InProcess { get; set; }
        public Nullable<int> SpeedThreshhold { get; set; }
        public Nullable<int> FreqSec { get; set; }
        public Nullable<int> RouteID { get; set; }
        public Nullable<int> AppVersionCode { get; set; }
        public Nullable<short> IsNameUpdated { get; set; }
        public Nullable<int> AreaID { get; set; }
        public Nullable<bool> IsJunk { get; set; }
        public Nullable<bool> IsAltitudeChecked { get; set; }
        public Nullable<int> FlightStart_GPSLocationID { get; set; }
        public Nullable<bool> IsChecked { get; set; }
    }
}
