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

app.MapGet("/api/weather/current", async (string city, WeatherService weatherService, CancellationToken cancellationToken) =>
{
    var weather = await weatherService.GetCurrentWeatherAsync(city, cancellationToken);
    var response = new Weather(weather);

    return TypedResults.Ok(response);
});

app.MapGet("/api/weather/daily", async (string city,
    int days,
    WeatherService weatherService, CancellationToken cancellationToken) =>
{
    var weather = await weatherService.GetWeatherForecastAsync(city, days, cancellationToken);
    return TypedResults.Ok(weather);
});

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