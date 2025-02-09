namespace GalacticaBotAPI.Features.Admin.Models;

public enum UserRole
{
    BotOwner,
    Admin,
}

public class AdminUser
{
    public string UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? AvatarHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsProfileComplete { get; set; }
}
