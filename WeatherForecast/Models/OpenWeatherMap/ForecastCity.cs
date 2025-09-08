using System.Text.Json.Serialization;

namespace WeatherForecast.Models.OpenWeatherMap;

public class ForecastCity
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("coord")]
    public required Position Position { get; set; }

    [JsonPropertyName("country")]
    public required string Country { get; set; }
}