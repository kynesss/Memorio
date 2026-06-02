using ErrorOr;
using MediatR;
using Memorio.Users.Application.Contracts;
using Memorio.Users.Domain;
using Microsoft.AspNetCore.Identity;

namespace Memorio.Users.Application.Auth.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ErrorOr<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

    public async Task<ErrorOr<UserResponse>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
        {
            return Error.Unauthorized();
        }

        return new UserResponse(user.Id, user.Email!, user.CreatedAt);
    }
}
