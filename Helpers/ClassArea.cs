﻿using System.Data.Entity;
using System.Linq;
using MVC_Acft_Track.Models;
using System.Collections.Generic;
using static MVC_Acft_Track.Finals;

namespace MVC_Acft_Track.Helpers
{
    public class ClassArea
    {
        public Entities db;
        int _areaId;
        string _radius;
        public ClassArea(int areaId,string radius)
        {
            _areaId = areaId;
            _radius = radius;
            db = new Entities();
        }

        public int areaId
        {
            set
            {
                _areaId = value;
            }
            get { return _areaId; }
        }

        public string radius
        {
            set
            {
                _radius = value;
            }
            get { return _radius; }
        }

        public AreaCircle circle
        {
            get {
                var c = area.Select(r => new { r.CenterLat, r.CenterLong }).ToList()[0];
                return new AreaCircle() { Lat = c.CenterLat, Long = c.CenterLong, Radius = _radius };
            }
        }

        public IQueryable<DimArea>area
        {
            get
            { return db.DimAreas.Where(r => r.AreaID.Equals(_areaId)); }
        }
        public class AreaCircle
        {
            public decimal? Lat;
            public decimal? Long;
            public string Radius;
        }
    }
}