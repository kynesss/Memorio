using FluentValidation;

namespace Memorio.Flashcards.Application.Decks.CreateDeck;

public sealed class CreateDeckCommandValidator : AbstractValidator<CreateDeckCommand>
{
    public CreateDeckCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(2000);
    }
}
