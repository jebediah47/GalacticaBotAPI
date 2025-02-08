using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using FluentValidation;
using GalacticaBotAPI.Features.Auth.Routes;
using GalacticaBotAPI.Features.Bot.Routes;
using GalacticaBotAPI.Features.Bot.Services;
using GalacticaBotAPI.Features.Bot.Validators;
using GalacticaBotAPI.Features.Shared.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Scalar.AspNetCore;
using Constants = GalacticaBotAPI.Constants;

var builder = WebApplication.CreateSlimBuilder(args);

Constants.LoadConfiguration(builder.Configuration);

builder.Services.AddOpenApi();
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
                var botOwnerId = await discordApiClient.GetBotOwner();

                var userIdClaim = context.Principal.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier
                );

                if (userIdClaim == null || userIdClaim.Value != botOwnerId)
                {
                    context.Fail("Access denied: only the bot owner is permitted to log in.");
                    context.HandleResponse();
                    context.Response.Redirect("/auth/denied");
                }
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
