using Memorio.Users.Domain;

namespace Memorio.Users.Application.Abstractions;

public interface IRefreshTokenStore
{
    Task<RefreshToken?> FindAsync(string token, CancellationToken cancellationToken);
}
