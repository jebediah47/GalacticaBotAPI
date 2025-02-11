using System.Security.Claims;
using GalacticaBotAPI.Features.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalacticaBotAPI.Features.Admin.Routes;

public static class Admin
{
    private static string? GetAuthenticatedUserId(HttpContext context)
    {
        return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;
    }

    public static void MapAdminEndpoints(this WebApplication app)
    {
        var admin = app.MapGroup("admin");

        admin.MapPost(
            "appoint",
            async (
                [FromQuery] string newAdminId,
                AdminManagementService adminService,
                HttpContext context
            ) =>
            {
                var authenticatedUserId = GetAuthenticatedUserId(context);
                if (authenticatedUserId == null)
                    return Results.Unauthorized();

                var result = await adminService.AppointAdministratorAsync(
                    authenticatedUserId,
                    newAdminId
                );

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
                [FromQuery] string adminId,
                AdminManagementService adminService,
                HttpContext context
            ) =>
            {
                var authenticatedUserId = GetAuthenticatedUserId(context);
                if (authenticatedUserId == null)
                    return Results.Unauthorized();

                var result = await adminService.RemoveAdministratorAsync(
                    authenticatedUserId,
                    adminId
                );

                if (!result)
                {
                    return Results.BadRequest(
                        "Failed to remove administrator. Ensure you are the bot owner and the target user is an admin."
                    );
                }

                return Results.Ok("Administrator removed successfully.");
            }
        );
    }
}
