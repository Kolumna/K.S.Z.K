using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Krasnoludki.Core.Problems.Huffman;
using System.Text.Json.Serialization;
using Krasnoludki.Web.Services;

namespace Krasnoludki.Web.Pages
{
  [IgnoreAntiforgeryToken]
  public class MapModel : PageModel
  {
    private readonly ScenarioFileService _scenarios;

    public MapModel(ScenarioFileService scenarios)
    {
      _scenarios = scenarios;
    }

    [BindProperty(SupportsGet = true)]
    public string? SelectedScenarioId { get; set; }

    public string ActiveScenarioName { get; set; } = "Nowa mapa";

    public string NodesJson { get; set; } = "[]";

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

    public IActionResult OnGetNew()
    {
      HttpContext.Session.Remove("activeScenarioId");
      return RedirectToPage("/Map");
    }

    /// <summary>
    /// Handler obsługujący żądanie typu POST dla /Map?handler=SaveHoffApi
    /// </summary>
    public IActionResult OnPostSaveHoffApi([FromBody] SaveRequest request)
    {
      if (request == null || string.IsNullOrEmpty(request.NodesJson))
        return BadRequest("Brak danych");

      var name = $"Scenariusz {DateTime.Now:dd.MM.yyyy HH:mm}";

      var id = _scenarios.Save(request.NodesJson, name);

      HttpContext.Session.SetString("activeScenarioId", id);

      var bytes = _scenarios.GetRawBytes(id);
      return File(bytes, "application/octet-stream", $"{id}.hoff");
    }


    public IActionResult OnPostUpdateHoffApi([FromBody] SaveRequest request)
    {
      if (request == null || string.IsNullOrEmpty(request.NodesJson))
        return BadRequest("Brak danych");

      var name = $"Scenariusz {DateTime.Now:dd.MM.yyyy HH:mm}";

      var id = request.ScenarioId;
      _scenarios.Update(id, request.NodesJson, name);

      HttpContext.Session.SetString("activeScenarioId", id);

      var bytes = _scenarios.GetRawBytes(id);
      return File(bytes, "application/octet-stream", $"{id}.hoff");
    }
  }
  public class SaveRequest
  {
    [JsonPropertyName("scenarioId")]
    public string ScenarioId { get; set; }

    [JsonPropertyName("nodes")]
    public string NodesJson { get; set; }
  }
}