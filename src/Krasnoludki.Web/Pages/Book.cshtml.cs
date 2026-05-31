
using System.Text.Json;
using Krasnoludki.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Krasnoludki.Web.Pages;

[IgnoreAntiforgeryToken]
public class BookModel : PageModel
{
  [BindProperty(SupportsGet = true)]
  public string? SelectedScenarioId { get; set; }

  [BindProperty(SupportsGet = true)]
  public string? Q { get; set; }

  public string ActiveScenarioName { get; set; } = "Nowa mapa";

  public string NodesJson { get; set; } = "[]";
  public string ResultsJson { get; set; } = "{}";

  private readonly ScenarioFileService _scenarios;

  public BookModel(ScenarioFileService scenarios)
  {
    _scenarios = scenarios;
  }

  public IEnumerable<dynamic> SavedScenarios { get; set; } = new List<dynamic>();

  public void OnGet()
  {
    SavedScenarios = _scenarios.GetManifest();

    var id = HttpContext.Session.GetString("activeScenarioId");
    if (id == null)
    {
      Reset();
      return;
    }

    if (!_scenarios.Exists(id))
    {
      HttpContext.Session.Remove("activeScenarioId");
      Reset();
      return;
    }

    var entry = SavedScenarios.FirstOrDefault(m => m.Id == id);
    if (entry == null)
    {
      HttpContext.Session.Remove("activeScenarioId");
      Reset();
      return;
    }

    var scenarioData = _scenarios.Load(id);
    ActiveScenarioName = scenarioData.Name;
    SelectedScenarioId = id;
    NodesJson = JsonSerializer.Serialize(scenarioData.Nodes);
    ResultsJson = JsonSerializer.Serialize(scenarioData.Results);
  }

  private void Reset()
  {
    ActiveScenarioName = "Nowa mapa";
    NodesJson = "[]";
    ResultsJson = "{}";
    SelectedScenarioId = null;
  }

  public IActionResult OnPostLoadScenario(string id)
  {
    if (!string.IsNullOrEmpty(id) && _scenarios.Exists(id))
    {
      HttpContext.Session.SetString("activeScenarioId", id);
    }

    return RedirectToPage();
  }

  public IActionResult OnPostDeleteScenario(string id)
  {
    Console.WriteLine($"Request to delete scenario with id: {id}");
    if (!string.IsNullOrEmpty(id) && _scenarios.Exists(id))
    {
      _scenarios.Delete(id);
      var currentSessionId = HttpContext.Session.GetString("activeScenarioId");
      if (currentSessionId == id)
      {
        HttpContext.Session.Remove("activeScenarioId");
      }
    }

    return RedirectToPage();
  }
}