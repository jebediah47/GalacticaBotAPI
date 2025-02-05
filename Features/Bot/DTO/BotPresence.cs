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

public record BotPresence(string presence, PresenceType presence_type);
