using Krasnoludki.Core.Graph;
using Krasnoludki.Core.McmfAlgorithm.Graph;

namespace Krasnoludki.Core.Algorithms;

public class BellmanFordAlgorithm
{
    public List<GraphEdgeFlow> bellmanFordAlgorithm(ResidualNetwork network,int src)
    {

        int nodes_count = network.SinkID + 1;

        long[] distances = new long[nodes_count];
        GraphEdgeFlow[] parentEdge = new GraphEdgeFlow[nodes_count]; // array for tracking route

        // Initialize all distances to infinity and the source node to 0.
        Array.Fill(distances, long.MaxValue);
        distances[src] = 0;


        // Standard Bellman-Ford relaxation loop run V-1 times to find the single-source shortest paths.
        bool modified = false;
        for(int iteration_count = 0; iteration_count < nodes_count; iteration_count++)
        {
            modified = false;
            foreach(GraphEdgeFlow edge in network.Edges)
            {
                // Verify residual capacity. We only consider edges that can take more flow (Capacity - CurrFlow > 0)
                if (distances[edge.From] != long.MaxValue &&
                        edge.Capacity - edge.CurrFlow > 0 &&
                            distances[edge.From] + edge.Cost
                                < distances[edge.To])
                {
                    // Relax the edge: update the shortest known distance to the target node
                    // and record the predecessor to enable path reconstruction.
                    modified = true;
                    parentEdge[edge.To] = edge;
                    distances[edge.To] = distances[edge.From] + edge.Cost;
                }

                GraphEdgeFlow backEdge = edge.BackwardEdge;
                if (distances[backEdge.From] != long.MaxValue &&
                        backEdge.Capacity - backEdge.CurrFlow > 0 &&
                            distances[backEdge.From] + backEdge.Cost < distances[backEdge.To])
                {
                    modified = true;
                    parentEdge[backEdge.To] = backEdge;
                    distances[backEdge.To] = distances[backEdge.From] + backEdge.Cost;
                }
            }

            if(!modified) break;
        }



        //Finding the shortest path
        int currNode = network.SinkID;
        GraphEdgeFlow currEdge;
        List<GraphEdgeFlow> path = new List<GraphEdgeFlow>();

        if(distances[currNode] == long.MaxValue) // there is no path
        {
            return path;
        }

        while(currNode != src)
        {
            currEdge = parentEdge[currNode];
            currNode = currEdge != null? currEdge.From: -1;
            
            if(currEdge != null)
                path.Add(currEdge);

            else if(currNode == -1)
            {
                break;
            }
        }

        path.Reverse();
        return path;
        
    }
}