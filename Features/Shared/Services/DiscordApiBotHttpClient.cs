using System.Net.Http.Headers;
using System.Text.Json;
using GalacticaBotAPI.Features.Shared.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GalacticaBotAPI.Features.Shared.Services;

public sealed class DiscordApiBotHttpClient
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private const string BotOwnerCacheKey = "DiscordBotOwnerId";

    public DiscordApiBotHttpClient(HttpClient httpClient, IMemoryCache cache)
    {
        _client = httpClient;
        _client.BaseAddress = new Uri(Constants.DiscordApiBaseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bot",
            Constants.GalacticaBotToken
        );
        _cache = cache;
    }

    public async Task<string> GetBotOwner()
    {
        if (_cache.TryGetValue(BotOwnerCacheKey, out string cachedOwner))
        {
            return cachedOwner;
        }

        var response = await _client.GetFromJsonAsync<DiscordApplication>(
            "applications/@me",
            JsonOptions
        );

        var ownerId =
            response?.Owner.Id
            ?? throw new InvalidOperationException("Failed to retrieve bot owner.");

        _cache.Set(BotOwnerCacheKey, ownerId, TimeSpan.FromMinutes(15));

        return ownerId;
    }
}
