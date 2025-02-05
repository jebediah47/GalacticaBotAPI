using FluentValidation;
using GalacticaBotAPI.Features.Bot.DTO;

namespace GalacticaBotAPI.Features.Bot.Validators;

public sealed class BotPresenceValidator : AbstractValidator<BotPresence>
{
    public BotPresenceValidator()
    {
        RuleFor(x => x.presence)
            .NotEmpty()
            .WithMessage("Presence is required.")
            .MaximumLength(50)
            .NotEmpty()
            .WithMessage("Presence must not exceed 50 characters.");

        RuleFor(x => x.presence_type)
            .IsInEnum()
            .WithMessage("Presence type must be a valid enum value.");
    }
}
