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
        public static bool BFS(Source source, Sink sink, List<EdgeFlow> edges, ref EdgeFlow[] parent)
        {
            HashSet<int> visited = new HashSet<int>();
            Queue<int> q = new Queue<int>();
            parent = new EdgeFlow[sink.PointId+1];

            q.Enqueue(source.PointId);
            visited.Add(source.PointId);

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
                        if(edge.To == sink.PointId) return true;
                    }
                }
            }
            return false;
        }
        public static int EdmondsKarp(Source source, Sink sink, List<EdgeFlow> edges)
        {
            int MaxFlow = 0;
            int sinkId = sink.PointId;

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
            Source source = new Source();
            Sink sink = new Sink(dwarves.Count() + mines.Count());

            List<EdgeFlow> edges = EdgeGen.GenerateEdges(dwarves, mines, source, sink);

            EdmondsKarp(source, sink, edges);

            foreach(Dwarf dwarf in dwarves)         //dla każdego krasnoludka po wykonaniu przypisania
            {
                foreach(EdgeFlow edge in edges)     
                {
                    if(edge.From == dwarf.PointId && edge.CurrFlow == 1)    //sprawdza która krawędź została wykorzystana do wysłania go do kopalni
                    {
                        foreach(Mine mine in mines)
                        {
                            if(mine.PointId == edge.To)     //na podstawie tego szuka tej kopalni
                            {
                                dwarf.AssignMine(mine);     //przypisuje kopalnie krasnoluskowi
                                mine.AddWorker(dwarf);      //i krasnoludka kopalni
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}


