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
    
    public partial class Pilot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pilot()
        {
            this.TimeForward = 0;
            this.LandingsForward = 0;
        }
    
        public int PilotID { get; set; }
        public string PilotCode { get; set; }
        public string PilotName { get; set; }
        public string PhoneIdType { get; set; }
        public string PhoneID { get; set; }
        public string SimNumber { get; set; }
        public string AnID { get; set; }
        public string PilotUserName { get; set; }
        public Nullable<int> StartingFlyWayPoint { get; set; }
        public bool IsShared { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string NameFirst { get; set; }
        public string NameLast { get; set; }
        public string BaseAirport { get; set; }
        public string SearchHint { get; set; }
        public string CertType { get; set; }
        public string PilotGuid { get; set; }
        public int TimeForward { get; set; }
        public int LandingsForward { get; set; }
        public Nullable<int> BaseAirportID { get; set; }
    }
}
