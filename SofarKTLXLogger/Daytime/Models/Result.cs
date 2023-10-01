using System.Text.Json.Serialization;

namespace SofarKTLXLogger.Daytime.Models;

public record Result
{
    [JsonPropertyName("sunrise")]
    public required DateTimeOffset Sunrise { get; set; }
    
    [JsonPropertyName("sunset")]
    public required DateTimeOffset Sunset { get; set; }
    
    [JsonPropertyName("solar_noon")]
    public required DateTimeOffset SolarNoon { get; set; }
    
    [JsonPropertyName("day_length")]
    public int DayLength { get; set; }
    
    [JsonPropertyName("civil_twilight_begin")]
    public required DateTimeOffset CivilTwilightBegin { get; set; }
    
    [JsonPropertyName("civil_twilight_end")]
    public required DateTimeOffset CivilTwilightEnd { get; set; }
    
    [JsonPropertyName("nautical_twilight_begin")]
    public required DateTimeOffset NauticalTwilightBegin { get; set; }
    
    [JsonPropertyName("nautical_twilight_end")]
    public required DateTimeOffset NauticalTwilightEnd { get; set; }
    
    [JsonPropertyName("astronomical_twilight_begin")]
    public required DateTimeOffset AstronomicalTwilightBegin { get; set; }
    
    [JsonPropertyName("astronomical_twilight_end")]
    public required DateTimeOffset AstronomicalTwilightEnd { get; set; }
}