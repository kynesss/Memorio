using FluentValidation;

namespace Memorio.Learning.Application.Sessions.Review;

public sealed class ReviewCardCommandValidator : AbstractValidator<ReviewCardCommand>
{
    public ReviewCardCommandValidator()
    {
        RuleFor(command => command.CardId).NotEmpty();
        RuleFor(command => command.Rating).IsInEnum();
        RuleFor(command => command.ReviewDurationMs).GreaterThanOrEqualTo(0);
    }
}
