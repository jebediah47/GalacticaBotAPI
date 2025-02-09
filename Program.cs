using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using FluentValidation;
using GalacticaBotAPI.Features.Admin.Data;
using GalacticaBotAPI.Features.Admin.Routes;
using GalacticaBotAPI.Features.Admin.Services;
using GalacticaBotAPI.Features.Auth.Routes;
using GalacticaBotAPI.Features.Bot.Routes;
using GalacticaBotAPI.Features.Bot.Services;
using GalacticaBotAPI.Features.Bot.Validators;
using GalacticaBotAPI.Features.Shared.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Constants = GalacticaBotAPI.Constants;

var builder = WebApplication.CreateSlimBuilder(args);

Constants.LoadConfiguration(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AdminDbContext>(options =>
{
    options.UseNpgsql(
        Constants.DbConnectionString,
        npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)
    );
});
builder.Services.AddScoped<AdminManagementService>();
builder.Services.AddHttpClient<GalacticaBotHttpClient>();
builder.Services.AddHttpClient<DiscordApiBotHttpClient>();

builder.Services.AddValidatorsFromAssemblyContaining<BotPresenceValidator>();

builder
    .Services.AddAuthentication(options =>
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
                var botOwnerId = await discordApiClient.GetBotOwner();

                var userIdClaim = context.Principal.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier
                );

                if (userIdClaim == null)
                {
                    context.Fail("Invalid user data received from Discord.");
                    context.HandleResponse();
                    context.Response.Redirect("/auth/denied");
                    return;
                }

                // Check if user is the bot owner
                if (userIdClaim.Value == botOwnerId)
                {
                    // If this is the bot owner's first login, add them to the database
                    if (!await adminService.IsUserAuthorizedAsync(botOwnerId))
                    {
                        await adminService.AppointAdministratorAsync(botOwnerId, botOwnerId); // Self-appointment for bot owner
                    }

                    // Get user details from claims
                    var username = context
                        .Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)
                        ?.Value;
                    var email = context
                        .Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                        ?.Value;
                    var avatarHash = context
                        .Principal.Claims.FirstOrDefault(c => c.Type == "urn:discord:avatar:hash")
                        ?.Value;

                    // Complete the profile
                    await adminService.CompleteUserProfileAsync(
                        botOwnerId,
                        username!,
                        email!,
                        avatarHash ?? string.Empty
                    );
                    return;
                }

                // If not bot owner, check if user is an authorized admin
                if (!await adminService.IsUserAuthorizedAsync(userIdClaim.Value))
                {
                    context.Fail("Access denied: User is not authorized.");
                    context.HandleResponse();
                    context.Response.Redirect("/auth/denied");
                    return;
                }

                // Update admin's profile information
                var adminUsername = context
                    .Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)
                    ?.Value;
                var adminEmail = context
                    .Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                    ?.Value;
                var adminAvatarHash = context
                    .Principal.Claims.FirstOrDefault(c => c.Type == "urn:discord:avatar:hash")
                    ?.Value;

                await adminService.CompleteUserProfileAsync(
                    userIdClaim.Value,
                    adminUsername!,
                    adminEmail!,
                    adminAvatarHash ?? string.Empty
                );
            },
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapAdminEndpoints();
app.MapAuthEndpoints();
app.MapBotEndpoints();

app.MapGet("/", (HttpContext ctx) => ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList());

app.MapGet(
    "/bot-owner",
    async (DiscordApiBotHttpClient discordApiBotHttpClient) =>
    {
        var botOwner = await discordApiBotHttpClient.GetBotOwner();
        return Results.Ok(botOwner);
    }
);

app.Run();
