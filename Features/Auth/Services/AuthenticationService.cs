using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using GalacticaBotAPI.Features.Admin.Services;
using GalacticaBotAPI.Features.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace GalacticaBotAPI.Features.Auth.Services;

public static class AuthenticationService
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(
                "Cookies",
                options =>
                {
                    options.LoginPath = "/auth/login";
                    options.LogoutPath = "/auth/logout";
                }
            )
            .AddDiscord(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClientId = Constants.DiscordClientId;
                options.ClientSecret = Constants.DiscordClientSecret;
                options.CallbackPath = "/auth/callback";
                options.Scope.Add("email");
                options.Scope.Add("guilds");
                options.Events = new OAuthEvents
                {
                    OnTicketReceived = async context =>
                    {
                        var discordApiClient =
                            context.HttpContext.RequestServices.GetRequiredService<DiscordApiBotHttpClient>();
                        var adminService =
                            context.HttpContext.RequestServices.GetRequiredService<AdminManagementService>();
                        try
                        {
                            var botOwnerId = await discordApiClient.GetBotOwner();

                            var userIdClaim = context.Principal.Claims.FirstOrDefault(c =>
                                c.Type == ClaimTypes.NameIdentifier
                            );

                            if (userIdClaim == null)
                            {
                                await HandleAuthFailure(
                                    context,
                                    "Invalid user data received from Discord."
                                );
                                return;
                            }

                            // Extract user details from claims
                            var userDetails = ExtractUserDetails(context.Principal);

                            // Handle bot owner authentication
                            if (userIdClaim.Value == botOwnerId)
                            {
                                await HandleBotOwnerAuth(adminService, botOwnerId, userDetails);
                                return;
                            }

                            // Handle regular admin authentication
                            if (
                                !await HandleAdminAuth(adminService, userIdClaim.Value, userDetails)
                            )
                            {
                                await HandleAuthFailure(
                                    context,
                                    "Access denied: User is not authorized."
                                );
                            }
                        }
                        catch (Exception e)
                        {
                            await HandleAuthFailure(context, e.Message);
                        }
                    },
                    OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/auth/denied");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },
                };
            });
        return services;
    }

    private static Task HandleAuthFailure(TicketReceivedContext context, string message)
    {
        context.Fail(message);
        context.HandleResponse();
        context.Response.Redirect("/auth/denied");
        return Task.CompletedTask;
    }

    private static (string username, string email, string avatarHash) ExtractUserDetails(
        ClaimsPrincipal? principal
    )
    {
        if (principal == null)
            return (string.Empty, string.Empty, string.Empty);

        var username =
            principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        var email =
            principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
        var avatarHash =
            principal.Claims.FirstOrDefault(c => c.Type == "urn:discord:avatar:hash")?.Value
            ?? string.Empty;

        return (username, email, avatarHash);
    }

    private static async Task HandleBotOwnerAuth(
        AdminManagementService adminService,
        string botOwnerId,
        (string username, string email, string avatarHash) userDetails
    )
    {
        // If this is the bot owner's first login, add them to the database
        if (!await adminService.IsUserAuthorizedAsync(botOwnerId))
        {
            await adminService.AppointAdministratorAsync(botOwnerId, botOwnerId);
        }

        // Update the bot owner's profile
        await adminService.CompleteUserProfileAsync(
            botOwnerId,
            userDetails.username,
            userDetails.email,
            userDetails.avatarHash
        );
    }

    private static async Task<bool> HandleAdminAuth(
        AdminManagementService adminService,
        string userId,
        (string username, string email, string avatarHash) userDetails
    )
    {
        // Check if user is an authorized admin
        if (!await adminService.IsUserAuthorizedAsync(userId))
        {
            return false;
        }

        // Update admin's profile information
        await adminService.CompleteUserProfileAsync(
            userId,
            userDetails.username,
            userDetails.email,
            userDetails.avatarHash
        );

        return true;
    }
}
