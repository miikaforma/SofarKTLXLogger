using System.Text.Json.Serialization;

namespace SofarKTLXLogger.Daytime.Models;

public record Response
{
    [JsonPropertyName("results")]
    public required Result Result { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }
}