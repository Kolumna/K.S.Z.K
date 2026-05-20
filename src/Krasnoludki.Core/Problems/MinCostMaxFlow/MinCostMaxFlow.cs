using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;

namespace Krasnoludki.Core.Problems;

public class MinCostMaxFlowProblem
{
    // Note: If the Final TotalCost is massive (e.g., >= 1,000,000),
    //  it indicates that some dwarves were assigned to non-preferred mines due to capacity
    //  limits. The real distance can be extracted using a modulo operation.
    public (double,double) MinCostMaxFlow(ResidualNetwork network)
    {
        int source = network.SourceID;

        BellmanFordAlgorithm algorithm = new BellmanFordAlgorithm();
        double MinCost = 0, MaxFlow = 0;
        List<EdgeFlow> path = new List<EdgeFlow>();

        while (true)
        {
            path = algorithm.bellmanFordAlgorithm(network,source);
            if(path.Count == 0)
                break;
            
            int residualCapacity = int.MaxValue;
            foreach (EdgeFlow edge in path)
            {
                residualCapacity = (edge.Capacity - edge.CurrFlow) < residualCapacity?
                    (edge.Capacity - edge.CurrFlow): residualCapacity;
            }

            MaxFlow += residualCapacity;

            foreach(EdgeFlow edge in path)
            {   
                if(edge.BackwardEdge is not null)
                    edge.BackwardEdge.CurrFlow -= residualCapacity;
                
                edge.CurrFlow += residualCapacity;

                MinCost += (residualCapacity * edge.Cost);
            }
        }

       return (MinCost,MaxFlow);
       
    }


    public List<Tuple<Krasnoludki.Core.Models.Point,Krasnoludki.Core.Models.Point>> GetReadyPointToPointEdges(ResidualNetwork networkAfterMCMF)
    {
        List<Tuple<Krasnoludki.Core.Models.Point,Krasnoludki.Core.Models.Point>> ReadyEdges =
             new List<Tuple<Krasnoludki.Core.Models.Point, Krasnoludki.Core.Models.Point>>();


        int index = 2 * networkAfterMCMF.DwarvesCount + 1;
        int last_dwarf_mine_index =  2 * (networkAfterMCMF.DwarvesCount * networkAfterMCMF.MinesCount) + 1;

        for(; index < last_dwarf_mine_index; index++)
        {
            if(networkAfterMCMF.Edges[index].CurrFlow > 0)
            {
                // Searching for Dwarf's Home Location on map 
                var FromNode = networkAfterMCMF.GetNode(networkAfterMCMF.Edges[index].From);
                var CurrDwarf = ((GraphNode<Dwarf>)FromNode).Data;
                Krasnoludki.Core.Models.Point DwarfLoc = CurrDwarf.HomeLocation;

                // Searching for Mine's map location
                var ToNode = networkAfterMCMF.GetNode(networkAfterMCMF.Edges[index].To);
                var CurrMine = ((GraphNode<Mine>)ToNode).Data;
                Krasnoludki.Core.Models.Point MineLoc = CurrMine.Location;

                //Adding new Point to Point Edge for visualization
                Tuple<Krasnoludki.Core.Models.Point, Krasnoludki.Core.Models.Point> DwarfToMinePointsEdge =
                     new Tuple<Krasnoludki.Core.Models.Point, Krasnoludki.Core.Models.Point>(DwarfLoc,MineLoc);
                
                ReadyEdges.Add(DwarfToMinePointsEdge);
            }
        }

        return ReadyEdges;
    }
}