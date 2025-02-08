namespace GalacticaBotAPI.Features.Shared.DTO;

public record DiscordTeamMember(
    int MembershipState,
    List<string> Permissions,
    string TeamId,
    DiscordUser User
);
