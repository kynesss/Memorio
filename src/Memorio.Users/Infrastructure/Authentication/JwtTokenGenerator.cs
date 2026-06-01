using System.Security.Claims;
using System.Text;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Domain;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Memorio.Users.Infrastructure.Authentication;

public sealed class JwtTokenGenerator : IAccessTokenGenerator
{
    private readonly JwtOptions _options;
    private readonly TimeProvider _clock;

    public JwtTokenGenerator(JwtOptions options, TimeProvider clock)
    {
        _options = options;
        _clock = clock;
    }

    public AccessToken Generate(ApplicationUser user)
    {
        var issuedAt = _clock.GetUtcNow().UtcDateTime;
        var expiresAt = issuedAt.Add(_options.AccessTokenLifetime);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ]),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return new AccessToken(token, expiresAt);
    }
}
