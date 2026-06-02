using ErrorOr;
using MediatR;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Microsoft.AspNetCore.Identity;

namespace Memorio.Users.Application.Auth.Refresh;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ErrorOr<AuthResponse>>
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

    public async Task<ErrorOr<AuthResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenStore.FindAsync(command.RefreshToken, cancellationToken);
        if (refreshToken is null || !refreshToken.IsActive(_clock))
        {
            return Error.Unauthorized(description: "Invalid or expired refresh token.");
        }

        refreshToken.Revoke(_clock);

        var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
        if (user is null)
        {
            return Error.Unauthorized();
        }

        return await _tokenIssuer.IssueAsync(user, cancellationToken);
    }
}
