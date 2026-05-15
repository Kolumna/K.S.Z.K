using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.Graph;

namespace Krasnoludki.Core.Problems;

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
}