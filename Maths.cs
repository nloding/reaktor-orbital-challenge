using System;
using System.Collections.Generic;

namespace ReaktorOrbitalChallenge
{
    public static class Maths
    {
        public static bool CanSee(Satellite sat1, Satellite sat2)
        {
            double cx = 0.00;
            double cy = 0.00;
            double cz = 0.00;

            double px = sat1.X;
            double py = sat1.Y;
            double pz = sat1.Z;

            double vx = sat2.X - px;
            double vy = sat2.Y - py;
            double vz = sat2.Z - pz;
            
            double A = vx * vx + vy * vy + vz * vz;
            double B = 2.0 * (px * vx + py * vy + pz * vz - vx * cx - vy * cy - vz * cz);
            double C = px * px - 2 * px * cx + cx * cx + py * py - 2 * py * cy + cy * cy +
                    pz * pz - 2 * pz * cz + cz * cz - Earth.Radius * Earth.Radius;

            // discriminant
            double D = B * B - 4 * A * C;
            //Console.WriteLine($"{sat1.Id} to {sat2.Id} = {D}");
            return D < 0;
        }
        
        public static double Distance(Satellite sat1, Satellite sat2)
        {
            var deltaX = sat2.X - sat1.X;
            var deltaY = sat2.Y - sat1.Y;
            var deltaZ = sat2.Z - sat1.Z;

            return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
        }
        
        public static double Distance(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            var deltaX = x2 - x1;
            var deltaY = y2 - y1;
            var deltaZ = z2 - z1;

            return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
        }
        
        public static List<string> ShortestPath(Dictionary<string, Dictionary<string, double>> graph, string start, string end)
        {
            var previous = new Dictionary<string, string>();
            var distances = new Dictionary<string, double>();
            var nodes = new List<string>();

            List<string> path = null;

            foreach (var vertex in graph)
            {
                if (vertex.Key == start)
                {
                    distances[vertex.Key] = 0;
                }
                else
                {
                    distances[vertex.Key] = double.MaxValue;
                }

                nodes.Add(vertex.Key);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => (int) (distances[x] - distances[y]));

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == end)
                {
                    path = new List<string>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == float.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in graph[smallest])
                {
                    var alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            // reverse it, because we worked backwards
            path.Reverse();
            return path;
        }
    }
}