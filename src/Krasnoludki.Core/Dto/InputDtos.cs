using System.Text.Json.Serialization;
using Krasnoludki.Core.Models;

public class MatchingInputDto
{
  [JsonPropertyName("dwarves")]
  public List<Dwarf> Dwarves { get; set; } = new();

  [JsonPropertyName("mines")]
  public List<Mine> Mines { get; set; } = new();
}

public class MinCostRequest
{
  [JsonPropertyName("dwarves")]
  public List<Dwarf> Dwarves { get; set; } = new();

  [JsonPropertyName("mines")]
  public List<Mine> Mines { get; set; } = new();
}

public class SegmentTreeInputDto
{
  [JsonPropertyName("dwarves")]
  public List<Dwarf> Dwarves { get; set; } = new();
}