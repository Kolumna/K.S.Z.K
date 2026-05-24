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
        public static bool BFS(Point from, Point to, List<EdgeFlow> edges, ref int[] parent)
        {
            HashSet<int> visited = new HashSet<int>();
            Queue<int> q = new Queue<int>();
            for(int i = 0; i < to.HowManyPoints(); i++)
            {
                parent[i] = -1;
            }

            q.Enqueue(from.PointId);
            visited.Add(from.PointId);

            while(q.Count > 0)
            {
                int p = q.Dequeue();

                foreach(EdgeFlow edge in edges)
                {
                    if(edge.From == p && edge.CurrFlow < edge.Capacity && !visited.Contains(edge.To) && edge.Cost < 1000000)    //krawędzi utworzone z brakiem preferencji między krasnoludkiem i kopalnią są ognorowane
                    {
                        q.Enqueue(edge.To);
                        visited.Add(edge.To);
                        parent[edge.To-1] = p;
                        if(edge.To == to.PointId) return true;
                    }
                    if(edge.BackwardEdge.From == p && edge.BackwardEdge.CurrFlow < edge.BackwardEdge.Capacity && !visited.Contains(edge.BackwardEdge.To) && edge.Cost < 1000000)
                    {
                        q.Enqueue(edge.BackwardEdge.To);
                        visited.Add(edge.BackwardEdge.To);
                        parent[edge.BackwardEdge.To-1] = p;
                        if(edge.BackwardEdge.To == to.PointId) return true;     //teoretycznie nigdy nie trafi się do sink przez krawędź wsteczną ale tak na wszelki to zostawiam
                    }
                }
            }
            return false;
        }
        public static int EdmondsKarp(Point source, Point sink, List<EdgeFlow> edges)
        {
            int MaxFlow = 0;
            Stack<EdgeFlow> currPath = new Stack<EdgeFlow>();
            int sourceId = source.PointId;
            int sinkId = sink.PointId;

            int[] parent = new int[sink.HowManyPoints()];
            while(BFS(source, sink, edges, ref parent))
            {
                int newFlow = int.MaxValue;
                int i = sinkId;
                while(i != sourceId)        //pętla odtwarza ściężkę z source do sink na podstawie tablicy zwróconej przez bfs i znajduję maksymalny pływ
                {
                    foreach(EdgeFlow edge in edges)
                    {
                        if(edge.To == i && edge.From == parent[i-1])
                        {
                            currPath.Push(edge);
                            newFlow = Math.Min(newFlow, edge.Capacity - edge.CurrFlow);
                            break;
                        }
                        if(edge.BackwardEdge.To == i && edge.BackwardEdge.From == parent[i-1])
                        {
                            currPath.Push(edge.BackwardEdge);
                            newFlow = Math.Min(newFlow, edge.BackwardEdge.Capacity - edge.BackwardEdge.CurrFlow);
                            break;
                        }
                    }
                    i = parent[i-1];
                }
                while(currPath.Count() > 0)     //pętla dodaje pływ do każdej krawędzi przed kolejnym wywołaniem bfs
                {
                    EdgeFlow edge = currPath.Pop();
                    edge.AddFlow(newFlow);
                }
                MaxFlow += newFlow;         //dodanie wysłanego pływ do w obecniej iteracji do całkowitego pływu
            }

            return MaxFlow;
        }
        public static void Assign(List<Dwarf> dwarves, List<Mine> mines)
        {
            Source source = new Source();
            Sink sink = new Sink();

            List<EdgeFlow> edges = GenerateEdges(dwarves, mines, source, sink);

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
                            }
                        }
                    }
                }
            }
        }
    }
}

