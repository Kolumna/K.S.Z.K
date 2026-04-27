using System;
using System.Runtime.CompilerServices;
using Krasnoludki.Core.Models;

namespace Krasnoludki.Core
{
    public class DwarfAssigning
    {
        public static int[] Dijkstra(Point start, Point[] points, EdgeWeight[] Edges)
        {
            int n = points.Length;
            int[] distances = new int[n];
            for(int i = 0; i < n; i++)
            {
                distances[i] = int.MaxValue;
            }

            int startIndex = Array.IndexOf(points, start);
            distances[startIndex] = 0;

            var pq = new PriorityQueue<(int distances, int index), int>();
            pq.Enqueue((0, startIndex), 0);

            while(pq.Count > 0)
            {
                var (currentDist, u) = pq.Dequeue();

                if(currentDist > distances[u]) continue;

                foreach(var edge in Edges)
                {
                    int v = -1;

                    if(edge.Connecting[0] == points[u])
                        v = Array.IndexOf(points, edge.Connecting[1]);
                    else if(edge.Connecting[1] == points[u])
                        v = Array.IndexOf(points, edge.Connecting[0]);

                    if(v == -1) continue;

                    int alt = distances[u] + edge.Length;

                    if(alt < distances[v])
                    {
                        distances[v] = alt;
                        pq.Enqueue((alt, v), alt);
                    }
                }
            }
            return distances;
        }
        public static void Assign(Point[] points, EdgeWeight[] edges)
        {
            foreach(Point d in points)
            {
                if(d.Type == PointType.Mine) continue;

                int[]  distance = Dijkstra(d, points, edges);

                List<MineralType> wantedResource = d.Dwarf.PreferredMinerals;
                Mine AssignedMine = new Mine(MineralType.None,0);       //pusta kopalnia jako placeholder
                int AssignedMineDistance = -1;

                int i = -1;
                foreach(Point m in points)
                {
                    i++;
                    if(m.Type == PointType.Dwarf) continue;
                    if(wantedResource.Contains(m.Mine.Resource) && (AssignedMineDistance > distance[i] || AssignedMineDistance == -1)
                        && !m.Mine.IsFull)
                    {
                        AssignedMine = m.Mine;
                    }
                }
                d.Dwarf.AssignMine(AssignedMine);
                AssignedMine.AddWorker();
            }
        }
    }
}

