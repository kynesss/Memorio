using Microsoft.AspNetCore.Identity;

namespace Memorio.Users.Domain;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    private readonly List<RefreshToken> _refreshTokens = new();

    public ApplicationUser()
    {
        Id = Guid.NewGuid();
    }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public static ApplicationUser Create(string email, TimeProvider clock) => new()
    {
        UserName = email,
        Email = email,
        CreatedAt = clock.GetUtcNow().UtcDateTime,
    };
}
