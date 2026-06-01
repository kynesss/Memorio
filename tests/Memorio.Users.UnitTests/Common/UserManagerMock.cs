using Memorio.Users.Domain;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Memorio.Users.UnitTests.Common;

internal static class UserManagerMock
{
    public static Mock<UserManager<ApplicationUser>> Create()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }
}
