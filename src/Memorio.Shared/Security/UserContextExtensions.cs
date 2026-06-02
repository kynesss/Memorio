using Memorio.Shared.Exceptions;

namespace Memorio.Shared.Security;

public static class UserContextExtensions
{
    public static Guid RequireUserId(this IUserContext userContext) =>
        userContext.UserId ?? throw new UnauthorizedException();
}
