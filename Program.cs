using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ReaktorOrbitalChallenge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // consume our data
            var dataFile = "data2.txt";
            var lines = new List<string>();
            
            using (StreamReader reader = File.OpenText(dataFile))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
            }
            
            var satellites = new List<Satellite>();
            
            // store for easy access later
            var seed = lines.First().Replace("#SEED: ", "");
            
            // skip first and last lines
            for (var i = 1; i < lines.Count - 1; i++)
            {
                var parts = lines[i].Split(',');
                var satellite = new Satellite(parts[0], Double.Parse(parts[1]), Double.Parse(parts[2]), Double.Parse(parts[3]));
                satellites.Add(satellite);
            }
            
            // grab the earth based start/end
            var earthCoords = lines.Last().Split(',');
            var earthStart = new Satellite("E1", Double.Parse(earthCoords[1]), Double.Parse(earthCoords[2]), 0.00);
            var earthEnd = new Satellite("E2", Double.Parse(earthCoords[3]), Double.Parse(earthCoords[4]), 0.00);
            
            // initialize the graph
            var graph = new Dictionary<string, Dictionary<string, double>>();
            
            // some intermediate vars to find start/end satellites
            Satellite start = null;
            Satellite end = null;
            
            var d1 = double.MaxValue;
            var d2 = double.MaxValue;
            var dist1 = double.MaxValue;
            var dist2 = double.MaxValue;
            
            Console.WriteLine("Satellites:");
            
            foreach (var satellite in satellites)
            {
                // find neighbors
                foreach (var sat in satellites)
                {
                    if (sat.Id == satellite.Id) continue;
                    if (satellite.CanSee(sat))
                    {
                        satellite.AddNeighbor(sat);
                    }
                }
                Console.WriteLine(satellite);
                graph.Add(satellite.Id, satellite.GetNodes());
                
                // check distances to earth start/end
                dist1 = Maths.Distance(earthStart, satellite);
                //Console.WriteLine($"Distance from earth start to {satellite.Id}: {dist1}");
                if (dist1 < d1)
                {
                    d1 = dist1;
                    start = satellite;
                }
                
                dist2 = Maths.Distance(earthEnd, satellite);
                //Console.WriteLine($"Distance from earth end to {satellite.Id}: {dist2}");
                if (dist2 < d2)
                {
                    d2 = dist2;
                    end = satellite;
                }
            }
            
            Console.WriteLine("Ground Coordinates:");
            Console.WriteLine(earthStart);
            Console.WriteLine(earthEnd);
            
            // oops
            if (start == null)
            {
                Console.WriteLine("Start was null.");
                return;
            }
            if (end == null)
            {
                Console.WriteLine("End was null.");
                return;
            }
            
            // find the shortest path - implementation of djikstra's algorithm
            Console.WriteLine($"Finding path from {start.Id} to {end.Id}");
            
            var path = Maths.ShortestPath(graph, start.Id, end.Id);
            
            Console.WriteLine($"Hops: {path.Count}");
            
            var route = $"{start.Id},";            
            path.ForEach(v => route += $"{v},");
            route = route.TrimEnd(',');
            Console.WriteLine($"Route: {route}");
            Console.WriteLine($"Seed: {seed}");
        }
    }
}
