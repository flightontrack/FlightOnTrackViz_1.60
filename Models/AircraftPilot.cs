//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FontNameSpace.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AircraftPilot
    {
        public int ID { get; set; }
        public int AcftID { get; set; }
        public string AcftName { get; set; }
        public int PilotID { get; set; }
        public System.DateTime Created { get; set; }
        public string AcftNumLocal { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string URLPictures { get; set; }
        public System.DateTime Updated { get; set; }
    }
}
