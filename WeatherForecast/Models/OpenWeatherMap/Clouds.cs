using System.Text.Json.Serialization;

namespace WeatherForecast.Models.OpenWeatherMap;

public class Clouds
{
    [JsonPropertyName("all")]
    public int Cloudiness { get; set; }
}