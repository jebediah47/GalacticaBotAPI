using FluentValidation;
using GalacticaBotAPI.Features.Bot.Routes;
using GalacticaBotAPI.Features.Bot.Services;
using GalacticaBotAPI.Features.Bot.Validators;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient<GalacticaBotHttpClient>();

builder.Services.AddValidatorsFromAssemblyContaining<BotPresenceValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapBotEndpoints();

app.Run();
