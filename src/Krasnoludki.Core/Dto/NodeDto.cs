using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Krasnoludki.Core.DTOs;

public class NodeDto
{
    [JsonPropertyName("pointId")]
    public int Id { get; set; }

    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("preferredMinerals")]
    public List<string> Minerals { get; set; } = new();

    [JsonPropertyName("resource")]
    public string Resource { get; set; }

    [JsonPropertyName("capacity")]
    public int? Capacity { get; set; }
    [JsonPropertyName("voiceLoudness")]
    public int? Loudness { get; set; }
}