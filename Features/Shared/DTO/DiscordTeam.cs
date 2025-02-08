namespace GalacticaBotAPI.Features.Shared.DTO;

public record DiscordTeam(string Id, string Name, string Icon, List<DiscordTeamMember> Members);
