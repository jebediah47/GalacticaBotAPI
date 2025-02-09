using GalacticaBotAPI.Features.Admin.Data;
using GalacticaBotAPI.Features.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace GalacticaBotAPI.Features.Admin.Services;

public class AdminManagementService
{
    private readonly AdminDbContext _context;

    public AdminManagementService(AdminDbContext context)
    {
        _context = context;
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

        await _context.SaveChangesAsync();

        return true;
    }
}
