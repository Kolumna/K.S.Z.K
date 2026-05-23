using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;

namespace Krasnoludki.Core
{
    public class DwarfAssigning
    {
        public static List<EdgeFlow> GenerateEdges(List<Dwarf> dwarves, List<Mine> mines, Source source, Sink sink)
        {
            List<EdgeFlow> result = new List<EdgeFlow>();
            foreach(Dwarf d in dwarves)
            {
                result.Add(new EdgeFlow(source, d, 1));       //tworzenie krawędzi od source do każdego krasnoludka
                foreach(Mine m in mines)
                {
                    EdgeFlow newEdge = new EdgeFlow(d, m, 1);       //tworzenie krawędzi od kranolduka do każdej koaplni
                    if(!d.PreferredMinerals.Contains(m.Resource)) newEdge.BadResource();    //ustawiania sztucznie wysokiego dystansu w przypadku niezgodności surowców i preferencji
                    result.Add(newEdge);
                }
            }
            foreach(Mine m in mines)
            {
                result.Add(new EdgeFlow(m, sink, m.Capacity));      //tworzenie krawędzi od każdej kopalni do sink
            }
            return result;
        }
        /*{
            List<EdgeFlow> result = new List<EdgeFlow>();
            foreach(Point p in points)
            {
                if(p.IsSoruce)
                {
                    foreach(Point d in points)
                    {
                        if(d is Dwarf)
                        {
                            result.Add(new EdgeFlow(p.PointId, d.PointId, 1));
                        }
                    }
                }
                if(p is Dwarf)
                {
                    Dwarf dwarf = (Dwarf)p;
                    foreach(Point m in points)
                    {
                        if(m is Mine)
                        {
                            Mine mine = (Mine)m;
                            if(dwarf.PreferredMinerals.Contains(mine.Resource))
                            {
                                result.Add(new EdgeFlow(dwarf.PointId, mine.PointId, 1));
                            }
                        }
                    }
                }
                if(p.IsSink)
                {
                    foreach(Point m in points)
                    {
                        if(m is Mine)
                        {
                            Mine mine = (Mine)m;
                            result.Add(new EdgeFlow(mine.PointId, p.PointId, mine.Capacity));
                        }
                    }
                }
            }
            return result;
        }*/
        public static bool BFS(int sourceId, int sink, List<EdgeFlow> edges, ref int[] parent)
        {
            HashSet<int> visited = new HashSet<int>();
            Queue<int> q = new Queue<int>();
            for(int i = 0; i < sink+1; i++)
            {
                parent[i] = -1;
            }

            q.Enqueue(sourceId);
            visited.Add(sourceId);

            while(q.Count > 0)
            {
                int p = q.Dequeue();

                foreach(EdgeFlow edge in edges)
                {
                    if(edge.From == p && edge.CurrFlow < edge.Capacity && !visited.Contains(edge.To))
                    {
                        q.Enqueue(edge.To);
                        visited.Add(edge.To);
                        parent[edge.To] = p;
                        if(edge.To == sink) return true;
                    }
                    /*if(edge.To == p && edge.BackCapacity > 0)
                    {
                        q.Enqueue(edge.From);
                        visited.Add(edge.From);
                        parent[edge.From] = p;
                    }*/
                }
            }
            return false;
        }
        /*public static int EdmondsKarp(int sourceId, int sink, List<EdgeFlow> edges)
        {
            int MaxFlow = 0;
            Stack<EdgeFlow> currPath = new Stack<EdgeFlow>();
            Stack<char> flowWay = new Stack<char>();

            int[] parent = new int[sink+1];
            while(BFS(sourceId, sink, edges, ref parent))
            {
                int newFlow = int.MaxValue;
                int i = sink;
                while(i != sourceId)
                {
                    foreach(EdgeFlow edge in edges)
                    {
                        if(edge.To == i && edge.From == parent[i])
                        {
                            currPath.Push(edge);
                            flowWay.Push('f');
                            newFlow = Math.Min(newFlow, edge.Capacity - edge.CurrFlow);
                            break;
                        }
                        if(edge.From == i && edge.To == parent[i])
                        {
                            currPath.Push(edge);
                            flowWay.Push('b');
                            newFlow = Math.Min(newFlow, edge.BackCapacity);
                            break;
                        }
                    }
                    i = parent[i];
                }
                while(currPath.Count() > 0)
                {
                    EdgeFlow edge = currPath.Pop();
                    char way = flowWay.Pop();
                    if(way == 'f')
                    {
                        edge.AddFlow(newFlow);
                    }
                    else edge.AddBackFlow(newFlow);
                }
                MaxFlow += newFlow;
            }

            return MaxFlow;
        }*/
        /*public static void Assign(Point[] points, EdgeFlow[] edges)
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
        }*/
    }
}

