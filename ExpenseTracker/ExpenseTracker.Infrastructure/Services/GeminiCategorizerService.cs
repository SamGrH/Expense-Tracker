using ExpenseTracker.Application.Common.Dtos;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace ExpenseTracker.Infrastructure.Services;

public class GeminiCategorizerService : IAiCategorizerService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiOptions _options;

    public GeminiCategorizerService(HttpClient httpClient, IOptions<GeminiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("Gemini:ApiKey no está configurada en User Secrets.");
        }
    }

    public async Task<List<ClassifiedExpenseDto>> CategorizeExpensesAsync(
        List<string> descriptions,
        CancellationToken cancellationToken = default)
    {
        if (descriptions == null || descriptions.Count == 0)
            return new List<ClassifiedExpenseDto>();

        // 1. Usamos el modelo y la clave directo de las options
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_options.Model}:generateContent?key={_options.ApiKey}";

        // 2. Usamos las instrucciones que viven en las options
        var requestBody = new
        {
            system_instruction = new
            {
                parts = new[]
                {
                    new { text = _options.SystemInstruction }
                }
            },
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = $"Clasifica la siguiente lista de consumos:\n{JsonSerializer.Serialize(descriptions)}" }
                    }
                }
            },
            generationConfig = new
            {
                response_mime_type = "application/json"
            }
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(url, jsonContent, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        using var doc = JsonDocument.Parse(responseString);
        var root = doc.RootElement;

        var aiTextResponse = root
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(aiTextResponse))
            return new List<ClassifiedExpenseDto>();

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var classifiedExpenses = JsonSerializer.Deserialize<List<ClassifiedExpenseDto>>(aiTextResponse, jsonOptions);

        return classifiedExpenses ?? new List<ClassifiedExpenseDto>();
    }
}