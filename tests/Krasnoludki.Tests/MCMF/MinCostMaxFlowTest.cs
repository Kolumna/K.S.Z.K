using Krasnoludki.Core.Models;
using Krasnoludki.Core.Problems;
using Krasnoludki.Core.McmfAlgorithm.Adapter;
using Krasnoludki.Core.McmfAlgorithm.Models;
using Krasnoludki.Core.Graph;
using System.Linq;
using Xunit;

namespace Krasnoludki.Test;

/// <summary>
/// Integration test for the Min-Cost Max-Flow algorithm using a "Gold Rush" scenario.
/// Validates capacity constraints and conflict resolution when multiple dwarves compete 
/// for a limited-capacity preferred mine, forcing the algorithm to mathematically 
/// route the least optimal dwarf to a backup mine.
/// </summary>
public class MinCostMaxFlowProblemTest
{
    private List<Dwarf> GetMcmfIntegrationDwarves()
    {
        return new List<Dwarf>
        {
            // Group 1: Gold miners (lined up close to mine 101)
            new Dwarf(1, 0.0, 10.0, new List<MineralType> { MineralType.Gold }, 5),
            new Dwarf(2, 0.0, 12.0, new List<MineralType> { MineralType.Gold }, 5),
            new Dwarf(3, 0.0, 14.0, new List<MineralType> { MineralType.Gold }, 5),

            // Group 2: Silver miners
            new Dwarf(4, 10.0, 0.0, new List<MineralType> { MineralType.Silver }, 5),
            new Dwarf(5, 12.0, 0.0, new List<MineralType> { MineralType.Silver }, 5),

            // Group 3: Outcast (no preferences, will incur penalty)
            new Dwarf(6, 100.0, 100.0, new List<MineralType>(), 5)
        };
    }

    private List<Mine> GetMcmfIntegrationMines()
    {
        return new List<Mine>
        {
            // Mine 101: Gold, close to Group 1, but capacity limited to 2
            new Mine(101, 0.0, 0.0, MineralType.Gold, 2),

            // Mine 102: Gold fallback (further away, high capacity)
            new Mine(102, 0.0, 50.0, MineralType.Gold, 5),

            // Mine 103: Silver, close to Group 2, exact capacity match
            new Mine(103, 10.0, 10.0, MineralType.Silver, 2),

            // Mine 104: Coal, far away, meant for the outcast
            new Mine(104, 100.0, 110.0, MineralType.Coal, 10)
        };
    }

    [Fact]
    public void TestMCMF()
    {
        McmfMapper mapper = new McmfMapper();

        List<GraphDwarf> dwarves = mapper.MapDwarves(GetMcmfIntegrationDwarves());
        List<GraphMine> mines = mapper.MapMines(GetMcmfIntegrationMines());

        ResidualNetwork network = new ResidualNetwork(dwarves, mines);

        // Validate residual network topological layers
        Assert.Contains(network.Edges, e => e.From == network.SourceID && e.Capacity > 0);
        Assert.Contains(network.Edges, e => e.From > 0 && e.To > network.DwarvesCount && e.Capacity > 0);
        Assert.Contains(network.Edges, e => e.To == network.SinkID && e.Capacity > 0);

        MinCostMaxFlowProblem mcmf = new MinCostMaxFlowProblem();
        var (minCost, maxFlow) = mcmf.MinCostMaxFlow(network);
        var (assignments, employedCount) = mcmf.ExtractAssignments(network);

        // 1. Verify flow completion: Ensure all dwarves successfully found employment
        Assert.Equal(5, maxFlow);
        Assert.Equal(5, employedCount);

        // 2. Verify business logic & penalties
        Assert.Equal(78.19804, minCost, 5); 

        // 3. Verify MCMF optimization (Conflict Resolution)
        // Mine 101 has a capacity of 2. Dwarves 1 and 2 are closer, so Dwarf 3 
        // must be outbid and pushed to the backup Mine 102.
        var mine101Assignments = assignments.Where(a => a.MineId == 101).ToList();
        Assert.Equal(2, mine101Assignments.Count);

        var dwarf1 = assignments.Single(a => a.DwarfId == 1);
        Assert.Equal(101, dwarf1.MineId); 

        var dwarf2 = assignments.Single(a => a.DwarfId == 2);
        Assert.Equal(101, dwarf2.MineId); 

        var dwarf3 = assignments.Single(a => a.DwarfId == 3);
        Assert.Equal(102, dwarf3.MineId); 
    }
}