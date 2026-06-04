using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.McmfAlgorithm.Models;
using Krasnoludki.Core.McmfAlgorithm.Graph;
using Xunit;
using System.IO.Pipelines;

namespace Krasnoludki.Test;

/// <summary>
/// Verifies the mathematical core of the Bellman-Ford algorithm using a mock graph.
/// Bypasses the geographic distance logic (Dwarves/Mines) to strictly validate 
/// whether the algorithm correctly identifies the mathematically cheapest path (Path 2) 
/// over a structurally shorter but more expensive path (Path 1).
/// </summary>
public class BellmanFordTest
{
    private ResidualNetwork CreateMockNetworkForBellmanFord()
    {
        int sourceId = 0;
        int sinkId = 3;

        var edges = new List<GraphEdgeFlow>
        {
            // Ścieżka 1 (Droższa: 10 + 50 = 60)
            new GraphEdgeFlow(sourceId, 1, 1, 10),
            new GraphEdgeFlow(1, sinkId, 1, 50),

            // Ścieżka 2 (Tańsza: 20 + 10 = 30)
            new GraphEdgeFlow(sourceId, 2, 1, 20),
            new GraphEdgeFlow(2, sinkId, 1, 10)
        };

        return new ResidualNetwork(new List<IGraphNode>(), edges, sourceId, sinkId);
    }

    [Fact]
    public void BellmanFordAlgorithmTest()
    {

        ResidualNetwork network = CreateMockNetworkForBellmanFord();
        BellmanFordAlgorithm algorithm = new BellmanFordAlgorithm();

        int source = 0;
        var result = algorithm.bellmanFordAlgorithm(network, source);

        Assert.True(result.Count == 2);

        Assert.Equal(source, result[0].From);
        Assert.Equal(2, result[0].To);
        Assert.Equal(1, result[0].Capacity);
        // Verify that the algorithm does not push flow itself, but only finds the route.
        Assert.Equal(0, result[0].CurrFlow);
        Assert.Equal(20, result[0].Cost);

        Assert.Equal(2, result[1].From);
        Assert.Equal(network.SinkID, result[1].To);
        Assert.Equal(1, result[1].Capacity);
        // Verify that the algorithm does not push flow itself, but only finds the route.
        Assert.Equal(0, result[1].CurrFlow);
        Assert.Equal(10, result[1].Cost);

    }
}