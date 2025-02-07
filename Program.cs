using AspNet.Security.OAuth.Discord;
using FluentValidation;
using GalacticaBotAPI;
using GalacticaBotAPI.Features.Auth.Routes;
using GalacticaBotAPI.Features.Bot.Routes;
using GalacticaBotAPI.Features.Bot.Services;
using GalacticaBotAPI.Features.Bot.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddOpenApi();

Constants.LoadConfiguration(builder.Configuration);

builder.Services.AddHttpClient<GalacticaBotHttpClient>();

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

app.Run();
