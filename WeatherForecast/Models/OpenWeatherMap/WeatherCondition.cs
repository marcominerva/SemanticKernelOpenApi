using System.Text.Json.Serialization;

namespace WeatherForecast.Models.OpenWeatherMap;

public class WeatherCondition
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public required string Condition { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("icon")]
    public required string ConditionIcon { get; set; }
}