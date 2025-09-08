using System.Text.Json.Serialization;

namespace WeatherForecast.Models.OpenWeatherMap;

public class Wind
{
    [JsonPropertyName("speed")]
    public decimal Speed { get; set; }

    [JsonPropertyName("deg")]
    public double Degree { get; set; }
}