using AwesomeAssertions;
using ErrorOr;
using FluentValidation;
using MediatR;
using Memorio.Shared.Behaviors;
using Xunit;

namespace Memorio.Users.UnitTests.Application.Behaviors;

public sealed class ValidationBehaviorTests
{
    public sealed record SampleRequest(string Name) : IRequest<ErrorOr<string>>;

    private sealed class SampleRequestValidator : AbstractValidator<SampleRequest>
    {
        public SampleRequestValidator() => RuleFor(request => request.Name).NotEmpty();
    }

    [Fact]
    public async Task Handle_InvokesNext_WhenRequestIsValid()
    {
        var behavior = new ValidationBehavior<SampleRequest, ErrorOr<string>>([new SampleRequestValidator()]);

        var result = await behavior.Handle(
            new SampleRequest("valid"),
            _ => Task.FromResult<ErrorOr<string>>("handled"),
            CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().Be("handled");
    }

    [Fact]
    public async Task Handle_ReturnsValidationErrors_WhenRequestIsInvalid()
    {
        var behavior = new ValidationBehavior<SampleRequest, ErrorOr<string>>([new SampleRequestValidator()]);

        var result = await behavior.Handle(
            new SampleRequest(string.Empty),
            _ => Task.FromResult<ErrorOr<string>>("handled"),
            CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(error =>
            error.Type == ErrorType.Validation && error.Code == nameof(SampleRequest.Name));
    }
}
