using AspNet.Security.OAuth.Discord;
using FluentValidation;
using GalacticaBotAPI.Features.Auth.Routes;
using GalacticaBotAPI.Features.Bot.Routes;
using GalacticaBotAPI.Features.Bot.Services;
using GalacticaBotAPI.Features.Bot.Validators;
using GalacticaBotAPI.Features.Shared.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
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
