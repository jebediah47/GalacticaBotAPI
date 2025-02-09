namespace GalacticaBotAPI.Features.Bot.Models;

public enum PresenceType
{
    Playing,
    Streaming,
    Listening,
    Watching,
    Custom,
    Competing,
}

public record BotPresence(string Presence, PresenceType Type);
