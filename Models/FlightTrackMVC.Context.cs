﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DimArea> DimAreas { get; set; }
        public virtual DbSet<SearchRequest> SearchRequests { get; set; }
        public virtual DbSet<vListAirport> vListAirports { get; set; }
        public virtual DbSet<vListPilot> vListPilots { get; set; }
        public virtual DbSet<AircraftPilot> AircraftPilots { get; set; }
        public virtual DbSet<vAircraftPilot> vAircraftPilots { get; set; }
        public virtual DbSet<vPilotLogBook> vPilotLogBooks { get; set; }
        public virtual DbSet<DimAircraftRemote> DimAircraftRemotes { get; set; }
        public virtual DbSet<vFlightAcftPilot> vFlightAcftPilots { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<GpsLocation> GpsLocations { get; set; }
        public virtual DbSet<vVisualPilotLogBook> vVisualPilotLogBook { get; set; }
        public virtual DbSet<vVisualPilotLogDestinations> vVisualPilotLogDestinations { get; set; }
        public virtual DbSet<AirportCoordinates> AirportCoordinates { get; set; }
        public virtual DbSet<DimAcftGroup> DimAcftGroups { get; set; }
        public virtual DbSet<AircraftGroup> AircraftGroups { get; set; }
        public virtual DbSet<vRoute> vRoutes { get; set; }
        public virtual DbSet<vListAircraft> vListAircrafts { get; set; }
        public virtual DbSet<Pilot> Pilots { get; set; }
    
        public virtual ObjectResult<Nullable<bool>> get_isArea(Nullable<int> gpsLocationID, Nullable<int> areaID, string radius)
        {
            var gpsLocationIDParameter = gpsLocationID.HasValue ?
                new ObjectParameter("GpsLocationID", gpsLocationID) :
                new ObjectParameter("GpsLocationID", typeof(int));
    
            var areaIDParameter = areaID.HasValue ?
                new ObjectParameter("AreaID", areaID) :
                new ObjectParameter("AreaID", typeof(int));
    
            var radiusParameter = radius != null ?
                new ObjectParameter("Radius", radius) :
                new ObjectParameter("Radius", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<bool>>("get_isArea", gpsLocationIDParameter, areaIDParameter, radiusParameter);
        }
    
        public virtual ObjectResult<string> get_PilotValue(Nullable<int> pilotid)
        {
            var pilotidParameter = pilotid.HasValue ?
                new ObjectParameter("pilotid", pilotid) :
                new ObjectParameter("pilotid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("get_PilotValue", pilotidParameter);
        }
    
        public virtual ObjectResult<string> get_PilotP(string phonenumber, string deviceid)
        {
            var phonenumberParameter = phonenumber != null ?
                new ObjectParameter("phonenumber", phonenumber) :
                new ObjectParameter("phonenumber", typeof(string));
    
            var deviceidParameter = deviceid != null ?
                new ObjectParameter("deviceid", deviceid) :
                new ObjectParameter("deviceid", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("get_PilotP", phonenumberParameter, deviceidParameter);
        }
    
        public virtual int update_PilotGuid(Nullable<int> pilotid)
        {
            var pilotidParameter = pilotid.HasValue ?
                new ObjectParameter("pilotid", pilotid) :
                new ObjectParameter("pilotid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("update_PilotGuid", pilotidParameter);
        }
    
        [DbFunction("Entities", "fVisualPilotLogBook")]
        public virtual IQueryable<fVisualPilotLogBook_Result> fVisualPilotLogBook(Nullable<int> pilotId)
        {
            var pilotIdParameter = pilotId.HasValue ?
                new ObjectParameter("pilotId", pilotId) :
                new ObjectParameter("pilotId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fVisualPilotLogBook_Result>("[Entities].[fVisualPilotLogBook](@pilotId)", pilotIdParameter);
        }
    
        [DbFunction("Entities", "fVisualPilotLogDestinations")]
        public virtual IQueryable<fVisualPilotLogDestinations_Result> fVisualPilotLogDestinations(Nullable<int> pilotId)
        {
            var pilotIdParameter = pilotId.HasValue ?
                new ObjectParameter("pilotId", pilotId) :
                new ObjectParameter("pilotId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fVisualPilotLogDestinations_Result>("[Entities].[fVisualPilotLogDestinations](@pilotId)", pilotIdParameter);
        }
    
        public virtual int merge_Flights(string flightsstring)
        {
            var flightsstringParameter = flightsstring != null ?
                new ObjectParameter("flightsstring", flightsstring) :
                new ObjectParameter("flightsstring", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("merge_Flights", flightsstringParameter);
        }
    }
}
