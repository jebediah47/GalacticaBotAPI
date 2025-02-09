using FluentValidation;
using GalacticaBotAPI.Features.Admin.Data;
using GalacticaBotAPI.Features.Admin.Routes;
using GalacticaBotAPI.Features.Admin.Services;
using GalacticaBotAPI.Features.Auth.Routes;
using GalacticaBotAPI.Features.Auth.Services;
using GalacticaBotAPI.Features.Bot.Routes;
using GalacticaBotAPI.Features.Bot.Services;
using GalacticaBotAPI.Features.Bot.Validators;
using GalacticaBotAPI.Features.Shared.Services;
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

builder.Services.ConfigureAuthentication();
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
