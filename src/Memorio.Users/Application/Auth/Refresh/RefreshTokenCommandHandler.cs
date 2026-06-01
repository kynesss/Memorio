using MediatR;
using Memorio.Shared.Exceptions;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Microsoft.AspNetCore.Identity;

namespace Memorio.Users.Application.Auth.Refresh;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthTokenIssuer _tokenIssuer;
    private readonly TimeProvider _clock;

    public RefreshTokenCommandHandler(
        IRefreshTokenStore refreshTokenStore,
        UserManager<ApplicationUser> userManager,
        IAuthTokenIssuer tokenIssuer,
        TimeProvider clock)
    {
        _refreshTokenStore = refreshTokenStore;
        _userManager = userManager;
        _tokenIssuer = tokenIssuer;
        _clock = clock;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenStore.FindAsync(command.RefreshToken, cancellationToken);
        if (refreshToken is null || !refreshToken.IsActive(_clock))
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        refreshToken.Revoke(_clock);

        var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
        if (user is null)
        {
            throw new UnauthorizedException();
        }

        return await _tokenIssuer.IssueAsync(user, cancellationToken);
    }
}
