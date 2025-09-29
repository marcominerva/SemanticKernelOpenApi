using System.ComponentModel;
using System.Security.Claims;
using SimpleAuthentication;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.OpenApi;
using WeatherForecast.Models;
using WeatherForecast.Models.OpenWeatherMap;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<WeatherService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AppSettings:OpenWeatherMapBaseUrl")!);
});

builder.Services.AddSimpleAuthentication(builder.Configuration);

builder.Services.AddOpenApi(options =>
{
    options.RemoveServerList();
    options.AddDefaultProblemDetailsResponse();

    options.AddSimpleAuthentication(builder.Configuration);
});

builder.Services.AddDefaultProblemDetails();
builder.Services.AddDefaultExceptionHandler();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapOpenApi().AllowAnonymous();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", app.Environment.ApplicationName);
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/weather/current", async (ClaimsPrincipal user, [Description("The city for which to get the current weather condition")] string city, WeatherService weatherService, CancellationToken cancellationToken) =>
{
    var weather = await weatherService.GetCurrentWeatherAsync(city, cancellationToken);
    var response = new Weather(weather);

    return TypedResults.Ok(response);
})
.RequireAuthorization()
.WithSummary("Get the current weather condition. This is the method to get weather of today");

app.MapGet("/api/weather/daily", async ([Description("The city for which to get the weather forecast for the next days.")] string city,
    [Description("The number of days for which to return the forecast (from 1 to 16)")] int days,
    WeatherService weatherService, CancellationToken cancellationToken) =>
{
    var weather = await weatherService.GetWeatherForecastAsync(city, days, cancellationToken);
    return TypedResults.Ok(weather);
})
.WithSummary("Get the weather condition for the next days. If you want to get the current condition, you should call the /api/weather/current endpoint");

app.Run();

public class WeatherService(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<CurrentWeather> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<CurrentWeather>($"weather?q={city}&appid={configuration.GetValue<string>("AppSettings:OpenWeatherMapAppId")}&units=metric", cancellationToken);
        return response!;
    }

    public async Task<DailyForecastWeather> GetWeatherForecastAsync(string city, int days, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<DailyForecastWeather>($"forecast/daily?q={city}&cnt={Math.Clamp(days, 1, 16)}&appid={configuration.GetValue<string>("AppSettings:OpenWeatherMapAppId")}&units=metric", cancellationToken);
        return response!;
    }
}