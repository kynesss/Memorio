using ErrorOr;
using MediatR;
using Memorio.Users.Application.Contracts;

namespace Memorio.Users.Application.Auth.Register;

public sealed record RegisterUserCommand(string Email, string Password) : IRequest<ErrorOr<AuthResponse>>;
