
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
      if (scenario != null)
      {
        ActiveScenarioName = scenario.Name;
        SelectedScenarioId = scenario.Id;
        var json = _scenarios.Load(id);
        NodesJson = json;

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