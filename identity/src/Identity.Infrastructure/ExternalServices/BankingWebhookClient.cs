using System.Net.Http.Json;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.ExternalServices;

public class BankingWebhookClient : IBankingWebhookClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BankingWebhookClient> _logger;

    public BankingWebhookClient(HttpClient httpClient, IConfiguration configuration, ILogger<BankingWebhookClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(configuration["BankingApi:BaseUrl"] ?? "https://localhost:7001");
    }

    public async Task SendVerificationCallbackAsync(IdentityWebhookPayloadDto payload, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/identity/webhook", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Webhook failed: {StatusCode} - {Body}", response.StatusCode, body);
            throw new InvalidOperationException($"Banking webhook failed with status {response.StatusCode}");
        }

        _logger.LogInformation("Webhook sent successfully for request {RequestId}", payload.RequestId);
    }
}
