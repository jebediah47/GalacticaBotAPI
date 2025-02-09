using FluentValidation;
using GalacticaBotAPI.Features.Bot.Models;
using GalacticaBotAPI.Features.Bot.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalacticaBotAPI.Features.Bot.Routes;

public static class Bot
{
    public static RouteGroupBuilder MapBotEndpoints(this WebApplication app)
    {
        var bot = app.MapGroup("bot");

        bot.MapGet(
            "presence",
            async (GalacticaBotHttpClient botHttpClient) => await botHttpClient.GetBotStatus()
        );

        bot.MapPost(
            "presence",
            async (
                GalacticaBotHttpClient botHttpClient,
                [FromBody] BotPresence botPresence,
                IValidator<BotPresence> validator
            ) =>
            {
                var validationResult = await validator.ValidateAsync(botPresence);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = await botHttpClient.ChangeBotStatus(botPresence);
                return Results.Ok(result);
            }
        );

        return bot;
    }
}
