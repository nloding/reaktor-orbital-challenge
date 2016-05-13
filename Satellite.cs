using System;
using System.Collections.Generic;
using System.Linq;

namespace ReaktorOrbitalChallenge
{
    public class Satellite
    {
        public string Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        
        public List<Satellite> Neighbors { get; set; }
        
        public bool Visited { get; set; }
        
        public Satellite(string id, double latitude, double longitude, double altitude)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude + Earth.Radius;
            
            LLAToECEF();
        }
        
        public void AddNeighbor(Satellite sat)
        {
            if (Neighbors == null) Neighbors = new List<Satellite>();
            Neighbors.Add(sat);
        }
        
        public bool CanSee(Satellite sat2)
        {
            return Maths.CanSee(this, sat2);
        }
        
        public Dictionary<string, double> GetNodes()
        {
            var dict = new Dictionary<string, double>();
            if (Neighbors == null) return dict;
            foreach (var neighbor in Neighbors)
            {
                //Console.WriteLine($"{this.Id} distance to {neighbor.Id} = {Maths.Distance(this, neighbor)}");
                dict.Add(neighbor.Id, Maths.Distance(this, neighbor));
            }
            return dict;
        }
        
        public override string ToString()
        {
            if (Neighbors == null) Neighbors = new List<Satellite>();
            var neighbors = string.Join(", ", Neighbors.Select(x => x.Id).ToList());
            return $"{Id}: ({X}, {Y}, {Z}), Neighbors: {neighbors}";
        }
        
        private void LLAToECEF()
        {
            // to radians
            var lat = Latitude * (Math.PI/180.0);
            var lon = Longitude * (Math.PI/180.0);
            
            X = Math.Cos(lat) * Math.Cos(lon) * Altitude;
            Y = Math.Cos(lat) * Math.Sin(lon) * Altitude;
            Z = Math.Sin(lat) * Altitude;

            // var f  = 0;                                                     //flattening
            // var ls = Math.Atan(Math.Pow((1.0 - f),2) * Math.Tan(lat));    // lambda

            // X = Earth.Radius * Math.Cos(ls) * Math.Cos(lon) + Altitude * Math.Cos(lat) * Math.Cos(lon);
            // Y = Earth.Radius * Math.Cos(ls) * Math.Sin(lon) + Altitude * Math.Cos(lat) * Math.Sin(lon);
            // Z = Earth.Radius * Math.Sin(ls) + Altitude * Math.Sin(lat);
        }
    }
}
