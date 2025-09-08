using KernelOpenApiWeb.Settings;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SimpleAuthentication;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddHttpContextAccessor();

var aiSettings = builder.Services.ConfigureAndGet<AzureOpenAISettings>(builder.Configuration, "AzureOpenAI")!;

builder.Services.AddSimpleAuthentication(builder.Configuration);

builder.Services.AddSingleton<PluginRegistry>();
builder.Services.AddHostedService<OpenApiPluginLoader>();

builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(aiSettings.Deployment, aiSettings.Endpoint, aiSettings.ApiKey);

builder.Services.AddTransient(services =>
{
    var pluginCollection = new KernelPluginCollection(services.GetRequiredService<PluginRegistry>());
    return new Kernel(services, pluginCollection);
});

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

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", app.Environment.ApplicationName);
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/ask", async (Kernel kernel, IChatCompletionService chat, string question) =>
{
    var aiRequestSettings = new AzureOpenAIPromptExecutionSettings
    {
        MaxTokens = 400,
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    // Q&A loop
    var history = new ChatHistory();

    // Add system message with instructions for handling unknown questions
    history.AddSystemMessage("""
        You are an assistant that can ONLY answer by using the available functions. For every user question, you MUST invoke a function to answer. 
        When you identify a function that can answer the user's question but you don't have all the required parameters or inputs needed to call that function, you MUST ask the user to provide the missing information. Be specific about what information is needed and why.
        If there is NO function that allows you to answer the question, you MUST reply that you don't know the answer or cannot provide the requested information and do NOT provide any other information.
        Always prioritize asking for missing parameters over declining to answer, as long as there is a relevant function available.
        """);

    history.AddUserMessage(question);

    var response = await chat.GetChatMessageContentAsync(history, aiRequestSettings, kernel);
    return TypedResults.Ok(new { response = response.ToString() });
});

app.Run();

public class PluginRegistry : List<KernelPlugin>;

public class OpenApiPluginLoader(PluginRegistry registry, IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var kernel = new Kernel();
        var plugin = await kernel.ImportPluginFromOpenApiAsync(pluginName: "weatherforecast",
            uri: new Uri("https://localhost:7219/openapi/v1.json"),
            executionParameters: new()
            {
                EnablePayloadNamespacing = true,
                ServerUrlOverride = new Uri("https://localhost:7219"),
                AuthCallback = (request, cancellationToken) =>
                {
                    var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                    if (httpContext?.Request.Headers.TryGetValue("x-api-key", out var authHeader) == true)
                    {
                        request.Headers.Add("x-api-key", [authHeader.ToString()]);
                    }

                    return Task.CompletedTask;
                }
            }
            , cancellationToken: stoppingToken);

        //var plugin = await kernel.ImportPluginFromOpenApiAsync(pluginName: "weatherforecast",
        //    uri: new Uri("https://localhost:7274/openapi/ai.json"),
        //    executionParameters: new()
        //    {
        //        EnablePayloadNamespacing = true,
        //        ServerUrlOverride = new Uri("https://localhost:7274"),
        //        AuthCallback = (request, cancellationToken) =>
        //        {
        //            var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        //            if (httpContext?.Request.Headers.TryGetValue("x-api-key", out var authHeader) == true)
        //            {
        //                request.Headers.Add("x-api-key", [authHeader.ToString()]);
        //            }

        //            return Task.CompletedTask;
        //        }
        //    }
        //    , cancellationToken: stoppingToken);

        registry.Add(plugin);
    }
}