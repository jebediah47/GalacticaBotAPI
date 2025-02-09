namespace GalacticaBotAPI.Features.Shared.Models;

public record DiscordTeamMember(
    int MembershipState,
    List<string> Permissions,
    string TeamId,
    DiscordUser User
);
