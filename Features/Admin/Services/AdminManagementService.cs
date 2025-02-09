using GalacticaBotAPI.Features.Admin.Data;
using GalacticaBotAPI.Features.Admin.Models;
using GalacticaBotAPI.Features.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace GalacticaBotAPI.Features.Admin.Services;

public class AdminManagementService
{
    private readonly AdminDbContext _context;
    private readonly DiscordApiBotHttpClient _discordApiBotHttpClient;

    public AdminManagementService(
        AdminDbContext context,
        DiscordApiBotHttpClient discordApiBotHttpClient
    )
    {
        _context = context;
        _discordApiBotHttpClient = discordApiBotHttpClient;
    }

    public async Task<bool> IsUserAuthorizedAsync(string userId)
    {
        return await _context.AdminUsers.AnyAsync(u => u.UserId == userId);
    }

    private async Task<bool> IsBotOwnerAsync(string userId)
    {
        var user = await _context.AdminUsers.FirstOrDefaultAsync(u => u.UserId == userId);

        return user?.Role == UserRole.BotOwner;
    }

    public async Task<bool> AppointAdministratorAsync(string botOwnerId, string newAdminId)
    {
        // Check if the database table is empty
        if (!await _context.AdminUsers.AnyAsync(u => u.UserId == botOwnerId))
        {
            // Check if the bot owner is the one making the request
            if (botOwnerId == await _discordApiBotHttpClient.GetBotOwner())
            {
                // Create the bot owner
                var botOwner = new AdminUser
                {
                    UserId = botOwnerId,
                    Role = UserRole.BotOwner,
                    IsProfileComplete = false,
                };

                await _context.AdminUsers.AddAsync(botOwner);
                await _context.SaveChangesAsync();
            }
            else
            {
                return false;
            }
        }
        // Verify the appointer is the bot owner
        if (!await IsBotOwnerAsync(botOwnerId))
            return false;

        // Check if user already exists
        if (await _context.AdminUsers.AnyAsync(u => u.UserId == newAdminId))
            return false;

        // Create new admin entry
        var newAdmin = new AdminUser
        {
            UserId = newAdminId,
            Role = UserRole.Admin,
            IsProfileComplete = false,
        };

        await _context.AdminUsers.AddAsync(newAdmin);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveAdministratorAsync(string botOwnerId, string adminIdToRemove)
    {
        // Verify the remover is the bot owner
        if (!await IsBotOwnerAsync(botOwnerId))
            return false;

        // Find the admin to remove
        var adminToRemove = await _context.AdminUsers.FirstOrDefaultAsync(u =>
            u.UserId == adminIdToRemove
        );

        // Ensure we're not removing the bot owner and the user exists
        if (adminToRemove == null || adminToRemove.Role == UserRole.BotOwner)
            return false;

        _context.AdminUsers.Remove(adminToRemove);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CompleteUserProfileAsync(
        string userId,
        string username,
        string email,
        string avatarHash
    )
    {
        var user = await _context.AdminUsers.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return false;

        user.Username = username;
        user.Email = email;
        user.AvatarHash = avatarHash;
        user.IsProfileComplete = true;
        user.LastLogin = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}
