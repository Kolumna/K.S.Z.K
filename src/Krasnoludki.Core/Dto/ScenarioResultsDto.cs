
using System.Text.Json.Serialization;

namespace Krasnoludki.Core.Dto;

public class ScenarioResultsDto
{
  [JsonPropertyName("matching")]
  public MatchingResultDto? Matching { get; set; }

  [JsonPropertyName("convexHull")]
  public ConvexHullResultDto? ConvexHull { get; set; }

  [JsonPropertyName("segmentTree")]
  public SegmentTreeResultDto? Rmq { get; set; }
  
  [JsonPropertyName("minCost")]
  public MinCostResultDto? MinCost { get; set; }
}