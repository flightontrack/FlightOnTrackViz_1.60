using System;
using System.ComponentModel.DataAnnotations;

namespace FontNameSpace.Models
{
    public partial class SearchRequestMetadata
    {

        [Display(Name = "Flight Date")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> FlightDate;
        [Display(Name = "Airport Code")]
        public string AirportCode;
        [Display(Name = "Aircraft Reg Number")]
        public string AcftRegNum;
        [Display(Name = "Pilot Callsign")]
        public string PilotCode;
        [Display(Name = "Flight ID")]
        public Nullable<int> FlightID;
        [Display(Name = "Area")]
        public Nullable<int> AreaID;
        [Display(Name = "Aircraft Number")]
        public string AcftNumLocal;
        public Nullable<int> GroupID { get; set; }

        [Display(Name = "Company Name")]
        public string GroupName { get; set; }
    }
}
