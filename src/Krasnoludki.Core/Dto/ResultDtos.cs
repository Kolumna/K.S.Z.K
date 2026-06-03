using System.Text.Json.Serialization;
using Krasnoludki.Core.DTOs;

public class ConvexHullResultDto
{
    [JsonPropertyName("hullPoints")]
    public List<PointDto> HullPoints { get; set; } = new();
}

public class MatchingResultDto
{
    [JsonPropertyName("assignments")]
    public List<DwarfAssignmentDto> Assignments { get; set; } = new();
}

public class MineAssignmentDto
{
    [JsonPropertyName("fromDwarfId")]
    public string FromDwarfId { get; set; }

    [JsonPropertyName("toMineId")]
    public string ToMineId { get; set; }

    [JsonPropertyName("allocatedCapacity")]
    public int AllocatedCapacity { get; set; }
}

public class DwarfAssignmentDto
{
    [JsonPropertyName("dwarfId")]
    public string DwarfId { get; set; }

    [JsonPropertyName("mineId")]
    public string MineId { get; set; }
}