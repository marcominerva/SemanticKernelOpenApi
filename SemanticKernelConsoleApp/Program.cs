using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernelConsoleApp;

var builder = Kernel.CreateBuilder();

builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddAzureOpenAIChatCompletion(Constants.Model, Constants.Endpoint, Constants.ApiKey);

var kernel = builder.Build();

await kernel.ImportPluginFromOpenApiAsync(
    pluginName: "weatherforecast",
    uri: new Uri("https://localhost:7219/openapi/v1.json"),
    executionParameters: new()
    {
        EnablePayloadNamespacing = true,
        ServerUrlOverride = new Uri("https://localhost:7219")
    }
);

var aiRequestSettings = new AzureOpenAIPromptExecutionSettings
{
    MaxTokens = 400,
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// Q&A loop
var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();

// Add system message with instructions for handling unknown questions
history.AddSystemMessage("""
    You are an assistant that can ONLY answer by using the available functions. For every user question, you MUST invoke a function to answer. 
    When you identify a function that can answer the user's question but you don't have all the required parameters or inputs needed to call that function, you MUST ask the user to provide the missing information. Be specific about what information is needed and why.
    If there is NO function that allows you to answer the question, you MUST reply that you don't know the answer or cannot provide the requested information and do NOT provide any other information.
    Always prioritize asking for missing parameters over declining to answer, as long as there is a relevant function available.
    """);

while (true)
{
    Console.Write("Question: ");

    var question = Console.ReadLine();
    history.AddUserMessage(question);

    var answer = new StringBuilder();
    await foreach (var token in chat.GetStreamingChatMessageContentsAsync(history, aiRequestSettings, kernel))
    {
        Console.Write(token);
        answer.Append(token);

        await Task.Delay(10);
    }

    history.AddAssistantMessage(answer.ToString());

    Console.WriteLine();
    Console.WriteLine();
}