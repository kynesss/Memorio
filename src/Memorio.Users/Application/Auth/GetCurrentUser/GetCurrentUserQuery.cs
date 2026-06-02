using ErrorOr;
using MediatR;
using Memorio.Users.Application.Contracts;

namespace Memorio.Users.Application.Auth.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<ErrorOr<UserResponse>>;
