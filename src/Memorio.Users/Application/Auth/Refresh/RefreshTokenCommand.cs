using ErrorOr;
using MediatR;
using Memorio.Users.Application.Contracts;

namespace Memorio.Users.Application.Auth.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<ErrorOr<AuthResponse>>;
