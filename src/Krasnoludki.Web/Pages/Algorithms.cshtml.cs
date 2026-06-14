
using System.Text.Json;
using Krasnoludki.Core;
using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.Dto;
using Krasnoludki.Core.DTOs;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.McmfAlgorithm.Adapter;
using Krasnoludki.Core.Models;
using Krasnoludki.Core.Problems;
using Krasnoludki.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Krasnoludki.Web.Pages;

[IgnoreAntiforgeryToken]
public class AlgorithmsModel : PageModel
{
  [BindProperty(SupportsGet = true)]
  public string? SelectedScenarioId { get; set; }

  [BindProperty(SupportsGet = true)]
  public string? Algorithm { get; set; }

  public string ActiveScenarioName { get; set; } = "Nowa mapa";

  public string NodesJson { get; set; } = "[]";
  public string ResultsJson { get; set; } = "{}";

  public ScenarioResultsDto ParsedResults { get; set; } = new ScenarioResultsDto();

  public bool HasMatchingResult => ParsedResults.Matching != null;
  public bool HasConvexHullResult => ParsedResults?.ConvexHull != null;
  public bool HasMinCostResult => ParsedResults?.MinCost != null;

  public bool HasRmqResult => ParsedResults?.Rmq != null;

  private readonly ScenarioFileService _scenarios;

  public AlgorithmsModel(ScenarioFileService scenarios)
  {
    _scenarios = scenarios;
  }

  public void OnGet()
  {
    var id = HttpContext.Session.GetString("activeScenarioId");
    if (id != null)
    {
      if (!_scenarios.Exists(id))
      {
        HttpContext.Session.Remove("activeScenarioId");
        ActiveScenarioName = "Nowa mapa";
        SelectedScenarioId = null;
        NodesJson = "[]";
        return;
      }

      var manifest = _scenarios.GetManifest();
      var scenario = manifest.FirstOrDefault(m => m.Id == id);
      if (_scenarios.Exists(id))
      {
        var loaded = _scenarios.Load(id);
        ActiveScenarioName = loaded.Name;
        SelectedScenarioId = id;
        NodesJson = JsonSerializer.Serialize(loaded.Nodes);
        ResultsJson = JsonSerializer.Serialize(loaded.Results);

        if (!string.IsNullOrWhiteSpace(ResultsJson) && ResultsJson != "{}")
        {
          ParsedResults = JsonSerializer.Deserialize<ScenarioResultsDto>(ResultsJson) ?? new ScenarioResultsDto();
        }

        return;
      }
      else
      {
        HttpContext.Session.Remove("activeScenarioId");
      }
    }

    ActiveScenarioName = "Nowa mapa";
    NodesJson = "[]";
    SelectedScenarioId = null;
  }

  /// <summary>
  /// Handler obsługujący żądanie typu POST dla /Algorithms?handler=CalculateGraham
  /// </summary>
  public IActionResult OnPostCalculateGraham([FromBody] List<PointDto> incomingPoints)
  {
    if (incomingPoints == null || incomingPoints.Count < 3)
      return BadRequest(new { success = false, message = "Zbyt mało punktów." });

    try
    {
      var corePoints = incomingPoints
          .Select(p => new Core.Point(
              (long)Math.Round(p.X),
              (long)Math.Round(p.Y)))
          .ToList();

      var hull = ConvexHullSolver.GrahamScan(corePoints);

      var hullPoints = hull
          .Select(p => new PointDto { X = p.X, Y = p.Y })
          .ToList();

      double perimeter = 0;
      for (int i = 0; i < hullPoints.Count; i++)
      {
        var a = hullPoints[i];
        var b = hullPoints[(i + 1) % hullPoints.Count];
        perimeter += Math.Sqrt(
            Math.Pow(a.X - b.X, 2) +
            Math.Pow(a.Y - b.Y, 2));
      }

      var result = new ConvexHullResultDto
      {
        HullPoints = [.. hullPoints.Select(p => new PointDto
        {
          X = p.X,
          Y = p.Y
        })],
      };

      var id = HttpContext.Session.GetString("activeScenarioId");
      if (id != null && _scenarios.Exists(id))
      {
        _scenarios.SaveResult(id, "convexHull", result);
      }

      return new JsonResult(new
      {
        success = true,
        data = hullPoints.Select(p => new { x = p.X, y = p.Y }),
        perimeter,
      });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new
      {
        success = false,
        message = "Error: " + ex.Message
      });
    }
  }

  public IActionResult OnPostCalculateMatching(
    [FromBody] MatchingInputDto input
  )
  {
    List<Dwarf> dwarves = input.Dwarves;
    List<Mine> mines = input.Mines;

    Console.WriteLine("Otrzymane dane do matching:");
    Console.WriteLine("Dwarves:");
    foreach (var d in dwarves)
    {
      Console.WriteLine($"- Id: {d.PointId}, Loudness: {d.VoiceLoudness}, x: {d.x}, y: {d.y}");
    }
    Console.WriteLine("Mines:");
    foreach (var m in mines)
    {
      Console.WriteLine($"- Id: {m.PointId}, Capacity: {m.Capacity}");
    }

    List<int[]> assigned = DwarfAssigning.Assign(dwarves, mines);

    Console.WriteLine("Assigned pairs:");
    foreach (var pair in assigned)    {
      Console.WriteLine($"Dwarf {pair[0]} -> Mine {pair[1]}");
    }

    var result = new MatchingResultDto
    {
      Assignments = [.. assigned.Select(pair => new DwarfAssignmentDto
      {
        DwarfId = pair[0].ToString(),
        MineId = pair[1].ToString()
      })]
    };

    var id = HttpContext.Session.GetString("activeScenarioId");
    if (id != null && _scenarios.Exists(id))
    {
      _scenarios.SaveResult(id, "matching", result);
    }

    return new JsonResult(new { success = true, data = result });
  }

  public IActionResult OnPostCalculateSegmentTree(
   [FromBody] List<Dwarf> dwarfesForRmq
 )
  {
    List<Dwarf> decametrists = dwarfesForRmq;

    var tree = new SegmentTree(decametrists);

    Dwarf loudestDwarf = tree.GetLoudestDecametrist();

    var result = new SegmentTreeResultDto
    {
      LoudestDwarfId = loudestDwarf.PointId.ToString()
    };

    var id = HttpContext.Session.GetString("activeScenarioId");
    if (id != null && _scenarios.Exists(id))
    {
      _scenarios.SaveResult(id, "segmentTree", result);
    }

    return new JsonResult(new { success = true, data = result });
  }

  public IActionResult OnPostCalculateMinCost([FromBody] MinCostRequest request)
  {
    try
    {
      var mapper = new McmfMapper();
      var dwarves = mapper.MapDwarves(request.Dwarves);
      var mines = mapper.MapMines(request.Mines);

      var network = new ResidualNetwork(dwarves, mines);
      var mcmf = new MinCostMaxFlowProblem();

      var (minCost, maxFlow) = mcmf.MinCostMaxFlow(network);
      var (assignments, employedCount) = mcmf.ExtractAssignments(network);


      var result = new MinCostResultDto
      {
          RealCost = Math.Round(minCost, 2),
          MaxFlow = maxFlow,
          EmployedCount = employedCount,
          Assignments = assignments
      };


      var id = HttpContext.Session.GetString("activeScenarioId");
      if (id != null && _scenarios.Exists(id))
        _scenarios.SaveResult(id, "minCost", result);

      return new JsonResult(new { success = true, data = result });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { success = false, message = ex.Message });
    }
  }
}