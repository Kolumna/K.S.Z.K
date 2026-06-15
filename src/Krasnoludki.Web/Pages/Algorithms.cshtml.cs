
using System.Diagnostics;
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

  public string? RunningAlgos => HttpContext.Session.GetString("runningAlgos");

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
  public async Task<IActionResult> OnPostCalculateGraham([FromBody] List<PointDto> incomingPoints)
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

      var (hull, ms) = Timed(() => ConvexHullSolver.GrahamScan(corePoints));

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
        ExecutionTimeMs = ms
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
        executionTimeMs = ms.ToString("F2")
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
    finally
    {
      var running = GetRunningAlgos();
      if (running.Contains("convexHull"))
      {
        running.Remove("convexHull");
        HttpContext.Session.SetString("runningAlgos", JsonSerializer.Serialize(running));
        await HttpContext.Session.CommitAsync();
      }
    }
  }

  /// <summary>
  /// Handler obsługujący żądanie typu POST dla /Algorithms?handler=CalculateMatching
  /// </summary>
  public async Task<IActionResult> OnPostCalculateMatching(
    [FromBody] MatchingInputDto input
  )
  {
    try
    {
      if (input == null || input.Dwarves == null || input.Mines == null)
      {
        return BadRequest(new { success = false, message = "Błędne dane wejściowe." });
      }

      List<Dwarf> dwarves = input.Dwarves;
      List<Mine> mines = input.Mines;

      var (assigned, ms) = Timed(() => DwarfAssigning.Assign(dwarves, mines));

      var result = new MatchingResultDto
      {
        Assignments = [.. assigned.Select(pair => new DwarfAssignmentDto
      {
        DwarfId = pair[0].ToString(),
        MineId = pair[1].ToString()
      })],
        ExecutionTimeMs = ms
      };

      var id = HttpContext.Session.GetString("activeScenarioId");
      if (id != null && _scenarios.Exists(id))
      {
        _scenarios.SaveResult(id, "matching", result);
      }

      return new JsonResult(new { success = true, data = result, executionTimeMs = ms });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { success = false, message = ex.Message });
    }
    finally
    {
      var running = GetRunningAlgos();
      if (running.Contains("matching"))
      {
        running.Remove("matching");
        HttpContext.Session.SetString("runningAlgos", JsonSerializer.Serialize(running));
        await HttpContext.Session.CommitAsync();
      }
    }
  }

  /// <summary>
  /// Handler obsługujący żądanie typu POST dla /Algorithms?handler=CalculateSegmentTree
  /// </summary>
  public async Task<IActionResult> OnPostCalculateSegmentTree(
   [FromBody] SegmentTreeInputDto input
 )
  {
    try
    {
      if (input == null || input.Dwarves == null)
      {
        return BadRequest(new { success = false, message = "Błędne dane wejściowe." });
      }
      List<Dwarf> decametrists = input.Dwarves;

      if (input == null)
      {
        return BadRequest(new { success = false, message = "Nieprawidłowe dane wejściowe." });
      }

      var (tree, segmentTreeMs) = Timed(() => new SegmentTree(decametrists));

      var (loudestDwarf, getLoudestMs) = Timed(() => tree.GetLoudestDecametrist());

      var result = new SegmentTreeResultDto
      {
        LoudestDwarfId = loudestDwarf.PointId.ToString(),
        ExecutionTimeMs = segmentTreeMs + getLoudestMs
      };

      var id = HttpContext.Session.GetString("activeScenarioId");
      if (id != null && _scenarios.Exists(id))
      {
        _scenarios.SaveResult(id, "segmentTree", result);
      }

      return new JsonResult(new { success = true, data = result });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { success = false, message = ex.Message });
    }
    finally
    {
      var running = GetRunningAlgos();
      if (running.Contains("segmentTree"))
      {
        running.Remove("segmentTree");
        HttpContext.Session.SetString("runningAlgos", JsonSerializer.Serialize(running));
        await HttpContext.Session.CommitAsync();
      }
    }
  }

  /// <summary>
  /// Handler obsługujący żądanie typu POST dla /Algorithms?handler=CalculateMinCost
  /// </summary>
  public async Task<IActionResult> OnPostCalculateMinCost([FromBody] MinCostRequest request)
  {
    try
    {
      var mapper = new McmfMapper();
      var dwarves = mapper.MapDwarves(request.Dwarves);
      var mines = mapper.MapMines(request.Mines);

      var (network, networkMs) = Timed(() => new ResidualNetwork(dwarves, mines));
      var mcmf = new MinCostMaxFlowProblem();

      var ((minCost, maxFlow), minCostMs) = Timed(() => mcmf.MinCostMaxFlow(network));
      var ((assignments, employedCount), employedCountMs) = Timed(() => mcmf.ExtractAssignments(network));


      var result = new MinCostResultDto
      {
        RealCost = Math.Round(minCost, 2),
        MaxFlow = maxFlow,
        EmployedCount = employedCount,
        Assignments = assignments,
        ExecutionTimeMs = networkMs + minCostMs + employedCountMs
      };


      var id = HttpContext.Session.GetString("activeScenarioId");
      if (id != null && _scenarios.Exists(id))
      {
        _scenarios.SaveResult(id, "minCost", result);
      }

      return new JsonResult(new { success = true, data = result, executionTimeMs = networkMs + minCostMs + employedCountMs });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { success = false, message = ex.Message });
    }
  }

  /// <summary>
  /// Handler obsługujący żądanie typu POST dla /Algorithms?handler=SetRunningAlgo
  /// </summary>
  public async Task<IActionResult> OnPostSetRunningAlgo([FromBody] SetRunningAlgoDto dto)
  {
    var running = GetRunningAlgos();

    if (string.IsNullOrEmpty(dto.AlgorithmType))
      return new JsonResult(new { success = false });

    if (dto.IsRunning)
    {
      running.Add(dto.AlgorithmType);
    }
    else
    {
      running.Remove(dto.AlgorithmType);
    }

    HttpContext.Session.SetString("runningAlgos", JsonSerializer.Serialize(running));
    await HttpContext.Session.CommitAsync(); // <-- Wymuś zapis

    return new JsonResult(new { success = true });
  }

  private HashSet<string> GetRunningAlgos()
  {
    var json = HttpContext.Session.GetString("runningAlgos");
    return string.IsNullOrEmpty(json)
        ? new HashSet<string>()
        : JsonSerializer.Deserialize<HashSet<string>>(json) ?? new HashSet<string>();
  }

  public string RunningAlgosJson => JsonSerializer.Serialize(GetRunningAlgos());

  private (T result, double ms) Timed<T>(Func<T> fn)
  {
    var sw = Stopwatch.StartNew();
    var result = fn();
    sw.Stop();
    return (result, sw.Elapsed.TotalMilliseconds);
  }
}

