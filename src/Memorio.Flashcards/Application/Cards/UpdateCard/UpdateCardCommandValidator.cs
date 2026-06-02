using FluentValidation;

namespace Memorio.Flashcards.Application.Cards.UpdateCard;

public sealed class UpdateCardCommandValidator : AbstractValidator<UpdateCardCommand>
{
    public UpdateCardCommandValidator()
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
