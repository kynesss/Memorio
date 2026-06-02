using FluentValidation;

namespace Memorio.Flashcards.Application.Cards.CreateCard;

public sealed class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
{
    public CreateCardCommandValidator()
    {
        RuleFor(command => command.Front)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(command => command.Back)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(command => command.Tags)
            .MaximumLength(500);

        RuleFor(command => command.Type)
            .IsInEnum();
    }
}
