using GalacticaBotAPI.Features.Admin.Models;
using GalacticaBotAPI.Features.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalacticaBotAPI.Features.Admin.Routes;

public static class Admin
{
    public static RouteGroupBuilder MapAdminEndpoints(this WebApplication app)
    {
        var admin = app.MapGroup("admin");

        admin.MapPost(
            "appoint",
            async (
                [FromQuery] string botOwnerId,
                [FromQuery] string newAdminId,
                AdminManagementService adminService
            ) =>
            {
                var result = await adminService.AppointAdministratorAsync(botOwnerId, newAdminId);

                if (!result)
                {
                    return Results.BadRequest(
                        "Failed to appoint administrator. Ensure you are the bot owner and the user isn't already an admin."
                    );
                }

                return Results.Ok("Administrator appointed successfully.");
            }
        );

        admin.MapDelete(
            "/remove",
            async (
                [FromQuery] string botOwnerId,
                [FromQuery] string adminId,
                AdminManagementService adminService
            ) =>
            {
                var result = await adminService.RemoveAdministratorAsync(botOwnerId, adminId);

                if (!result)
                {
                    return Results.BadRequest(
                        "Failed to remove administrator. Ensure you are the bot owner and the target user is an admin."
                    );
                }

                return Results.Ok("Administrator removed successfully.");
            }
        );

        admin.MapPost(
            "/complete-profile",
            async (
                [FromQuery] string userId,
                [FromBody] ProfileUpdateRequest request,
                AdminManagementService adminService
            ) =>
            {
                var result = await adminService.CompleteUserProfileAsync(
                    userId,
                    request.Username,
                    request.Email,
                    request.AvatarHash
                );

                if (!result)
                {
                    return Results.BadRequest(
                        "Failed to update profile. User may not exist in the database."
                    );
                }

                return Results.Ok("Profile updated successfully.");
            }
        );

        return admin;
    }
}
