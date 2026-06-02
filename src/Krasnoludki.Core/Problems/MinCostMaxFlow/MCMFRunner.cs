using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Krasnoludki.Core.Problems;
using System.Diagnostics;

namespace Krasnoludki.Core.Routing;

public record AssignmentDto(int DwarfId, int MineId, double Distance);

public class RoutingResultDto
{
    public List<AssignmentDto> ReadyEdgesWithIdDistance { get; set; }
    public double MinCostResult { get; set; }
    public double MaxFlowResult { get; set; }
    public int EmployedOnlyByDistance { get; set; }
    public int UnemployedDwarvesCount{ get; set; }

    public long TimeInMilisecons { get; set; }

    public RoutingResultDto(List<AssignmentDto> edges, double cost, double flow,int unemployedDwarvesCount,
         int distanceEmployedDwarfs, long totalTime)
    {
        ReadyEdgesWithIdDistance = edges;

        MinCostResult = cost;
        MaxFlowResult = flow;

        UnemployedDwarvesCount = unemployedDwarvesCount;
        EmployedOnlyByDistance = distanceEmployedDwarfs;

        TimeInMilisecons = totalTime;
    }
}

public class MCMFRunner
{   
    public RoutingResultDto MCMFRun(List<Dwarf> dwarves, List<Mine> mines)
    {
        if (dwarves is null || mines is null || dwarves.Count == 0 || mines.Count == 0) 
        {
            return new RoutingResultDto(new List<AssignmentDto>(), 0, 0, 0, 0, 0);
        }

        Stopwatch timer = new Stopwatch();
        timer.Start();
        ResidualNetwork network = new ResidualNetwork(dwarves,mines);
        MinCostMaxFlowProblem mcmf = new MinCostMaxFlowProblem();

        var (_, maxFlow) = mcmf.MinCostMaxFlow(network);
        var (readyEdges, realCost, employedCount, distanceEmployedCount) = mcmf.ExtractAssignments(network);
        timer.Stop();
        
        int unemployedDwarvesCount = dwarves.Count - employedCount;

        return new RoutingResultDto(readyEdges, realCost, maxFlow, unemployedDwarvesCount,
             distanceEmployedCount, timer.ElapsedMilliseconds);

    }
    
}