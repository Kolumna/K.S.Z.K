using System.Text.Json.Serialization;
using Krasnoludki.Core.Models;

public class MatchingInputDto
{
  [JsonPropertyName("dwarves")]
  public List<Dwarf> Dwarves { get; set; }

  [JsonPropertyName("mines")]
  public List<Mine> Mines { get; set; }
}