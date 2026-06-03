
using System.Text.Json.Serialization;

namespace Krasnoludki.Core.Dto;

public class ScenarioResultsDto
{
  [JsonPropertyName("convexHull")]
  public ConvexHullResultDto ConvexHull { get; set; }
  // public MinCostResultDto MinCost { get; set; }
  // public MatchingResultDto Matching { get; set; }
  // public RmqResultDto Rmq { get; set; }
}