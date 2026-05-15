using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Krasnoludki.Core.Algorithms;
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

        List<EdgeFlow> edges = new List<EdgeFlow>();
        EdgeFlow edge1 = new EdgeFlow(0,1,1,10);
        edges.Add(edge1);

        EdgeFlow edge2 = new EdgeFlow(0,2,1,5);
        edges.Add(edge2);

        EdgeFlow edge3 = new EdgeFlow(2,1,1,-2);
        edges.Add(edge3);

        EdgeFlow edge4 = new EdgeFlow(1,3,1,10);
        edges.Add(edge4);

        EdgeFlow edge5 = new EdgeFlow(2,3,1,20);
        edges.Add(edge5);



        ResidualNetwork network = new ResidualNetwork(nodes, edges, 0, 3);
        BellmanFordAlgorithm algorithm = new BellmanFordAlgorithm();

        int source = 0;
        
        Assert.Equal(new List<EdgeFlow>{edge2,edge3,edge4},
                algorithm.bellmanFordAlgorithm(network,source));

    }
}