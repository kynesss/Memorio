using ErrorOr;
using MediatR;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Microsoft.AspNetCore.Identity;

namespace Memorio.Users.Application.Auth.Register;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ErrorOr<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthTokenIssuer _tokenIssuer;
    private readonly TimeProvider _clock;

    public RegisterUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IAuthTokenIssuer tokenIssuer,
        TimeProvider clock)
    {
        _userManager = userManager;
        _tokenIssuer = tokenIssuer;
        _clock = clock;
    }

    public async Task<ErrorOr<AuthResponse>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var user = ApplicationUser.Create(command.Email, _clock);

        var result = await _userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            return ToErrors(result);
        }

        return await _tokenIssuer.IssueAsync(user, cancellationToken);
    }

    private static List<Error> ToErrors(IdentityResult result) =>
        result.Errors.Select(ToError).ToList();

    private static Error ToError(IdentityError error) =>
        error.Code.Contains("Duplicate", StringComparison.OrdinalIgnoreCase)
            ? Error.Conflict(error.Code, error.Description)
            : Error.Validation(error.Code, error.Description);
}
