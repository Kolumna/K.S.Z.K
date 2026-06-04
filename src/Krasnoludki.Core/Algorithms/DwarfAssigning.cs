using System;
using System.IO.Pipelines;
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
                    EdgeFlow backEdge = edge.BackwardEdge;
                    if(backEdge.From == p && backEdge.CurrFlow < backEdge.Capacity && !visited.Contains(backEdge.To) && backEdge.Cost < 1000000)    //krawędzi utworzone z brakiem preferencji między krasnoludkiem i kopalnią są ognorowane
                    {
                        q.Enqueue(backEdge.To);
                        visited.Add(backEdge.To);
                        parent[backEdge.To] = backEdge;
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
        public static List<int[]> Assign(List<Dwarf> dwarves, List<Mine> mines)
        {
            List<int[]> result = new List<int[]>();
            Source source = new Source();
            Sink sink = new Sink(dwarves.Count() + mines.Count());

            List<EdgeFlow> edges = EdgeGen.GenerateEdges(dwarves, mines, source, sink);

            EdmondsKarp(source, sink, edges);

            foreach(EdgeFlow e in edges)
            {
                if(e.From == 0 || e.CurrFlow <= 0)      //sprawdza czy krawędź nie jest od source lub czy jest nie używana
                {
                    continue;           //jeśli tak to pomija
                }
                if(e.To == sink.PointId)        //krawedzie do sink sa ostanie w liscie wiec gdy petal do nich dotrze, zostaje przerwana
                {
                    break;
                }
                result.Add(new int[]{e.From, e.To});    //dodaje do listy wyników tab int[2] gdzie tab[0] = id krasnoludka, a tab[1] = id kopalni do  której został przydzielony
            }
            return result;
        }
    }
}


