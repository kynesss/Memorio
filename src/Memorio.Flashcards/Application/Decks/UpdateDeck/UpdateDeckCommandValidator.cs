using FluentValidation;

namespace Memorio.Flashcards.Application.Decks.UpdateDeck;

public sealed class UpdateDeckCommandValidator : AbstractValidator<UpdateDeckCommand>
{
    public UpdateDeckCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(2000);
    }
}
