using System.Text.Json.Serialization;
using Krasnoludki.Core.DTOs;

public class ConvexHullResultDto
{
    [JsonPropertyName("hullPoints")]
    public List<PointDto> HullPoints { get; set; } = new();
}