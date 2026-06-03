
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
}