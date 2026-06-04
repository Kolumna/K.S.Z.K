using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Krasnoludki.Core.DTOs;

public class ScenarioDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("nodes")]
    public List<NodeDto> Nodes { get; set; } = new();

    [JsonPropertyName("results")]
    public ScenarioResults Results { get; set; } = new();
}

public class ScenarioResults
{
    [JsonPropertyName("convexHull")]
    public ConvexHullResultDto? ConvexHull { get; set; }
}