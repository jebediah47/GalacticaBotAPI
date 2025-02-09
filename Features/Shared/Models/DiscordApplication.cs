namespace GalacticaBotAPI.Features.Shared.Models;

public record DiscordApplication(
    string Id,
    string Name,
    string Icon,
    string Description,
    List<string> RpcOrigins,
    bool BotPublic,
    bool BotRequireCodeGrant,
    string TermsOfServiceUrl,
    string PrivacyPolicyUrl,
    DiscordUser Owner,
    string Summary,
    string VerifyKey,
    DiscordTeam Team,
    string GuildId,
    string PrimarySkuId,
    string Slug,
    string CoverImage,
    int Flags
);
