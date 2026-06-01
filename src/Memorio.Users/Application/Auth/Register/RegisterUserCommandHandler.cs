using MediatR;
using Memorio.Users.Application.Abstractions;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Microsoft.AspNetCore.Identity;
using ValidationException = Memorio.Shared.Exceptions.ValidationException;

namespace Memorio.Users.Application.Auth.Register;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
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

    public async Task<AuthResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var user = ApplicationUser.Create(command.Email, _clock);

        var result = await _userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            throw new ValidationException(ToErrors(result));
        }

        return await _tokenIssuer.IssueAsync(user, cancellationToken);
    }

    private static IReadOnlyDictionary<string, string[]> ToErrors(IdentityResult result) =>
        result.Errors
            .GroupBy(error => error.Code)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());
}
