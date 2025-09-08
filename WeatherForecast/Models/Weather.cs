using System.ComponentModel;
using WeatherForecast.Models.OpenWeatherMap;

namespace WeatherForecast.Models;

public class Weather(CurrentWeather weather)
{
    public string CityName { get; set; } = weather.Name;

    [Description("A single word that describe the condition")]
    public string Condition { get; set; } = weather.Conditions.First().Condition;

    public string ConditionIcon { get; set; } = weather.Conditions.First().ConditionIcon;

    public string ConditionIconUrl => $"https://openweathermap.org/img/w/{ConditionIcon}.png";

    [Description("A brief description of the condition")]
    public string ConditionDescription { get; set; } = weather.Conditions.First().Description;

    [Description("The current temperature, in Celsius degrees")]
    public decimal Temperature { get; set; } = weather.Detail.Temperature;
}
