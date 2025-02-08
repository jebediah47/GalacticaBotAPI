using System.Net.Http.Headers;
using System.Text.Json;
using GalacticaBotAPI.Features.Shared.DTO;

namespace GalacticaBotAPI.Features.Shared.Services;

public sealed class DiscordApiBotHttpClient
{
    private readonly HttpClient _client;
    
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions 
    { 
        PropertyNameCaseInsensitive = true 
    };

    public DiscordApiBotHttpClient(HttpClient httpClient)
    {
        _client = httpClient;
        _client.BaseAddress = new Uri(Constants.DiscordApiBaseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bot",
            Constants.GalacticaBotToken
        );
    }

    public async Task<string> GetBotOwner()
    {
        var response = await _client.GetFromJsonAsync<DiscordApplication>(
            "applications/@me",
            JsonOptions
        );

        return response?.Owner.Id
            ?? throw new InvalidOperationException("Failed to retrieve bot owner.");
    }
}
