//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC_Acft_Track.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FlightView
    {
        public int FlightID { get; set; }
        public Nullable<System.DateTime> FlightDate { get; set; }
        public System.DateTime FlightDateUTC { get; set; }
        public Nullable<int> FlightDurationMin { get; set; }
        public Nullable<bool> IsShared { get; set; }
        public Nullable<int> Points { get; set; }
        public Nullable<bool> IsChecked { get; set; }
        public string Acft { get; set; }
        public Nullable<int> PilotID { get; set; }
        public string FlightName { get; set; }
        public string Comments { get; set; }
        public Nullable<int> RouteID { get; set; }
        public Nullable<System.DateTime> FlightTimeStart { get; set; }
        public Nullable<int> AcftID { get; set; }
    }
}
