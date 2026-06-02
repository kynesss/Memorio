using ErrorOr;
using MediatR;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Microsoft.AspNetCore.Identity;

namespace Memorio.Users.Application.Auth.Login;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ErrorOr<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthTokenIssuer _tokenIssuer;

    public LoginUserCommandHandler(UserManager<ApplicationUser> userManager, IAuthTokenIssuer tokenIssuer)
    {
        _userManager = userManager;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, command.Password))
        {
            return Error.Unauthorized(description: "Invalid email or password.");
        }

        return await _tokenIssuer.IssueAsync(user, cancellationToken);
    }
}
