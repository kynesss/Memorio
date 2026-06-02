using AwesomeAssertions;
using ErrorOr;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Auth.Register;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Memorio.Users.UnitTests.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace Memorio.Users.UnitTests.Application.Auth;

public sealed class RegisterUserCommandHandlerTests
{
    private static readonly AuthResponse ExpectedTokens = new("access", "refresh", "Bearer", 900);

    private readonly Mock<UserManager<ApplicationUser>> _userManager = UserManagerMock.Create();
    private readonly Mock<IAuthTokenIssuer> _tokenIssuer = new();
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 6, 1, 10, 0, 0, TimeSpan.Zero));

    private RegisterUserCommandHandler CreateHandler() => new(_userManager.Object, _tokenIssuer.Object, _clock);

    [Fact]
    public async Task Handle_ReturnsTokens_WhenCreationSucceeds()
    {
        _userManager
            .Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _tokenIssuer
            .Setup(issuer => issuer.IssueAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExpectedTokens);

        var result = await CreateHandler().Handle(new RegisterUserCommand("user@memorio.test", "Sup3rSecret!"), CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeSameAs(ExpectedTokens);
    }

    [Fact]
    public async Task Handle_ReturnsConflict_WhenEmailIsDuplicated()
    {
        var identityError = new IdentityError { Code = "DuplicateEmail", Description = "Email already taken." };
        _userManager
            .Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        var result = await CreateHandler().Handle(new RegisterUserCommand("user@memorio.test", "Sup3rSecret!"), CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Conflict);
        result.FirstError.Code.Should().Be("DuplicateEmail");
        _tokenIssuer.Verify(issuer => issuer.IssueAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
