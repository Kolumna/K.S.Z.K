using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.McmfAlgorithm.Models;
using Krasnoludki.Core.McmfAlgorithm.Graph;
using Xunit;

namespace Krasnoludki.Test;

public class BellmanFordTest
{
    [Fact]
    public void BellmanFordAlgorithmTest()
    {

         List<IGraphNode> nodes = new List<IGraphNode>
        {
            new GraphNode<object?>(0, null),
            new GraphNode<object?>(1, null),
            new GraphNode<object?>(2, null),
            new GraphNode<object?>(3, null)
        };

        List<GraphEdgeFlow> edges = new List<GraphEdgeFlow>();
        GraphEdgeFlow edge1 = new GraphEdgeFlow(0,1,1,10);
        edges.Add(edge1);

        GraphEdgeFlow edge2 = new GraphEdgeFlow(0,2,1,5);
        edges.Add(edge2);

        GraphEdgeFlow edge3 = new GraphEdgeFlow(2,1,1,-2);
        edges.Add(edge3);

        GraphEdgeFlow edge4 = new GraphEdgeFlow(1,3,1,10);
        edges.Add(edge4);

        GraphEdgeFlow edge5 = new GraphEdgeFlow(2,3,1,20);
        edges.Add(edge5);



        ResidualNetwork network = new ResidualNetwork(nodes, edges, 0, 3);
        BellmanFordAlgorithm algorithm = new BellmanFordAlgorithm();

        int source = 0;
        
        Assert.Equal(new List<GraphEdgeFlow>{edge2,edge3,edge4},
                algorithm.bellmanFordAlgorithm(network,source));

    }
}