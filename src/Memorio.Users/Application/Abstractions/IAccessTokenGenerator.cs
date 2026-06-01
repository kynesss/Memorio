using Memorio.Users.Domain;

namespace Memorio.Users.Application.Abstractions;

public interface IAccessTokenGenerator
{
    AccessToken Generate(ApplicationUser user);
}

public sealed record AccessToken(string Value, DateTime ExpiresAtUtc);
