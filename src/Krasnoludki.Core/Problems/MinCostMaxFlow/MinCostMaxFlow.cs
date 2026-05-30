using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;

namespace Krasnoludki.Core.Problems;

/// <summary>
/// Solves the Min-Cost Max-Flow problem.
/// Combines the concept of the Ford-Fulkerson algorithm (maximum flow) 
/// with the Bellman-Ford algorithm for finding the minimum cost augmenting paths.
/// </summary>
/// <param name="network">The initialized residual network with nodes and capacities.</param>
/// <returns>A tuple containing the total minimum cost (MinCost) and the maximum flow (MaxFlow).</returns>
/// <remarks>
/// If the final TotalCost is massive (e.g., >= 1,000,000), 
/// it indicates that some dwarves were assigned to non-preferred mines due to capacity 
/// limits. The real distance can then be extracted using a modulo operation.
/// </remarks>
public class MinCostMaxFlowProblem
{
    public (double,double) MinCostMaxFlow(ResidualNetwork network)
    {
        int source = network.SourceID;

        BellmanFordAlgorithm algorithm = new BellmanFordAlgorithm();
        double MinCost = 0, MaxFlow = 0;
        List<EdgeFlow> path = new List<EdgeFlow>();

        while (true)
        {
            // Searching for the new augmenting path with Bellman-Ford algorithm
            path = algorithm.bellmanFordAlgorithm(network,source);
            //If there's no more augmenting path Min Cost Max Flow Algorithm can stop
            if(path.Count == 0)
                break;
            
            int residualCapacity = int.MaxValue;
            foreach (EdgeFlow edge in path) // Searching for the residual capacity of a path: \(c_f(p)\)
            {
                residualCapacity = (edge.Capacity - edge.CurrFlow) < residualCapacity?
                    (edge.Capacity - edge.CurrFlow): residualCapacity;
            }

            MaxFlow += residualCapacity;
            foreach(EdgeFlow edge in path) // For every edge in graph increase flow by new residual capacity of a path
            {   
                if(edge.BackwardEdge is not null)
                    edge.BackwardEdge.CurrFlow -= residualCapacity;
                
                edge.CurrFlow += residualCapacity;

                MinCost += (residualCapacity * edge.Cost);
            }
        }

       return (MinCost,MaxFlow);
       
    }

/// <summary>
/// Extracts the final dwarf-to-mine assignments from the residual network after the algorithm's execution.
/// </summary>
/// <param name="networkAfterMCMF">The residual network with populated flows (CurrFlow).</param>
/// <returns>A list of point-to-point pairs (Start - End) ready for frontend visualization.</returns>
    public List<Tuple<Krasnoludki.Core.Models.Point,Krasnoludki.Core.Models.Point>> GetReadyPointToPointEdges(ResidualNetwork networkAfterMCMF)
    {
        List<Tuple<Krasnoludki.Core.Models.Point,Krasnoludki.Core.Models.Point>> ReadyEdges =
             new List<Tuple<Krasnoludki.Core.Models.Point, Krasnoludki.Core.Models.Point>>();


        // WARNING: The index calculation below strictly depends on the exact edge insertion order 
        // inside the ResidualNetwork. Modifying the generation order will break this extraction logic.
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