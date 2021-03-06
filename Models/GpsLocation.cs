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
    
    public partial class GpsLocation
    {
        public int GPSLocationID { get; set; }
        public System.DateTime lastUpdate { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public Nullable<int> speed { get; set; }
        public Nullable<int> distance { get; set; }
        public Nullable<System.DateTime> gpsTime { get; set; }
        public string gpsTimeOnly { get; set; }
        public Nullable<int> accuracy { get; set; }
        public string eventType { get; set; }
        public string extraInfo { get; set; }
        public int onSessionPointNum { get; set; }
        public int FlightID { get; set; }
        public Nullable<int> SpeedMpC { get; set; }
        public Nullable<int> SpeedMph { get; set; }
        public Nullable<int> SpeedKnot { get; set; }
        public Nullable<int> SpeedKmpH { get; set; }
        public Nullable<decimal> AltitudeM { get; set; }
        public Nullable<int> DistanceToStartPoint { get; set; }
        public System.Data.Entity.Spatial.DbGeography GeoLocation { get; set; }
        public string AirportCode { get; set; }
        public Nullable<bool> IsGeocoded { get; set; }
        public Nullable<int> signalStrength { get; set; }
        public Nullable<int> AltitudeFt { get; set; }
        public Nullable<int> AreaID { get; set; }
    }
}
