using AwesomeAssertions;
using FluentValidation;
using MediatR;
using Memorio.Users.Application.Behaviors;
using Xunit;
using ValidationException = Memorio.Shared.Exceptions.ValidationException;

namespace Memorio.Users.UnitTests.Application.Behaviors;

public sealed class ValidationBehaviorTests
{
    public sealed record SampleRequest(string Name) : IRequest<string>;

    private sealed class SampleRequestValidator : AbstractValidator<SampleRequest>
    {
        public SampleRequestValidator() => RuleFor(request => request.Name).NotEmpty();
    }

    [Fact]
    public async Task Handle_InvokesNext_WhenRequestIsValid()
    {
        var behavior = new ValidationBehavior<SampleRequest, string>([new SampleRequestValidator()]);

        var result = await behavior.Handle(new SampleRequest("valid"), _ => Task.FromResult("handled"), CancellationToken.None);

        result.Should().Be("handled");
    }

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenRequestIsInvalid()
    {
        var behavior = new ValidationBehavior<SampleRequest, string>([new SampleRequestValidator()]);

        var act = () => behavior.Handle(new SampleRequest(string.Empty), _ => Task.FromResult("handled"), CancellationToken.None);

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().ContainKey(nameof(SampleRequest.Name));
    }
}
