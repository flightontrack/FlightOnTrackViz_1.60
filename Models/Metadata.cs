using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_Acft_Track.Models
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
        public Nullable<int> AreaID;
        [Display(Name = "Aircraft Number")]
        public string AcftNumLocal;
    }
}
