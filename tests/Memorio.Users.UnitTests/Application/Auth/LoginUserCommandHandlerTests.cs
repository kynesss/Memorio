using AwesomeAssertions;
using Memorio.Shared.Exceptions;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Auth.Login;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Memorio.Users.UnitTests.Common;
using Moq;
using Xunit;

namespace Memorio.Users.UnitTests.Application.Auth;

public sealed class LoginUserCommandHandlerTests
{
    private static readonly AuthResponse ExpectedTokens = new("access", "refresh", "Bearer", 900);

    private readonly Mock<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>> _userManager = UserManagerMock.Create();
    private readonly Mock<IAuthTokenIssuer> _tokenIssuer = new();

    private LoginUserCommandHandler CreateHandler() => new(_userManager.Object, _tokenIssuer.Object);

    [Fact]
    public async Task Handle_ReturnsTokens_WhenCredentialsAreValid()
    {
        var user = new ApplicationUser { Email = "user@memorio.test" };
        _userManager.Setup(manager => manager.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManager.Setup(manager => manager.CheckPasswordAsync(user, "correct")).ReturnsAsync(true);
        _tokenIssuer.Setup(issuer => issuer.IssueAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync(ExpectedTokens);

        var result = await CreateHandler().Handle(new LoginUserCommand(user.Email!, "correct"), CancellationToken.None);

        result.Should().BeSameAs(ExpectedTokens);
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorized_WhenUserDoesNotExist()
    {
        _userManager.Setup(manager => manager.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

        var act = () => CreateHandler().Handle(new LoginUserCommand("missing@memorio.test", "any"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
        _tokenIssuer.Verify(issuer => issuer.IssueAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorized_WhenPasswordIsInvalid()
    {
        var user = new ApplicationUser { Email = "user@memorio.test" };
        _userManager.Setup(manager => manager.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManager.Setup(manager => manager.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(false);

        var act = () => CreateHandler().Handle(new LoginUserCommand(user.Email!, "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
