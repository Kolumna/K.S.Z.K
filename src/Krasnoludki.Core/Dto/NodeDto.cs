using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Krasnoludki.Core.DTOs;

public class NodeDto
{
    [JsonPropertyName("_id")]
    public int Id { get; set; }
    
    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("minerals")]
    public List<string> Minerals { get; set; } = new();

    [JsonPropertyName("capacity")]
    public int? Capacity { get; set; }
}