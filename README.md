# Semantic Kernel with OpenAPI

A sample repository that shows how to use OpenAPI specification as Semantic Kernel functions.

## Prerequisites

### Azure OpenAI

This repository uses Azure OpenAI. You need to configure the following settings in the projects that use it:

- **Endpoint**: Your Azure OpenAI endpoint URL
- **Model**: The deployment name of your model
- **API Key**: Your Azure OpenAI API key

You can use a different model if you prefer, as long as it supports **Tool Calling**.

You can create an Azure OpenAI resource in the [Azure AI Foundry portal](https://ai.azure.com/).

### OpenWeatherMap API Key

To use the WeatherForecast functionality, you need an API key that can be obtained for free from [OpenWeatherMap](https://openweathermap.org/).

### Northwind Database

To use the Northwind database, you need to download and install it from the [SQL Server Samples repository](https://github.com/Microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs).
