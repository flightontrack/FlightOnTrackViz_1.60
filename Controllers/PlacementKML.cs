using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using  System.Collections;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using MVC_Acft_Track.Models;

namespace MVC_Acft_Track.Controllers
{
    class FlightLineStyles
    {
        private static int _instanceCount = 0;
        public ArrayList flightStylesColors = new ArrayList();
        private Style _style;
        public Style style
        {
            get { return _style; }
        }
        public FlightLineStyles(int i)
        {
            _instanceCount = i;
            flightStylesColors.Add(new YellowGreenStyle());
            flightStylesColors.Add(new RedBlueStyle());
            flightStylesColors.Add(new BlueBlueStyle());
            flightStylesColors.Add(new WhiteWhiteStyle());
            var lc = (FlightLineColors)flightStylesColors[i<3?i:3];
            //_instanceCount=_instanceCount == 1 ? 0 : 1;
            _style = new Style();
            _style.Line = new LineStyle();
            _style.Line.Width = 4;
            _style.Line.Color = new Color32(lc._lalpha, lc._lblue, lc._lgreen, lc._lred);
            _style.Polygon = new PolygonStyle();
            _style.Polygon.ColorMode = ColorMode.Random;
            _style.Polygon.Color = new Color32(lc._palpha, lc._pblue, lc._pgreen, lc._pred);
            _style.Id = lc._styleId;
        }
    }
    abstract class FlightLineColors
    {
        public string _styleId;
        public byte _lalpha, _lblue, _lgreen, _lred;
        public byte _palpha, _pblue, _pgreen, _pred;

    }
    class YellowGreenStyle : FlightLineColors
    {
        public YellowGreenStyle()
        {
            base._styleId = "yellowGreen";
            base._lalpha = 0xff;
            base._lblue = 0x05;
            base._lgreen = 0xbf;
            base._lred = 0xfd;
            base._palpha = 0x33;
            base._pblue = 0xff;
            base._pgreen = 0xff;
            base._pred = 0xff;
        }
    }
    class RedBlueStyle : FlightLineColors
    {
        public RedBlueStyle()
        {
            _styleId = "redBlue";
            _lalpha = 0xff;
            _lblue = 0;
            _lgreen = 0;
            _lred = 0xff;
            _palpha = 0x33;
            _pblue = 0xff;
            _pgreen = 0xff;
            _pred = 0xff;
        }
    }
    class BlueBlueStyle : FlightLineColors
    {
        public BlueBlueStyle()
        {
            _styleId = "BlueBlue";
            _lalpha = 0xff;
            _lblue = 0xff;
            _lgreen = 0xaa;
            _lred = 0x0;
            _palpha = 0x33;
            _pblue = 0xff;
            _pgreen = 0xff;
            _pred = 0xff;
        }
    }
    class WhiteWhiteStyle : FlightLineColors
    {
        public WhiteWhiteStyle()
        {
            _styleId = "WhiteWhite";
            _lalpha = 0xff;
            _lblue = 0xff;
            _lgreen = 0xff;
            _lred = 0xff;
            _palpha = 0x22;
            _pblue = 0xff;
            _pgreen = 0xff;
            _pred = 0xff;
        }
    }
    class FlightPlacemarkLineString
    {
        private int _flightId;
        private Placemark _placemark;
        LineString _line;
        Timestamp _timestamp;
        private Entities db = new Entities();

        public string styleUrlRef
        {
            get {return _placemark.StyleUrl.ToString(); }
            set { _placemark.StyleUrl = new Uri("#" + value, UriKind.Relative);}
        }

        public Placemark placemark
        {
            get {return _placemark;}
            set { _placemark = value; }
        }

        public FlightPlacemarkLineString(int flightId)
        {
            _line = new LineString();
            _timestamp = new Timestamp();
            _line.AltitudeMode = AltitudeMode.Absolute;
            _line.Extrude = true;
            _line.Tessellate = true;
            _line.Coordinates = new CoordinateCollection();
            _flightId = flightId;
            var gpsloc = db.GpsLocations.Where(row => row.FlightID == _flightId).OrderBy(row => row.onSessionPointNum).Select(row => new { row.latitude, row.longitude, row.AltitudeM }).ToList();
            foreach (var loc in gpsloc)
            {
                _line.Coordinates.Add(new Vector((double)loc.latitude, (double)loc.longitude, (double)loc.AltitudeM));
                //_timestamp.When.;
            }
            _placemark = new Placemark();
            _placemark.Geometry = _line;
            _placemark.Time = _timestamp;
            _placemark.Id = _flightId.ToString();
            _placemark.Name = "Flight " + _flightId;
        }
    }
    class FlightPlacemarkPoint
    {
        private int _flightId;
        private Placemark _placemark;
        private Point _point;
        private List<Placemark> _pointplacemarks = new List<Placemark>();
        private Timestamp _timestamp;
        private Entities db = new Entities();

        public string styleUrlRef
        {
            get { return _placemark.StyleUrl.ToString(); }
            set { _placemark.StyleUrl = new Uri("#" + value, UriKind.Relative); }
        }

        public List<Placemark> placemarks
        {
            get { return _pointplacemarks; }
            //set { _pointplacemarks = value; }
        }

        public FlightPlacemarkPoint(int flightId)
        {

            _flightId = flightId;
            var gpsloc = db.GpsLocations.Where(row => row.FlightID == _flightId).OrderBy(row => row.onSessionPointNum).Select(row => new { row.latitude, row.longitude, row.AltitudeM,row.onSessionPointNum,row.gpsTime }).ToList();

            foreach (var loc in gpsloc)
            {
                _placemark = new Placemark();
                _point = new Point();
                _timestamp = new Timestamp();
                var _coord = new Vector((double)loc.latitude, (double)loc.longitude, (double)loc.AltitudeM);
                _point.Coordinate = _coord;
                //_point.Coordinate.Altitude=(double)loc.AltitudeM;
                //_point.Coordinate.Latitude=(double)loc.latitude;
                //_point.Coordinate.Longitude = (double)loc.longitude;
                _point.Id = loc.onSessionPointNum.ToString();
                _timestamp.When=loc.gpsTime.GetValueOrDefault();
                _placemark.Id = loc.onSessionPointNum.ToString();
                _placemark.Geometry = _point;
                _placemark.Time = _timestamp;
                _placemark.Name = "FlyPoint " + _flightId;
                _pointplacemarks.Add(_placemark);
            }
        }
    }
}
