using System.Security.Cryptography;
using Memorio.Shared.Domain;

namespace Memorio.Users.Domain;

public sealed class RefreshToken : BaseEntity
{
    private const int TokenSizeInBytes = 64;

    private RefreshToken()
    {
    }

    public string Token { get; private set; } = default!;

    public Guid UserId { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public static RefreshToken Issue(Guid userId, TimeSpan lifetime, TimeProvider clock) => new()
    {
        UserId = userId,
        Token = GenerateSecureToken(),
        ExpiresAt = clock.GetUtcNow().UtcDateTime.Add(lifetime),
    };

    public bool IsActive(TimeProvider clock) =>
        RevokedAt is null && clock.GetUtcNow().UtcDateTime < ExpiresAt;

    public void Revoke(TimeProvider clock) => RevokedAt = clock.GetUtcNow().UtcDateTime;

    private static string GenerateSecureToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenSizeInBytes));
}
