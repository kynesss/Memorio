using Memorio.Users.Application.Abstractions;
using Memorio.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Users.Infrastructure.Persistence;

public sealed class RefreshTokenStore : IRefreshTokenStore
{
    private readonly UsersDbContext _dbContext;

    public RefreshTokenStore(UsersDbContext dbContext) => _dbContext = dbContext;

    public Task<RefreshToken?> FindAsync(string token, CancellationToken cancellationToken) =>
        _dbContext.RefreshTokens.FirstOrDefaultAsync(stored => stored.Token == token, cancellationToken);
}
