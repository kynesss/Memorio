using MediatR;
using Memorio.Shared.Exceptions;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Microsoft.AspNetCore.Identity;

namespace Memorio.Users.Application.Auth.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<UserResponse> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
        {
            throw new UnauthorizedException();
        }

        return new UserResponse(user.Id, user.Email!, user.CreatedAt);
    }
}
