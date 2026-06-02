using AwesomeAssertions;
using ErrorOr;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Auth.Refresh;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Memorio.Users.UnitTests.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace Memorio.Users.UnitTests.Application.Auth;

public sealed class RefreshTokenCommandHandlerTests
{
    private static readonly AuthResponse ExpectedTokens = new("access", "refresh", "Bearer", 900);

    private readonly Mock<IRefreshTokenStore> _refreshTokenStore = new();
    private readonly Mock<UserManager<ApplicationUser>> _userManager = UserManagerMock.Create();
    private readonly Mock<IAuthTokenIssuer> _tokenIssuer = new();
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 6, 1, 10, 0, 0, TimeSpan.Zero));

    private RefreshTokenCommandHandler CreateHandler() =>
        new(_refreshTokenStore.Object, _userManager.Object, _tokenIssuer.Object, _clock);

    [Fact]
    public async Task Handle_RotatesToken_WhenRefreshTokenIsActive()
    {
        var userId = Guid.NewGuid();
        var storedToken = RefreshToken.Issue(userId, TimeSpan.FromDays(7), _clock);
        var user = new ApplicationUser { Email = "user@memorio.test" };

        _refreshTokenStore.Setup(store => store.FindAsync(storedToken.Token, It.IsAny<CancellationToken>())).ReturnsAsync(storedToken);
        _userManager.Setup(manager => manager.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _tokenIssuer.Setup(issuer => issuer.IssueAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync(ExpectedTokens);

        var result = await CreateHandler().Handle(new RefreshTokenCommand(storedToken.Token), CancellationToken.None);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeSameAs(ExpectedTokens);
        storedToken.IsActive(_clock).Should().BeFalse("the consumed refresh token must be revoked during rotation");
    }

    [Fact]
    public async Task Handle_ReturnsUnauthorized_WhenTokenIsUnknown()
    {
        _refreshTokenStore.Setup(store => store.FindAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((RefreshToken?)null);

        var result = await CreateHandler().Handle(new RefreshTokenCommand("unknown"), CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Handle_ReturnsUnauthorized_WhenTokenIsAlreadyRevoked()
    {
        var storedToken = RefreshToken.Issue(Guid.NewGuid(), TimeSpan.FromDays(7), _clock);
        storedToken.Revoke(_clock);
        _refreshTokenStore.Setup(store => store.FindAsync(storedToken.Token, It.IsAny<CancellationToken>())).ReturnsAsync(storedToken);

        var result = await CreateHandler().Handle(new RefreshTokenCommand(storedToken.Token), CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unauthorized);
        _tokenIssuer.Verify(issuer => issuer.IssueAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
