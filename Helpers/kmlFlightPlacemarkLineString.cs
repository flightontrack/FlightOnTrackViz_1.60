using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using FontNameSpace.Models;

namespace kmlFlightPlacemarkLineString
{
    class FlightPlacemarkLineString : IDisposable
    {
        private int _flightId;
        private Placemark _placemark;
        LineString _line;
        Timestamp _timestamp;
        private Entities db = new Entities();

        public string styleUrlRef
        {
            get { return _placemark.StyleUrl.ToString(); }
            set { _placemark.StyleUrl = new Uri("#" + value, UriKind.Relative); }
        }

        public Placemark placemark
        {
            get { return _placemark; }
            set { _placemark = value; }
        }

        public FlightPlacemarkLineString(int flightId)
        {
            _line = new LineString();
            _timestamp = new Timestamp();
            _line.AltitudeMode = AltitudeMode.Absolute;
            _line.Extrude = false;
            _line.Tessellate = false;
            _line.Coordinates = new CoordinateCollection();
            _flightId = flightId;
            var gpsloc = db.GpsLocations.Where(row => row.FlightID == _flightId).OrderBy(row => row.onSessionPointNum).Select(row => new { row.latitude, row.longitude, row.AltitudeM }).ToList();
            foreach (var loc in gpsloc)
            {
                _line.Coordinates.Add(new Vector((double)loc.latitude, (double)loc.longitude, (double)loc.AltitudeM));
            }
            _placemark = new Placemark();
            _placemark.Geometry = _line;
            _placemark.Time = _timestamp;
            _placemark.Id = _flightId.ToString();
            //_placemark.Name = "Flight " + _flightId;
        }

        public void Dispose()
        {
            ((IDisposable)db).Dispose();
        }
    }
}
