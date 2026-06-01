using AwesomeAssertions;
using Memorio.Shared.Exceptions;
using Memorio.Users.Application.Auth.GetCurrentUser;
using Memorio.Users.Domain;
using Memorio.Users.UnitTests.Common;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Memorio.Users.UnitTests.Application.Auth;

public sealed class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager = UserManagerMock.Create();

    private GetCurrentUserQueryHandler CreateHandler() => new(_userManager.Object);

    [Fact]
    public async Task Handle_ReturnsUser_WhenUserExists()
    {
        var user = new ApplicationUser { Email = "user@memorio.test" };
        _userManager.Setup(manager => manager.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);

        var result = await CreateHandler().Handle(new GetCurrentUserQuery(user.Id), CancellationToken.None);

        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorized_WhenUserDoesNotExist()
    {
        _userManager.Setup(manager => manager.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

        var act = () => CreateHandler().Handle(new GetCurrentUserQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
