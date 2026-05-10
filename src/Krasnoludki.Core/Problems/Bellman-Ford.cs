using Krasnoludki.Core.Graph;

namespace Krasnoludki.Core.Algorithms;

class BellmanFordAlgorithm
{

    public List<EdgeFlow> bellmanFordAlgorithm(ResidualNetwork network,int src)
    {

        int nodes_count = network.SinkID + 1;

        double[] distances = new double[nodes_count];
        EdgeFlow[] parentEdge = new EdgeFlow[nodes_count]; // array for tracking route

        Array.Fill(distances, double.MaxValue);
        distances[src] = 0;

        bool modified = false;
        for(int iteration_count = 0; iteration_count < nodes_count; iteration_count++)
        {
            modified = false;
            foreach(EdgeFlow edge in network.Edges)
            {
                //Relaxation
                if (distances[edge.From] != double.MaxValue &&
                        edge.Capacity - edge.CurrFlow > 0 &&
                            distances[edge.From] + edge.Cost
                                < distances[edge.To])
                {
                    modified = true;
                    parentEdge[edge.To] = edge;
                    distances[edge.To] = distances[edge.From] + edge.Cost;
                }
            }

            if(!modified) break;
        }


        List<int> AffectedNodes = new List<int>();
        List<int> NegativeCycle = new List<int>();

        //Finding nodes from negative cycles
        foreach(EdgeFlow edge in network.Edges)
        {
            
            if (distances[edge.From] != double.MaxValue &&
                        edge.Capacity - edge.CurrFlow > 0 &&
                            distances[edge.From] + edge.Cost
                                < distances[edge.To])
            {
                distances[edge.To] = distances[edge.From] + edge.Cost;

                if(!AffectedNodes.Contains(edge.To))
                    AffectedNodes.Add(edge.To);
            }
        }


        //Finding the shortest path
        int currNode = network.SinkID;
        EdgeFlow currEdge;
        List<EdgeFlow> path = new List<EdgeFlow>();

        if(distances[currNode] == double.MaxValue) // there is no path
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