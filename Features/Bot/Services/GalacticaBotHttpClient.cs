using GalacticaBotAPI.Features.Bot.DTO;

namespace GalacticaBotAPI.Features.Bot.Services;

public sealed class GalacticaBotHttpClient
{
    private readonly HttpClient _client;

    public GalacticaBotHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _client = httpClient;
        var apiBaseUrl = Constants.GalacticaBotApiBaseUrl;
        _client.BaseAddress = new Uri(apiBaseUrl);
    }

    public async Task<BotPresence> GetBotStatus()
    {
        var botStatus = await _client.GetFromJsonAsync<BotPresence>("config/presence");
        if (botStatus == null)
        {
            throw new InvalidOperationException("Failed to retrieve bot status.");
        }
        return botStatus;
    }

    public async Task<BotPresence> ChangeBotStatus(BotPresence botStatus)
    {
        var response = await _client.PostAsJsonAsync("config/presence", botStatus);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to change bot status.");
        }
        return botStatus;
    }
}
