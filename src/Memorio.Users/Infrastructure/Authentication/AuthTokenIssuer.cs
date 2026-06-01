using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Memorio.Users.Infrastructure.Persistence;

namespace Memorio.Users.Infrastructure.Authentication;

public sealed class AuthTokenIssuer : IAuthTokenIssuer
{
    private const string BearerTokenType = "Bearer";

    private readonly UsersDbContext _dbContext;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly JwtOptions _options;
    private readonly TimeProvider _clock;

    public AuthTokenIssuer(
        UsersDbContext dbContext,
        IAccessTokenGenerator accessTokenGenerator,
        JwtOptions options,
        TimeProvider clock)
    {
        _dbContext = dbContext;
        _accessTokenGenerator = accessTokenGenerator;
        _options = options;
        _clock = clock;
    }

    public async Task<AuthResponse> IssueAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var accessToken = _accessTokenGenerator.Generate(user);
        var refreshToken = RefreshToken.Issue(user.Id, _options.RefreshTokenLifetime, _clock);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            accessToken.Value,
            refreshToken.Token,
            BearerTokenType,
            (int)_options.AccessTokenLifetime.TotalSeconds);
    }
}
