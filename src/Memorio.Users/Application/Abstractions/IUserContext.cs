namespace Memorio.Users.Application.Abstractions;

public interface IUserContext
{
    Guid? UserId { get; }
}
