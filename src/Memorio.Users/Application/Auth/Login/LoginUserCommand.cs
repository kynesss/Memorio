using ErrorOr;
using MediatR;
using Memorio.Users.Application.Contracts;

namespace Memorio.Users.Application.Auth.Login;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<ErrorOr<AuthResponse>>;
