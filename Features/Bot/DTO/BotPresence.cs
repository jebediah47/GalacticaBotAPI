using System.ComponentModel.DataAnnotations;

namespace GalacticaBotAPI.Features.Bot.DTO;

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
