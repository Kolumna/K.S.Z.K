using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.DTOs;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Krasnoludki.Core.Problems;
using System.Diagnostics;

namespace Krasnoludki.Core.Routing;

public record AssignmentDto(int DwarfId, int MineId, double Distance);

public class RoutingResultDto
{
    public List<AssignmentDto> ReadyEdgesWithIdDistance { get; set; } = new List<AssignmentDto>();
    public double MinCostResult { get; set; }
    public double MaxFlowResult { get; set; }

    public long TimeInMilisecons { get; set; }

    public RoutingResultDto(List<AssignmentDto> edges, double cost, double flow, long total_time)
    {
        ReadyEdgesWithIdDistance = edges;
        MinCostResult = cost;
        MaxFlowResult = flow;
        TimeInMilisecons = total_time;
    }
}

public class RoutingEngine
{   
    public MineralType ConvertToMineralType(string mineral)
    {
            return mineral switch 
            { 
                "gold" => MineralType.Gold, 
                "coal" => MineralType.Coal,
                "quartz" => MineralType.Quartz,
                "silver" => MineralType.Silver,
                "none" => MineralType.None,
                _ => MineralType.None 
            };
    }

    public RoutingResultDto ProcessRouting(List<NodeDto> frontendData)
    {
        if (frontendData is null) 
        {
            return new RoutingResultDto(new List<AssignmentDto>(), 0, 0, 0);
        }

        List<Dwarf> dwarves = new List<Dwarf>();
        List<Mine> mines = new List<Mine>();

        foreach(var nodeDTO in frontendData)
        {
            if(nodeDTO.Type == "dwarf")
            {
                Dwarf new_dwarf = new Dwarf(nodeDTO.Id, nodeDTO.X, nodeDTO.Y,
                    (nodeDTO.Minerals ?? new List<string>()).Select(m => ConvertToMineralType(m)).ToList(), 0);
                
                dwarves.Add(new_dwarf);
            }
            else if(nodeDTO.Type == "mine")
            {
                Mine new_mine = new Mine(nodeDTO.Id, nodeDTO.X, nodeDTO.Y,
                    ConvertToMineralType(nodeDTO.Minerals?.FirstOrDefault()), nodeDTO.Capacity ?? 0);
                    
                mines.Add(new_mine);
            }
        }

        if (dwarves.Count == 0 || mines.Count == 0) 
        {
            return new RoutingResultDto(new List<AssignmentDto>(), 0, 0, 0);
        }

        Stopwatch timer = new Stopwatch();
        timer.Start();
        ResidualNetwork network = new ResidualNetwork(dwarves,mines);
        MinCostMaxFlowProblem mcmf = new MinCostMaxFlowProblem();


        var mcmfresult = mcmf.MinCostMaxFlow(network);
        var DataForFrontend = mcmf.ExtractAssignments(network);
        timer.Stop();
        

        return new RoutingResultDto(DataForFrontend, mcmfresult.Item1, mcmfresult.Item2, timer.ElapsedMilliseconds);

    }
    
}