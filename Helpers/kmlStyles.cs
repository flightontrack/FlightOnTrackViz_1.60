using System;
using System.Collections;
using SharpKml.Base;
using SharpKml.Dom;

namespace kmlStyles
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
            var lc = (FlightLineColors)flightStylesColors[i < 3 ? i : 3];
            //_instanceCount=_instanceCount == 1 ? 0 : 1;
            _style = new Style();
            _style.Line = new LineStyle();
            _style.Line.Width = 0.3;
            _style.Line.Color = new Color32(lc._lalpha, lc._lblue, lc._lgreen, lc._lred);
            _style.Polygon = new PolygonStyle();
            _style.Polygon.ColorMode = ColorMode.Random;
            _style.Polygon.Color = new Color32(lc._palpha, lc._pblue, lc._pgreen, lc._pred);
            _style.Id = "Line"+lc._styleId;
        }
    }

    class FlightPointStyles
    {
        private static int _instanceCount = 0;
        public ArrayList flightStylesColors = new ArrayList();
        private Style _style;

        public Style style
        {
            get { return _style; }
        }
        public FlightPointStyles(int i,int hide)
        {
            _instanceCount = i;
            flightStylesColors.Add(new YellowGreenStyle());
            flightStylesColors.Add(new RedBlueStyle());
            flightStylesColors.Add(new BlueBlueStyle());
            flightStylesColors.Add(new WhiteWhiteStyle());
            var lc = (FlightLineColors)flightStylesColors[i < 3 ? i : 3];
            //_instanceCount=_instanceCount == 1 ? 0 : 1;
            _style = new Style();
            var labelStyleHide = new  LabelStyle();
            labelStyleHide.Scale = hide;
            _style.Label = labelStyleHide;
            _style.Icon = new IconStyle();
            _style.Icon.Color = new Color32(lc._lalpha, lc._lblue, lc._lgreen, lc._lred);
            _style.Icon.Icon = new IconStyle.IconLink(new System.Uri("http://maps.google.com/mapfiles/kml/shapes/road_shield3.png"));
            _style.Icon.Scale = 0.3;
            _style.Id = "Point_"+(hide==0?"hide_":"show_")+lc._styleId;
        }
    }
    class StyleMap
    {
        StyleMapCollection _styleMap = new StyleMapCollection();
        public StyleMapCollection styleMap
        {
            get { return _styleMap; }
        }
        public StyleMap(Style style1, Style style2)
        {
            var pair1 = new Pair();
            pair1.State = StyleState.Normal;
            pair1.StyleUrl = new Uri("#" + style1.Id, UriKind.Relative);
            var pair2 = new Pair();
            pair2.State = StyleState.Highlight;
            pair2.StyleUrl = new Uri("#" + style2.Id, UriKind.Relative);
            //var styleMap = new StyleMapCollection();
            _styleMap.Add(pair1);
            _styleMap.Add(pair2);
            _styleMap.Id = "Point_hide_show";
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
}
