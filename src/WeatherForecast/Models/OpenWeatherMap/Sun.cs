using System.Text.Json.Serialization;
using WeatherForecast.Models.OpenWeatherMap.Converters;

namespace WeatherForecast.Models.OpenWeatherMap;

public class Sun
{
    [JsonPropertyName("country")]
    public required string Country { get; set; }

    [JsonConverter(typeof(UnixToDateTimeConverter))]
    [JsonPropertyName("sunrise")]
    public DateTime Sunrise { get; set; }

    [JsonConverter(typeof(UnixToDateTimeConverter))]
    [JsonPropertyName("sunset")]
    public DateTime Sunset { get; set; }
}