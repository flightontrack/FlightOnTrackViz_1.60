using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using  System.Collections;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using FontNameSpace.Models;

namespace kmlFlightPlacemarkPoint
{
 
    class FlightPlacemarkPoint : IDisposable
    {
        private int _flightId;
        private System.Uri _styleUrl;
        private Placemark _placemark;
        private Point _point;
        private List<Placemark> _pointplacemarks = new List<Placemark>();
        private Timestamp _timestamp;
        private Entities db = new Entities();

        public string styleUrlRef
        {
            //get { return _styleUrl; }
            set { _styleUrl = new Uri("#" + value, UriKind.Relative); }
        }

        public List<Placemark> placemarks
        {
            get { return _pointplacemarks; }
            //set { _pointplacemarks = value; }
        }

        public void getFlightPlacemarkPoints(int flightId)
        {
            _flightId = flightId;
            var gpsloc = db.GpsLocations.Where(row => row.FlightID == _flightId).OrderBy(row => row.onSessionPointNum).Select(row => new { row.latitude, row.longitude, row.AltitudeM,row.onSessionPointNum,row.gpsTime, row.AltitudeFt }).ToList();

            foreach (var loc in gpsloc)
            {
                _placemark = new Placemark();
                _point = new Point();
                _timestamp = new Timestamp();
                var _coord = new Vector((double)loc.latitude, (double)loc.longitude, (double)loc.AltitudeM);
                _point.Coordinate = _coord;
                _point.AltitudeMode = AltitudeMode.Absolute;
                _point.Extrude = true;
                //_point.Id = loc.onSessionPointNum.ToString();
                _timestamp.When=loc.gpsTime.GetValueOrDefault();
                _placemark.Id = loc.onSessionPointNum.ToString();
                _placemark.Geometry = _point;
                _placemark.Time = _timestamp;
                _placemark.Name = "f" + flightId + "  p" + loc.onSessionPointNum.ToString() + "  alt " + loc.AltitudeFt;
                _placemark.StyleUrl = _styleUrl; 
                _pointplacemarks.Add(_placemark);
            }
        }

        public void Dispose()
        {
            ((IDisposable)db).Dispose();
        }
    }
}
