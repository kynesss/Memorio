namespace Memorio.Users.Application.Contracts;

public sealed record UserResponse(Guid Id, string Email, DateTime CreatedAt);
