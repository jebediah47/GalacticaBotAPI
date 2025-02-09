namespace GalacticaBotAPI.Features.Shared.Models;

public record DiscordTeam(string Id, string Name, string Icon, List<DiscordTeamMember> Members);
