using Krasnoludki.Core.Models;
using Krasnoludki.Core.Routing; 
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Krasnoludki.Test;

/// <summary>
/// Black-box testing of the MCMF facade (MCMFRunner).
/// Validates the end-to-end pipeline: accepting raw frontend domain models, 
/// processing the flow network, and returning accurately mapped AssignmentDto 
/// results without exposing internal graph complexities.
/// </summary>
public class McmfRunnerTest
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
    public void Runner_ShouldExecuteFullPipelineAndReturnAssignments()
    {
        var dwarves = GetMcmfIntegrationDwarves();
        var mines = GetMcmfIntegrationMines();
        var runner = new MCMFRunner(); 

        var result = runner.MCMFRun(dwarves, mines);

        // 1. Validate core metrics: Flow completion, total cost, and penalty application
        Assert.Equal(6, dwarves.Count - result.UnemployedDwarvesCount);
        Assert.Equal(88.19804, result.MinCostResult, 5);
        Assert.Equal(1, result.EmployedOnlyByDistance);

        // 2. Validate DTO structure output
        Assert.NotNull(result.ReadyEdgesWithIdDistance);
        Assert.Equal(6, result.ReadyEdgesWithIdDistance.Count);

        // 3. Validate black-box optimization: Dwarf 3 should be pushed to the fallback mine (102)
        var dwarf3Assignment = result.ReadyEdgesWithIdDistance.Single(a => a.DwarfId == 3);
        Assert.Equal(102, dwarf3Assignment.MineId);
    }
}