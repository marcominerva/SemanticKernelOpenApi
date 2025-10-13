using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Northwind.Data;
using SimpleAuthentication;
using TinyHelpers.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSimpleAuthentication(builder.Configuration);

builder.Services.AddSqlServer<NorthwindContext>(builder.Configuration.GetConnectionString("SqlConnection"), options =>
{
    options.EnableRetryOnFailure();
}, options =>
{
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDefaultExceptionHandler();
builder.Services.AddDefaultProblemDetails();

builder.Services.AddOpenApi(options =>
{
    options.AddSimpleAuthentication(builder.Configuration);

    options.ShouldInclude = _ => true;
});

builder.Services.AddOpenApi("ai", options =>
{
    options.AddSimpleAuthentication(builder.Configuration);

    options.ShouldInclude = description => description.GroupName == "ai";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapOpenApi().AllowAnonymous();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Default API v1");
    options.SwaggerEndpoint("/openapi/ai.json", "AI API");
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
