using System.Text.Json.Serialization;

namespace Krasnoludki.Core.DTOs;

public class PointDto
{
    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonPropertyName("index")]
    public int? Index { get; set; }
}