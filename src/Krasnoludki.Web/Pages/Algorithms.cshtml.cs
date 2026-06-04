
using System.Text.Json;
using Krasnoludki.Core;
using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.Dto;
using Krasnoludki.Core.DTOs;
using Krasnoludki.Core.Models;
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
  // public bool HasMinCostResult => ResultsJson.Contains("minCost");

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

        Console.WriteLine($"Loaded scenario: {loaded.Name}, Nodes: {loaded.Nodes.Count}, Results: {ResultsJson}");
        Console.WriteLine($"Parsed Convex Hull: {ParsedResults.ConvexHull?.HullPoints.Count ?? 0} points");
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
    Console.WriteLine("Received matching request with input: " + JsonSerializer.Serialize(input));
    List<Dwarf> dwarves = input.Dwarves;
    List<Mine> mines = input.Mines;

    Console.WriteLine($"Received {dwarves.Count} dwarves and {mines.Count} mines for matching.");

    foreach (var dwarf in dwarves)
    {
      Console.WriteLine($"Dwarf {dwarf.PointId}: Prefers {string.Join(", ", dwarf.PreferredMinerals)}");
    }

    foreach (var mine in mines)
    {
      Console.WriteLine($"Mine {mine.PointId}: Contains {mine.Resource} with capacity {mine.Capacity}");
    }

    List<int[]> assigned = DwarfAssigning.Assign(dwarves, mines);

    var result = new MatchingResultDto
    {
      Assignments = [.. assigned.Select(pair => new DwarfAssignmentDto
      {
        DwarfId = pair[0].ToString(),
        MineId = pair[1].ToString()
      })]
    };

    Console.WriteLine("Matching result: " + JsonSerializer.Serialize(result));

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
    Console.WriteLine("Received segment tree request with input: " + JsonSerializer.Serialize(dwarfesForRmq));
    List<Dwarf> decametrists = dwarfesForRmq;

    Console.WriteLine($"Received {decametrists.Count} decametrists for segment tree.");

    foreach (var dwarf in decametrists)
    {
      Console.WriteLine($"Dwarf {dwarf.PointId}: Prefers {string.Join(", ", dwarf.PreferredMinerals)}");
    }
    var tree = new SegmentTree(decametrists);

    Dwarf loudestDwarf = tree.GetLoudestDecametrist();

    Console.WriteLine("Segment tree result: " + JsonSerializer.Serialize(loudestDwarf));

    var result = new SegmentTreeResultDto
    {
      LoudestDwarfId = loudestDwarf.PointId.ToString()
    };

    Console.WriteLine("Segment tree result: " + JsonSerializer.Serialize(result));

    var id = HttpContext.Session.GetString("activeScenarioId");
    if (id != null && _scenarios.Exists(id))
    {
      _scenarios.SaveResult(id, "segmentTree", result);
    }

    return new JsonResult(new { success = true, data = result });
  }
}