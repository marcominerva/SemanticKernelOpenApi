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

var aiRequestSettings = new AzureOpenAIPromptExecutionSettings
{
    MaxTokens = 400,
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// Q&A loop
var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();

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