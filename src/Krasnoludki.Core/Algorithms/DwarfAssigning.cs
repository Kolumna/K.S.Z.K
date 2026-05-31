using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;

namespace Krasnoludki.Core
{
    public class DwarfAssigning
    {
        public static bool BFS(int source, int sink, List<EdgeFlow> edges, ref EdgeFlow[] parent)
        {
            HashSet<int> visited = new HashSet<int>();
            Queue<int> q = new Queue<int>();
            parent = new EdgeFlow[sink+1];

            q.Enqueue(source);
            visited.Add(source);

            while(q.Count > 0)
            {
                int p = q.Dequeue();

                foreach(EdgeFlow edge in edges)
                {
                    if(edge.From == p && edge.CurrFlow < edge.Capacity && !visited.Contains(edge.To) && edge.Cost < 1000000)    //krawędzi utworzone z brakiem preferencji między krasnoludkiem i kopalnią są ognorowane
                    {
                        q.Enqueue(edge.To);
                        visited.Add(edge.To);
                        parent[edge.To] = edge;
                        if(edge.To == sink) return true;
                    }
                    if(edge.BackwardEdge.From == p && edge.BackwardEdge.CurrFlow < edge.BackwardEdge.Capacity && !visited.Contains(edge.BackwardEdge.To) && edge.Cost < 1000000)
                    {
                        q.Enqueue(edge.BackwardEdge.To);
                        visited.Add(edge.BackwardEdge.To);
                        parent[edge.BackwardEdge.To] = edge.BackwardEdge;
                        //if(edge.BackwardEdge.To == to.PointId) return true;     //teoretycznie nigdy nie trafi się do sink przez krawędź wsteczną ale tak na wszelki to zostawiam
                    }
                }
            }
            return false;
        }
        public static int EdmondsKarp(int source, int sink, List<EdgeFlow> edges)
        {
            int MaxFlow = 0;
            int sinkId = sink;

            EdgeFlow[] parent = new EdgeFlow[sinkId+1];
            while(BFS(source, sink, edges, ref parent))     
            {
                Stack<EdgeFlow> currPath = new Stack<EdgeFlow>();
                int newFlow = int.MaxValue;

                for(EdgeFlow e = parent[sinkId]; e != null; e = parent[e.From])
                {
                    newFlow = Math.Min(newFlow, e.ReturnCapacity());
                    currPath.Push(e);
                }

                while(currPath.Count() > 0)     //pętla dodaje pływ do każdej krawędzi przed kolejnym wywołaniem bfs
                {
                    EdgeFlow edge = currPath.Pop();
                    edge.AddFlow(newFlow);
                }
                MaxFlow += newFlow;         //dodanie wysłanego pływ do w obecniej iteracji do całkowitego pływu
                parent = new EdgeFlow[sinkId+1];      //reset tablicy
            }
            return MaxFlow;
        }
        public static void Assign(List<Dwarf> dwarves, List<Mine> mines)
        {
            int source = 0;
            int sink = dwarves.Count() + mines.Count() + 1;

            List<EdgeFlow> edges = EdgeGen.GenerateEdges(dwarves, mines, source, sink);

            EdmondsKarp(source, sink, edges);

            foreach(Dwarf dwarf in dwarves)         //dla każdego krasnoludka po wykonaniu przypisania
            {
                foreach(EdgeFlow edge in edges)     
                {
                    if(edge.From == dwarf.HomeLocation.PointId && edge.CurrFlow == 1)    //sprawdza która krawędź została wykorzystana do wysłania go do kopalni
                    {
                        foreach(Mine mine in mines)
                        {
                            if(mine.Location.PointId == edge.To)     //na podstawie tego szuka tej kopalni
                            {
                                mine.AddWorker(dwarf);     //przypisuje kopalnie krasnoluskowi i krasnoludka kopalni
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}


